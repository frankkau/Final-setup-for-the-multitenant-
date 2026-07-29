using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CouncilRevenueCollection.Application.Implementation;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITenantService _tenantService;

    public RefreshTokenService(
        ApplicationDbContext context,
        UserManager<User> userManager,
        IJwtTokenService jwtTokenService,
        ITenantService tenantService)
    {
        _context = context;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _tenantService = tenantService;
    }

    public async Task<string> CreateAsync(
        User user,
        string jwtId,
        string ipAddress)
    {
        var refreshToken = GenerateToken();

        var entity = new RefreshToken
        {
            Id = Guid.NewGuid().ToString(),
            UserId = user.Id,
            TenantId = user.TenantId,
            JwtId = jwtId,
            TokenHash = Hash(refreshToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedByIp = ipAddress
        };

        _context.RefreshTokens.Add(entity);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<RefreshToken?> FindAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            return null;

        var hash = Hash(refreshToken.Trim());

        return await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.TokenHash == hash &&
                x.TenantId == tenantId);
    }

    public async Task<LoginResponseDto> RefreshAsync(
        string refreshToken,
        string ipAddress)
    {
        var storedToken = await FindAsync(refreshToken);

        if (storedToken == null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (storedToken.IsRevoked)
        {
            await RevokeAllAsync(storedToken.UserId);

            throw new UnauthorizedAccessException(
                "Refresh token reuse detected.");
        }

        if (storedToken.IsExpired)
            throw new UnauthorizedAccessException(
                "Refresh token has expired.");

        if (storedToken.User == null)
            throw new UnauthorizedAccessException(
                "User not found.");

        if (storedToken.User.TenantId != storedToken.TenantId)
            throw new UnauthorizedAccessException(
                "Tenant validation failed.");

        if (await _userManager.IsLockedOutAsync(storedToken.User))
            throw new UnauthorizedAccessException(
                "User account is locked.");

        var currentSecurityStamp =
            await _userManager.GetSecurityStampAsync(storedToken.User);

        if (currentSecurityStamp != storedToken.User.SecurityStamp)
        {
            await RevokeAllAsync(storedToken.UserId);

            throw new UnauthorizedAccessException(
                "User credentials have changed.");
        }

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;

        var login =
            await _jwtTokenService.GenerateTokenAsync(storedToken.User);

        var jwtId = GetJwtId(login.AccessToken);

        var newRefreshToken = await CreateAsync(
            storedToken.User,
            jwtId,
            ipAddress);

        storedToken.ReplacedByTokenHash = Hash(newRefreshToken);

        login.RefreshToken = newRefreshToken;

        await _context.SaveChangesAsync();

        return login;
    }

    public async Task RevokeAsync(
        string refreshToken,
        string ipAddress)
    {
        var token = await FindAsync(refreshToken);

        if (token == null)
            return;

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;

        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllAsync(string userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId &&
                        x.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    private static string GetJwtId(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadJwtToken(accessToken).Id;
    }

    private static string GenerateToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
    }

    private static string Hash(string value)
    {
        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(value)));
    }
}
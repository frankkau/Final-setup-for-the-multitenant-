
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using CouncilRevenueCollection.Application.InterfaceClass;
namespace CouncilRevenueCollection.Application.Implementation;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITenantService _tenantService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthenticationService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITenantService tenantService,
        IJwtTokenService jwtTokenService,
        ILogger<AuthenticationService> logger,
        IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tenantService = tenantService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new UnauthorizedAccessException("Tenant could not be resolved.");

        var email = dto.Email.Trim().ToUpperInvariant();

        var user = await _userManager.Users
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.NormalizedEmail == email);

        // Never reveal whether the email exists
        if (user == null)
        {
            _logger.LogWarning(
                "Login failed. Unknown user. Tenant={TenantId}, Email={Email}",
                tenantId,
                dto.Email);

            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!user.EmailConfirmed)
            throw new UnauthorizedAccessException("Email has not been confirmed.");

        if (await _userManager.IsLockedOutAsync(user))
            throw new UnauthorizedAccessException("Account is locked.");

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Invalid password for user {UserId}",
                user.Id);

            throw new UnauthorizedAccessException("Invalid email or password.");
        }

       var response = await _jwtTokenService.GenerateTokenAsync(user);

            var ipAddress =
                _signInManager.Context?.Connection?.RemoteIpAddress?.ToString()
                ?? "Unknown";

            // Read the JWT to obtain the JTI
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(response.AccessToken);

            response.RefreshToken = await _refreshTokenService.CreateAsync(
                user,
                jwt.Id,
                ipAddress);

            _logger.LogInformation(
                "User {UserId} logged in successfully.",
                user.Id);

            return response;
    }

    public async Task<LoginResponseDto> RefreshAsync(string refreshToken)
{
    if (string.IsNullOrWhiteSpace(refreshToken))
        throw new UnauthorizedAccessException("Refresh token is required.");

    var tenantId = _tenantService.GetCurrentTenantId();

    if (string.IsNullOrWhiteSpace(tenantId))
        throw new UnauthorizedAccessException("Tenant could not be resolved.");

        try
        {
            var ipAddress = _signInManager.Context?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;

            var response = await _refreshTokenService.RefreshAsync(refreshToken, ipAddress);

        _logger.LogInformation(
            "Refresh token exchanged successfully for tenant {TenantId}.",
            tenantId);

        return response;
    }
    catch (UnauthorizedAccessException)
    {
        _logger.LogWarning(
            "Invalid refresh token used for tenant {TenantId}.",
            tenantId);

        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Error refreshing token for tenant {TenantId}.",
            tenantId);

        throw;
    }
}

    public async Task LogoutAsync(string refreshToken)
{
    if (string.IsNullOrWhiteSpace(refreshToken))
        return;

    try
    {
        var ipAddress = _signInManager.Context?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;

        await _refreshTokenService.RevokeAsync(
            refreshToken,
            ipAddress);

        _logger.LogInformation("Refresh token revoked successfully.");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to revoke refresh token.");
        throw;
    }
}

    public async Task ChangePasswordAsync(ChangePasswordDto dto)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new UnauthorizedAccessException("Tenant could not be resolved.");

        var user = await _userManager.Users
            .FirstOrDefaultAsync(x =>
                x.TenantId == tenantId &&
                x.UserName == _signInManager.Context.User.Identity!.Name);

        if (user == null)
            throw new UnauthorizedAccessException();

        var result = await _userManager.ChangePasswordAsync(
            user,
            dto.CurrentPassword,
            dto.NewPassword);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(
                ", ",
                result.Errors.Select(e => e.Description)));
        }
        await _refreshTokenService.RevokeAllAsync(user.Id);

        _logger.LogInformation(
            "Password changed for user {UserId}",
            user.Id);
    }

    public async Task<UserResponseDto> MeAsync()
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new UnauthorizedAccessException();

        var user = await _userManager.GetUserAsync(_signInManager.Context.User);

        if (user == null)
            throw new UnauthorizedAccessException();

        var roles = await _userManager.GetRolesAsync(user);

        return new UserResponseDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.TenantId,
            user.Email!,
            roles.ToList());
    }
}
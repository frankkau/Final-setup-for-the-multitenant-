

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CouncilRevenueCollection.Application.Implementation;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public JwtTokenService(
        IConfiguration configuration,
        UserManager<User> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

        public async Task<LoginResponseDto> GenerateTokenAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            new Claim(CustomClaimTypes.TenantId, user.TenantId),

            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),

            new Claim("SecurityStamp", user.SecurityStamp!)
        };

    foreach (var role in roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));

    var credentials = new SigningCredentials(
        key,
        SecurityAlgorithms.HmacSha256);

    var expiry = DateTime.UtcNow.AddMinutes(15);

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: expiry,
        signingCredentials: credentials);

    return new LoginResponseDto
    {
        AccessToken = new JwtSecurityTokenHandler().WriteToken(token),

        // Will be assigned by RefreshTokenService
        RefreshToken = string.Empty,

        ExpiresAt = expiry,

        UserId = user.Id,
        Email = user.Email!,
        TenantId = user.TenantId,
        Roles = roles
    };
}
}
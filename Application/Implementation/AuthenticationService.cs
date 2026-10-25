
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
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
    private readonly IHttpContextAccessor _httpContextAccessor;


    public AuthenticationService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITenantService tenantService,
        IJwtTokenService jwtTokenService,
        ILogger<AuthenticationService> logger,
        IHttpContextAccessor httpContextAccessor,   
        IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tenantService = tenantService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<LoginResponseDto> LoginAsync(
    LoginRequestDto dto)
{
    // -----------------------------------------------------
    // 1. Resolve tenant
    // -----------------------------------------------------

    var tenantId =
        _tenantService.GetCurrentTenantId();

    if (string.IsNullOrWhiteSpace(tenantId))
    {
        throw new UnauthorizedAccessException(
            "Tenant could not be resolved.");
    }

    tenantId = tenantId.Trim().ToLowerInvariant();


    // -----------------------------------------------------
    // 2. Normalize email
    // -----------------------------------------------------

    var email =
        dto.Email.Trim().ToUpperInvariant();


    // -----------------------------------------------------
    // 3. Find user inside current tenant
    // -----------------------------------------------------

    var user = await _userManager.Users
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(
            x =>
                x.TenantId == tenantId &&
                x.NormalizedEmail == email);


    if (user is null)
    {
        _logger.LogWarning(
            "Login failed. Unknown user. Tenant={TenantId}, Email={Email}",
            tenantId,
            dto.Email);

        throw new UnauthorizedAccessException(
            "Invalid email or password.");
    }


    // -----------------------------------------------------
    // 4. Check email confirmation
    // -----------------------------------------------------

    if (!user.EmailConfirmed)
    {
        throw new UnauthorizedAccessException(
            "Email has not been confirmed.");
    }


    // -----------------------------------------------------
    // 5. Check lockout
    // -----------------------------------------------------

    if (await _userManager.IsLockedOutAsync(user))
    {
        throw new UnauthorizedAccessException(
            "Account is locked.");
    }


    // -----------------------------------------------------
    // 6. Validate password
    // -----------------------------------------------------

    var passwordResult =
        await _signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            lockoutOnFailure: true);


    if (!passwordResult.Succeeded)
    {
        _logger.LogWarning(
            "Invalid password for User={UserId}, Tenant={TenantId}",
            user.Id,
            tenantId);

        throw new UnauthorizedAccessException(
            "Invalid email or password.");
    }


    // -----------------------------------------------------
    // 7. Generate JWT
    // -----------------------------------------------------

    var response =
        await _jwtTokenService.GenerateTokenAsync(
            user);


    // -----------------------------------------------------
    // 8. Get JWT ID
    // -----------------------------------------------------

    var handler =
        new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

    var jwt =
        handler.ReadJwtToken(
            response.AccessToken);


    // -----------------------------------------------------
    // 9. Get IP address
    // -----------------------------------------------------

    var ipAddress =
        _httpContextAccessor.HttpContext?
            .Connection
            .RemoteIpAddress?
            .ToString()
        ?? "Unknown";


    // -----------------------------------------------------
    // 10. Create refresh token
    // -----------------------------------------------------

    response.RefreshToken =
        await _refreshTokenService.CreateAsync(
            user,
            jwt.Id,
            ipAddress);


    // -----------------------------------------------------
    // 11. Log successful login
    // -----------------------------------------------------

    _logger.LogInformation(
        "User {UserId} logged in successfully for tenant {TenantId}.",
        user.Id,
        tenantId);


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
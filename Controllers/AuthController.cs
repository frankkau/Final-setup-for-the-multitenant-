using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
     private readonly IRefreshTokenService _refreshTokenService;


    public AuthController(
        IAuthenticationService authenticationService,
        IRefreshTokenService refreshTokenService)
    {
        _authenticationService = authenticationService;
        _refreshTokenService = refreshTokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        LoginRequestDto dto)
    {
        return Ok(await _authenticationService.LoginAsync(dto));
    }

    // In AuthController
    
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest("Refresh token is required.");

        var ipAddress =
            HttpContext.Connection.RemoteIpAddress?.ToString()
            ?? "Unknown";

        try
        {
            var response = await _refreshTokenService.RefreshAsync(
                request.RefreshToken,
                ipAddress);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }

     [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest("Refresh token is required.");

        var ipAddress =
            HttpContext.Connection.RemoteIpAddress?.ToString()
            ?? "Unknown";

        await _refreshTokenService.RevokeAsync(
            request.RefreshToken,
            ipAddress);

        return Ok(new
        {
            message = "Logged out successfully."
        });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponseDto>> Me()
    {
        return Ok(await _authenticationService.MeAsync());
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordDto dto)
    {
        await _authenticationService.ChangePasswordAsync(dto);

        return NoContent();
    }

    [Authorize]
[HttpGet("claims")]
public IActionResult Claims()
{
    return Ok(User.Claims.Select(c => new
    {
        c.Type,
        c.Value
    }));
}


[AllowAnonymous]
[HttpGet("headers")]
public IActionResult Headers()
{
    return Ok(Request.Headers.ToDictionary(
        h => h.Key,
        h => h.Value.ToString()));
}
}
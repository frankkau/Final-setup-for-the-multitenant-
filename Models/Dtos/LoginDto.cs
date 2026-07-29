namespace CouncilRevenueCollection.Models.Dtos;

public record LoginDtoDto(string Email, string Password);

public sealed class LoginResponseDto
{
    public required string AccessToken { get; set; }

    public required string RefreshToken { get; set; }

    public required DateTime ExpiresAt { get; set; }

    public string UserId { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string TenantId { get; set; } = default!;

    public IEnumerable<string> Roles { get; set; } = [];
}

public sealed class LoginRequestDto
{
    public required string Email { get; set; }

    public required string Password { get; set; }
}
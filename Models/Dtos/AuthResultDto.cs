namespace CouncilRevenueCollection.Models.Dtos;

public record AuthResultDto(string AccessToken, string RefreshToken, DateTime ExpiresAt);
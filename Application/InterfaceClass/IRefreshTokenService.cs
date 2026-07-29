using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Entity;

public interface IRefreshTokenService
{
    Task<string> CreateAsync(
        User user,
        string jwtId,
        string ipAddress);

    Task<LoginResponseDto> RefreshAsync(
        string refreshToken,
        string ipAddress);

    Task RevokeAsync(
        string refreshToken,
        string ipAddress);

    Task RevokeAllAsync(string userId);

    Task<RefreshToken?> FindAsync(string refreshToken);
}
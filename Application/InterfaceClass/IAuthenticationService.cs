using CouncilRevenueCollection.Models.Dtos;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IAuthenticationService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);

    Task<LoginResponseDto> RefreshAsync(string refreshToken);

    Task LogoutAsync(string refreshToken);

    Task ChangePasswordAsync(ChangePasswordDto dto);

    Task<UserResponseDto> MeAsync();
}
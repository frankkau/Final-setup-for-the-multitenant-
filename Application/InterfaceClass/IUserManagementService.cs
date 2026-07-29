using CouncilRevenueCollection.Models.Dtos;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IUserManagementService
{
    Task<IEnumerable<UserResponseDto>> GetAllAsync();

    Task<UserResponseDto?> GetByIdAsync(string id);

    Task<UserResponseDto> CreateAsync(CreateUserDto dto);

    Task<UserResponseDto> UpdateAsync(string id, UpdateUserDto dto);

    Task DeleteAsync(string id);

    Task ResetPasswordAsync(string userId, string newPassword);

    Task LockUserAsync(string userId);

    Task UnlockUserAsync(string userId);
}
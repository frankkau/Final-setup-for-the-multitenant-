using CouncilRevenueCollection.Models.Dtos;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IRoleManagementService
{
    Task<IEnumerable<RoleDto>> GetAllAsync();

    Task CreateAsync(CreateRoleDto dto);

    Task DeleteAsync(string roleName);

    Task AssignUserAsync(string userId, string roleName);

    Task RemoveUserAsync(string userId, string roleName);

    Task<IList<string>> GetUserRolesAsync(string userId);
}
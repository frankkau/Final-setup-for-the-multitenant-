using CouncilRevenueCollection.Models.Dtos;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface ITenantManagementService
{
    Task<IEnumerable<TenantResponseDto>> GetAllAsync();

    Task<TenantResponseDto?> GetByIdAsync(string id);
    Task<TenantResponseDto?> GetByDomainAsync(string domain);

    Task<TenantResponseDto> CreateAsync(CreateTenantDto dto, CancellationToken cancellationToken = default);

    Task UpdateAsync(string id, UpdateTenantDto dto);

    Task<IReadOnlyList<UserResponseDto>> GetUsersByTenantAsync(
    string tenantId,
    CancellationToken cancellationToken);

    Task DeleteAsync(string id);
}
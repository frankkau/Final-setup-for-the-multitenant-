using CouncilRevenueCollection.Models.Dtos;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface ITenantManagementService
{
    Task<IEnumerable<TenantResponseDto>> GetAllAsync();

    Task<TenantResponseDto?> GetByIdAsync(string id);
    Task<TenantResponseDto?> GetByDomainAsync(string domain);

    Task<TenantResponseDto> CreateAsync(CreateTenantDto dto);

    Task UpdateAsync(string id, UpdateTenantDto dto);

    Task DeleteAsync(string id);
}
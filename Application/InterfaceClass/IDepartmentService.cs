using CouncilRevenueCollection.Models.Dtos.Department;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<DepartmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<DepartmentDto> CreateAsync(
        CreateDepartmentDto dto,
        CancellationToken cancellationToken);

    Task<DepartmentDto> UpdateAsync(
        Guid id,
        UpdateDepartmentDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}
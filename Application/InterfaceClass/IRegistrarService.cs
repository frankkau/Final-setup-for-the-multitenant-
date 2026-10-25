using CouncilRevenueCollection.Models.Dtos.Registrar;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IRegistrarService
{
    Task<RegistrarDto> CreateAsync(
        CreateRegistrarDto dto,
        CancellationToken cancellationToken);

    Task<RegistrarDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<RegistrarDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<RegistrarDto> UpdateAsync(
        Guid id,
        UpdateRegistrarDto dto,
        CancellationToken cancellationToken);
    Task<RegistrarDto?> GetByEmployeeNoAsync(
        string employeeNo,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);
}
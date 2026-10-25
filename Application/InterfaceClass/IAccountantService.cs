using CouncilRevenueCollection.Models.Dtos.Accountant;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IAccountantService
{
    Task<AccountantDto> CreateAsync(
        CreateAccountantDto dto,
        CancellationToken cancellationToken);

    Task<AccountantDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AccountantDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<AccountantDto?> GetByEmployeeNoAsync(
        string employeeNo,
        CancellationToken cancellationToken);

    Task<AccountantDto> UpdateAsync(
        Guid id,
        UpdateAccountantDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);
}
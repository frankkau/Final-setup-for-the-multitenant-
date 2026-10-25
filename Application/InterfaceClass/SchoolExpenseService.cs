using CouncilRevenueCollection.Models.Dtos.SchoolExpense;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface ISchoolExpenseService
{
    Task<IReadOnlyList<SchoolExpenseListDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<SchoolExpenseDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<SchoolExpenseDto> CreateAsync(
        CreateSchoolExpenseDto dto,
        CancellationToken cancellationToken);

    Task<SchoolExpenseDto> UpdateAsync(
        Guid id,
        UpdateSchoolExpenseDto dto,
        CancellationToken cancellationToken);

    Task CancelAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task ApproveAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<SchoolExpenseListDto>>
        GetByTermAsync(
            Guid termId,
            CancellationToken cancellationToken);

    Task<IReadOnlyList<SchoolExpenseListDto>>
        GetByAcademicYearAsync(
            Guid academicYearId,
            CancellationToken cancellationToken);
}
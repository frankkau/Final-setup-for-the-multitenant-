using CouncilRevenueCollection.Models.Dtos.LibraryLoan;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface ILibraryLoanService
{
    Task<IReadOnlyList<LibraryLoanDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LibraryLoanDto>> GetByStudentAsync(
        Guid studentId,
        CancellationToken cancellationToken);

    Task<LibraryLoanDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<LibraryLoanDto> BorrowAsync(
        CreateLibraryLoanDto dto,
        CancellationToken cancellationToken);

    Task ReturnAsync(
        Guid id,
        decimal fineAmount,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}
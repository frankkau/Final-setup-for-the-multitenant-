using CouncilRevenueCollection.Models.Dtos.ExpenseReceipt;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IExpenseReceiptService
{
    Task<IReadOnlyList<ExpenseReceiptDto>> GetByExpenseAsync(
        Guid schoolExpenseId,
        CancellationToken cancellationToken);

    Task<ExpenseReceiptDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<ExpenseReceiptDto> CreateAsync(
        CreateExpenseReceiptDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}
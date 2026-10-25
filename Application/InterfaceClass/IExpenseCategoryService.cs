using CouncilRevenueCollection.Models.Dtos.ExpenseCategory;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface IExpenseCategoryService
{
    Task<IReadOnlyList<ExpenseCategoryDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<ExpenseCategoryDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<ExpenseCategoryDto> CreateAsync(
        CreateExpenseCategoryDto dto,
        CancellationToken cancellationToken);

    Task<ExpenseCategoryDto> UpdateAsync(
        Guid id,
        UpdateExpenseCategoryDto dto,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);
}
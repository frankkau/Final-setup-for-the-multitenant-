namespace CouncilRevenueCollection.Models.Dtos.ExpenseCategory;

public class ExpenseCategoryDto
{
    public Guid ExpenseCategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
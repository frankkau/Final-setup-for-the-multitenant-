namespace CouncilRevenueCollection.Models.Dtos.ExpenseCategory;

public class CreateExpenseCategoryDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
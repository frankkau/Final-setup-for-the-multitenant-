namespace CouncilRevenueCollection.Models.Dtos.ExpenseCategory;

public class UpdateExpenseCategoryDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}

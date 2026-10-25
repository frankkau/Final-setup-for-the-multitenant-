namespace CouncilRevenueCollection.Models.Dtos.SchoolExpense;

public class UpdateSchoolExpenseDto
{
    public Guid ExpenseCategoryId { get; set; }

    public Guid AcademicYearId { get; set; }

    public Guid TermId { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateOnly ExpenseDate { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string? ReferenceNumber { get; set; }

    public string? Payee { get; set; }

    public string? Remarks { get; set; }
}
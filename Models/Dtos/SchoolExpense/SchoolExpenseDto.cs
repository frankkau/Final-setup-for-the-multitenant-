namespace CouncilRevenueCollection.Models.Dtos.SchoolExpense;

public class SchoolExpenseDto
{
    public Guid SchoolExpenseId { get; set; }

    public string ExpenseNumber { get; set; } = string.Empty;

    public Guid ExpenseCategoryId { get; set; }

    public string ExpenseCategoryName { get; set; } = string.Empty;

    public Guid AcademicYearId { get; set; }

    public Guid TermId { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateOnly ExpenseDate { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string? ReferenceNumber { get; set; }

    public string? Payee { get; set; }

    public string? Remarks { get; set; }

    public bool IsApproved { get; set; }

    public bool IsCancelled { get; set; }

    public IReadOnlyList<ExpenseReceiptDto> Receipts { get; set; }
        = Array.Empty<ExpenseReceiptDto>();
}
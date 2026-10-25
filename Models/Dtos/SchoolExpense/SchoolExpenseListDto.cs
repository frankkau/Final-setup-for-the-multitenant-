namespace CouncilRevenueCollection.Models.Dtos.SchoolExpense;

public class SchoolExpenseListDto
{
    public Guid SchoolExpenseId { get; set; }

    public string ExpenseNumber { get; set; } = string.Empty;

    public string ExpenseCategory { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateOnly ExpenseDate { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string? Payee { get; set; }

    public bool IsApproved { get; set; }

    public bool IsCancelled { get; set; }

    public bool HasReceipt { get; set; }
}

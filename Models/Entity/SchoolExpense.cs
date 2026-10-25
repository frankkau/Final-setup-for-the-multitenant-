using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class SchoolExpense : IMustHaveTenant
{
    public Guid SchoolExpenseId { get; set; }

    public string TenantId { get; set; } = default!;


    public Guid ExpenseCategoryId { get; set; }

    public Guid AcademicYearId { get; set; }

    public Guid TermId { get; set; }


    public string ExpenseNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;


    public decimal Amount { get; set; }


    public DateOnly ExpenseDate { get; set; }


    public string PaymentMethod { get; set; } = string.Empty;

    public string? ReferenceNumber { get; set; }

    public string? Payee { get; set; }

    public string? Remarks { get; set; }


    public bool IsApproved { get; set; }

    public bool IsCancelled { get; set; }


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public ExpenseCategory ExpenseCategory { get; set; }
        = default!;

    // public AcademicYear AcademicYear { get; set; }
    //     = default!;

    // public Term Term { get; set; }
    //     = default!;


    public ICollection<ExpenseReceipt> Receipts { get; set; }
        = new List<ExpenseReceipt>();
}
using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class Invoice : IMustHaveTenant
{
    public Guid InvoiceId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid StudentId { get; set; }

    public Guid FeeStructureId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal Balance { get; set; }

    public DateOnly InvoiceDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public bool IsPaid { get; set; }

    public bool IsCancelled { get; set; }

    public string? Remarks { get; set; }


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public Student Student { get; set; } = default!;

    public FeeStructure FeeStructure { get; set; } = default!;

    public ICollection<FeePayment> Payments { get; set; }
        = new List<FeePayment>();
}
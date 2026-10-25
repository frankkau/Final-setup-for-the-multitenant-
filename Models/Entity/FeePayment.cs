using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class FeePayment : IMustHaveTenant
{
    public Guid FeePaymentId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid StudentId { get; set; }

    public Guid FeeStructureId { get; set; }

    public Guid InvoiceId { get; set; }

    public decimal AmountPaid { get; set; }

    public DateOnly PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string? ReferenceNumber { get; set; }

    public string? Remarks { get; set; }


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public Student Student { get; set; } = default!;

    public FeeStructure FeeStructure { get; set; } = default!;

    public Invoice Invoice { get; set; } = default!;
}
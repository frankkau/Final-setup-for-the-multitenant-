using CouncilRevenueCollection.Common;
using SchoolManagement.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class FeeStructure : IMustHaveTenant
{
    public Guid FeeStructureId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid ClassId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public bool IsMandatory { get; set; } = true;

    public bool IsActive { get; set; } = true;


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public Class Class { get; set; } = default!;

    public ICollection<FeePayment> FeePayments { get; set; }
        = new List<FeePayment>();
}
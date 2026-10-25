using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class Department : IMustHaveTenant
{
    public Guid DepartmentId { get; set; }

    public string TenantId { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

     
    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public ICollection<Teacher> Teachers { get; set; }
        = new List<Teacher>();
}
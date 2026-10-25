using CouncilRevenueCollection.Common;

namespace CouncilRevenueCollection.Models.Entity;

public class Accountant : IMustHaveTenant
{
    public Guid AccountantId { get; set; }

    public string TenantId { get; set; } = default!;

    public string EmployeeNo { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string UserId { get; set; } = default!;

    public User User { get; set; } = default!;

    public Guid? DepartmentId { get; set; }

    public Department? Department { get; set; }

    public bool Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public Tenant Tenant { get; set; } = default!;
}
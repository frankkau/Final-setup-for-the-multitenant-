// namespace CouncilRevenueCollection.Models.Entity;

using CouncilRevenueCollection.Common;
using Microsoft.AspNetCore.Identity;

namespace CouncilRevenueCollection.Models.Entity;

public class ApplicationRole : IdentityRole<string>, IMustHaveTenant
{
    public required string TenantId { get; set; }

    public required string Description { get; set; }

    public bool IsSystemRole { get; set; }
}
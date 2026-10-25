namespace CouncilRevenueCollection.Models.Entity;

using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

public class StudentGuardian : IMustHaveTenant
{
    public Guid StudentGuardianId { get; set; }


    public string TenantId { get; set; } = default!;



    public Guid StudentId { get; set; }


    public Guid GuardianId { get; set; }



    public bool IsPrimary { get; set; }


    public bool CanPickup { get; set; }



    public Tenant Tenant { get; set; } = default!;


    public Student Student { get; set; } = default!;


    public Guardian Guardian { get; set; } = default!;
}
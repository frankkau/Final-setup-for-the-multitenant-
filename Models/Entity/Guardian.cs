namespace CouncilRevenueCollection.Models;

using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Models.Entity;


public class Guardian : IMustHaveTenant
{
    public Guid GuardianId { get; set; }

    public string TenantId { get; set; } = default!;
    public  Tenant Tenant { get; set; } = default!;


    public string FirstName { get; set; } = string.Empty;


    public string LastName { get; set; } = string.Empty;


    public string Relationship { get; set; } = string.Empty;
    // Father, Mother, Uncle, Guardian etc.


    public string Phone { get; set; } = string.Empty;


    public string Email { get; set; } = string.Empty;


    public string Address { get; set; } = string.Empty;


    public bool Status { get; set; } = true;


   

    public ICollection<StudentGuardian> StudentGuardians { get; set; }
        = new List<StudentGuardian>();
}
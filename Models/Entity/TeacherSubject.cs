namespace CouncilRevenueCollection.Models.Entity;

using CouncilRevenueCollection.Common;
using SchoolManagement.Domain.Entities;


public class TeacherSubject : IMustHaveTenant
{
    public Guid TeacherSubjectId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid TeacherId { get; set; }

    public Guid SubjectId { get; set; }

    public bool IsActive { get; set; } = true;


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public Teacher Teacher { get; set; } = default!;

    public Subject Subject { get; set; } = default!;
}
using CouncilRevenueCollection;
using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Models.Entity;

namespace SchoolManagement.Domain.Entities;

public class Subject : IMustHaveTenant
{
    public Guid SubjectId { get; set; }

    public string TenantId { get; set; } = default!;

    public string SubjectCode { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsCompulsory { get; set; }

    public bool IsActive { get; set; } = true;

    // Relationships
    public Tenant Tenant { get; set; } = default!;

    public ICollection<ClassSubject> ClassSubjects { get; set; }
        = new List<ClassSubject>();

    public ICollection<TeacherSubject> TeacherSubjects { get; set; }
        = new List<TeacherSubject>();
}
using CouncilRevenueCollection;
using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Models.Entity;

namespace SchoolManagement.Domain.Entities;

public class ClassSubject : IMustHaveTenant
{
    public Guid ClassSubjectId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid ClassId { get; set; }

    public Guid SubjectId { get; set; }

    // Optional: Subject teacher
    public Guid? TeacherId { get; set; }

    // Number of periods per week
    public int PeriodsPerWeek { get; set; }

    // Indicates if this subject is optional for this class
    public bool IsOptional { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties

    public Tenant Tenant { get; set; } = default!;

    public Class Class { get; set; } = default!;

    public Subject Subject { get; set; } = default!;

    public Teacher? Teacher { get; set; }
    public ICollection<Timetable> Timetables { get; set; }
    = new List<Timetable>();
}
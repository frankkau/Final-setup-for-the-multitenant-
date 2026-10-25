namespace CouncilRevenueCollection.Models.Entity;

using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

public class StudentEnrollment : IMustHaveTenant
{
    public Guid StudentEnrollmentId { get; set; }

    public string TenantId { get; set; } = default!;


    public Guid StudentId { get; set; }

    public Guid ClassId { get; set; }

    public Guid AcademicYearId { get; set; }


    public DateOnly EnrollmentDate { get; set; }

    public bool Status { get; set; }


    public Tenant Tenant { get; set; } = default!;

    public Student Student { get; set; } = default!;

    public Class Class { get; set; } = default!;

      public ICollection<StudentAttendance> Attendances { get; set; }
        = new List<StudentAttendance>();
}
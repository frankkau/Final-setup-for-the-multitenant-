namespace CouncilRevenueCollection.Models.Entity;

using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;


public class StudentAttendance : IMustHaveTenant
{
    public Guid StudentAttendanceId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid StudentId { get; set; }

    public Guid ClassId { get; set; }

    public Guid AcademicYearId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public AttendanceStatus Status { get; set; }

    public string? Remarks { get; set; }
    public Guid StudentEnrollmentId { get; set; }

    public StudentEnrollment StudentEnrollment { get; set; } = default!;

    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public Student Student { get; set; } = default!;

    public Class Class { get; set; } = default!;
}
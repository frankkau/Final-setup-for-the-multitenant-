namespace CouncilRevenueCollection.Models.Dtos.StudentAttendance;

using CouncilRevenueCollection.Common;

public class StudentAttendanceDto
{
    public Guid StudentAttendanceId { get; set; }

    public Guid StudentEnrollmentId { get; set; }

    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public Guid ClassId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public DateOnly AttendanceDate { get; set; }

    public AttendanceStatus Status { get; set; }

    public string? Remarks { get; set; }
}
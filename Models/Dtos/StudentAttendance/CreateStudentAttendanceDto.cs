namespace CouncilRevenueCollection.Models.Dtos.StudentAttendance;

using CouncilRevenueCollection.Common;

public class CreateStudentAttendanceDto
{
    public Guid StudentEnrollmentId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public AttendanceStatus Status { get; set; }

    public string? Remarks { get; set; }
}
namespace CouncilRevenueCollection.Models.Dtos.StudentAttendance;

using CouncilRevenueCollection.Common;

public class UpdateStudentAttendanceDto
{
    public AttendanceStatus Status { get; set; }

    public string? Remarks { get; set; }
}
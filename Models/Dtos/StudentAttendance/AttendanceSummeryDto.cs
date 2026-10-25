namespace CouncilRevenueCollection.Models.Dtos.StudentAttendance;



public class AttendanceSummaryDto
{
    public Guid StudentId { get; set; }

    public Guid StudentEnrollmentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public int TotalDays { get; set; }

    public int PresentDays { get; set; }

    public int AbsentDays { get; set; }

    public int LateDays { get; set; }

    public decimal AttendancePercentage { get; set; }
}

namespace CouncilRevenueCollection.Models.Dtos.Timetable;

public class TimetableListDto
{
    public Guid TimetableId { get; set; }

    public Guid ClassSubjectId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public string? TeacherName { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? RoomNumber { get; set; }

    public bool IsActive { get; set; }
}
namespace CouncilRevenueCollection.Models.Dtos;



public class CreateTimetableDto
{
    public Guid ClassSubjectId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? RoomNumber { get; set; }

    public bool IsActive { get; set; } = true;
}
namespace CouncilRevenueCollection.Models.Dtos.Exam;

public class ExamListDto
{
    public Guid ExamId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; }
}
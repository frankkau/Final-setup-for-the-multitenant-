namespace CouncilRevenueCollection.Models.Dtos.Student;

public class StudentExamDto
{
    public Guid ExamId { get; set; }

    public string ExamName { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; }
}
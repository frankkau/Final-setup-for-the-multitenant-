namespace CouncilRevenueCollection.Models.Dtos.ExamSubject;

public class ExamSubjectListDto
{
    public Guid ExamSubjectId { get; set; }

    public string ExamName { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public string? TeacherName { get; set; }

    public DateOnly ExamDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public decimal TotalMarks { get; set; }

    public bool IsActive { get; set; }
}
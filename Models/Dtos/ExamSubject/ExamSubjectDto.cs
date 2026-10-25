namespace CouncilRevenueCollection.Models.Dtos.ExamSubject;

public class ExamSubjectDto
{
    public Guid ExamSubjectId { get; set; }

    public Guid ExamId { get; set; }

    public string ExamName { get; set; } = string.Empty;

    public Guid ClassId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public Guid SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    public Guid? TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public DateOnly ExamDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public decimal TotalMarks { get; set; }

    public bool IsActive { get; set; }
}
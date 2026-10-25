namespace CouncilRevenueCollection.Models.Dtos.Student;

public class StudentResultDto
{
    public Guid StudentExamResultId { get; set; }

    public Guid ExamId { get; set; }

    public string ExamName { get; set; } = string.Empty;

    public Guid ExamSubjectId { get; set; }

    public Guid SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    public DateOnly ExamDate { get; set; }

    public decimal TotalMarks { get; set; }

    public decimal MarksObtained { get; set; }

    public decimal? GradePoint { get; set; }

    public string? Grade { get; set; }

    public string? Remarks { get; set; }

    public bool IsPublished { get; set; }
}
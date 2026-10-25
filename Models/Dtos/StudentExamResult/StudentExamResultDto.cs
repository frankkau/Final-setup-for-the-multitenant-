namespace CouncilRevenueCollection.Models.Dtos.StudentExamResult;

public class StudentExamResultDto
{
    public Guid StudentExamResultId { get; set; }

    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public Guid ExamId { get; set; }

    public string ExamName { get; set; } = string.Empty;

    public Guid ExamSubjectId { get; set; }

    public Guid SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    public decimal MarksObtained { get; set; }

    public decimal? GradePoint { get; set; }

    public string? Grade { get; set; }

    public string? Remarks { get; set; }

    public bool IsPublished { get; set; }
}
namespace CouncilRevenueCollection.Models.Dtos.StudentExamResult;

public class StudentExamResultListDto
{
    public Guid StudentExamResultId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string ExamName { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public decimal MarksObtained { get; set; }

    public string? Grade { get; set; }

    public bool IsPublished { get; set; }
}
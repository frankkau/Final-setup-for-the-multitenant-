namespace CouncilRevenueCollection.Models.Dtos.StudentExamResult;

public class CreateStudentExamResultDto
{
    public Guid StudentId { get; set; }

    public Guid ExamId { get; set; }

    public Guid ExamSubjectId { get; set; }

    public decimal MarksObtained { get; set; }

    public decimal? GradePoint { get; set; }

    public string? Grade { get; set; }

    public string? Remarks { get; set; }

    public bool IsPublished { get; set; }
}
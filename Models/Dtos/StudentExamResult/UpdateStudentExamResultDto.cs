namespace CouncilRevenueCollection.Models.Dtos.StudentExamResult;

public class UpdateStudentExamResultDto
{
    public decimal MarksObtained { get; set; }

    public decimal? GradePoint { get; set; }

    public string? Grade { get; set; }

    public string? Remarks { get; set; }

    public bool IsPublished { get; set; }
}
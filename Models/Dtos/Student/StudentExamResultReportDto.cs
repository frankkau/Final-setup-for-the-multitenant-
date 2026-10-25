namespace CouncilRevenueCollection.Models.Dtos.Student;

public class StudentExamResultReportDto
{
    public Guid ExamId { get; set; }

    public string ExamName { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public List<StudentSubjectResultDto> Subjects { get; set; }
        = new();
}
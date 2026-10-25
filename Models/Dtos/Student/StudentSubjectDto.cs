namespace CouncilRevenueCollection.Models.Dtos.Student;


public class StudentSubjectDto
{
    public Guid SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    public int PeriodsPerWeek { get; set; }

    public bool IsOptional { get; set; }
}

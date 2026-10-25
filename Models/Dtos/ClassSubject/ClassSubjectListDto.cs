namespace CouncilRevenueCollection.Models.Dtos.ClassSubject;


public class ClassSubjectListDto
{
    public Guid ClassSubjectId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;


    public string? TeacherName { get; set; }


    public int PeriodsPerWeek { get; set; }

    public bool IsOptional { get; set; }

    public bool IsActive { get; set; }
}
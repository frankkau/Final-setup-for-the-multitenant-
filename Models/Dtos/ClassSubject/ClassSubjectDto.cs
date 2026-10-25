namespace CouncilRevenueCollection.Application.Models.Dtos.ClassSubject;
public class ClassSubjectDto
{
    public Guid ClassSubjectId { get; set; }

    public Guid ClassId { get; set; }

    public string ClassName { get; set; } = string.Empty;


    public Guid SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;


    public Guid? TeacherId { get; set; }

    public string? TeacherName { get; set; }


    public int PeriodsPerWeek { get; set; }

    public bool IsOptional { get; set; }

    public bool IsActive { get; set; }
}
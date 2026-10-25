namespace CouncilRevenueCollection.Models.Dtos.TeacherSubject;

public class TeacherSubjectDto
{
    public Guid TeacherSubjectId { get; set; }

    public Guid TeacherId { get; set; }

    public string TeacherName { get; set; } = string.Empty;

    public Guid SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
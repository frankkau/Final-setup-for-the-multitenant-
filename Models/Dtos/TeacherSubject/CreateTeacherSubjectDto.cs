namespace CouncilRevenueCollection.Models.Dtos.TeacherSubject;

public class CreateTeacherSubjectDto
{
    public Guid TeacherId { get; set; }

    public Guid SubjectId { get; set; }
}
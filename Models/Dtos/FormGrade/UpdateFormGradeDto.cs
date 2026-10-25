namespace CouncilRevenueCollection.Models.Dtos.FormGrade;

public class UpdateFormGradeDto
{
    public Guid AcademicStructureId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int MinAge { get; set; }

    public int MaxAge { get; set; }

    public int Sequence { get; set; }
}

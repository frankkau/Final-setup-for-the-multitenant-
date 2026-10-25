namespace CouncilRevenueCollection.Models.Dtos.FormGrade;

public class FormGradeListDto
{
    public Guid FormGradeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Sequence { get; set; }

    public int MinAge { get; set; }

    public int MaxAge { get; set; }
    public Guid AcademicStructureId { get; set; }
    public string AcademicStructureName { get; set; } = string.Empty;   
}
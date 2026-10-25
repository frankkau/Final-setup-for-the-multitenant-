

namespace CouncilRevenueCollection.Models.Dtos.FormGrade;

public class FormGradeDto
{
    public Guid FormGradeId { get; set; }

    public string TenantId { get; set; } = string.Empty;

    public Guid AcademicStructureId { get; set; }

    public string AcademicStructureName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int MinAge { get; set; }

    public int MaxAge { get; set; }

    public int Sequence { get; set; }
}

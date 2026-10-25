namespace CouncilRevenueCollection.Models.Dtos.AcademicStructure;

public class AcademicStructureDto
{
    public Guid AcademicStructureId { get; set; }

    public string TenantId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Sequence { get; set; }

    public string Description { get; set; } = string.Empty;
}
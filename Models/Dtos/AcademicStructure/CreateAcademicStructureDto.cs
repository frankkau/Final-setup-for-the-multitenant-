namespace CouncilRevenueCollection.Models.Dtos.AcademicStructure;

public class CreateAcademicStructureDto
{
    // public Guid TenantId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Sequence { get; set; }

    public string Description { get; set; } = string.Empty;
}
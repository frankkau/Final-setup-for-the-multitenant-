namespace CouncilRevenueCollection.Models.Dtos.AcademicStructure;

// namespace SchoolManagement.Application.DTOs.AcademicStructure;

public class AcademicStructureListDto
{
    public Guid AcademicStructureId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Sequence { get; set; }

    public string Description { get; set; } = string.Empty;
}

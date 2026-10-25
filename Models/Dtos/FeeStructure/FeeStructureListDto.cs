namespace CouncilRevenueCollection.Models.Dtos.FeeStructure;

public class FeeStructureListDto
{
    public Guid FeeStructureId { get; set; }

    public Guid ClassId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public bool IsMandatory { get; set; }

    public bool IsActive { get; set; }
}
namespace CouncilRevenueCollection.Models.Dtos.FeeStructure;

public class CreateFeeStructureDto
{
    public Guid ClassId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public bool IsMandatory { get; set; } = true;

    public bool IsActive { get; set; } = true;
}
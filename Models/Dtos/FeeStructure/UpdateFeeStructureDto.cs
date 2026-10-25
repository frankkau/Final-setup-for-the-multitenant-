namespace CouncilRevenueCollection.Models.Dtos.FeeStructure;

public class UpdateFeeStructureDto
{
    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public bool IsMandatory { get; set; }

    public bool IsActive { get; set; }
}
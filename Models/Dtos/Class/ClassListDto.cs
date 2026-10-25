namespace CouncilRevenueCollection.Application.DTOs.Class;

public class ClassListDto
{
    public Guid ClassId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public string FormGradeName { get; set; } = string.Empty;

    public int Capacity { get; set; }
}

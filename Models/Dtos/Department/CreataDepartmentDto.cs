namespace CouncilRevenueCollection.Models.Dtos.Department;

public class CreateDepartmentDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
namespace CouncilRevenueCollection.Models.Dtos.Department;

public class DepartmentDto
{
    public Guid DepartmentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int TeacherCount { get; set; }
}
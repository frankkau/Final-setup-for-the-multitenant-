namespace CouncilRevenueCollection.Models.Dtos.Registrar;

public class UpdateRegistrarDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string EmployeeNo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public Guid? DepartmentId { get; set; }

    public bool Status { get; set; }
}
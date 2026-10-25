namespace CouncilRevenueCollection.Models.Dtos.Registrar;

public class CreateRegistrarDto
{
    public string EmployeeNo { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public Guid? DepartmentId { get; set; }

    public bool Status { get; set; }

    public string Password { get; set; } = string.Empty;
}
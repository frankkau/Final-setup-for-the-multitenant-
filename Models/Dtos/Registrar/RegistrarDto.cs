namespace CouncilRevenueCollection.Models.Dtos.Registrar;

public class RegistrarDto
{
    public Guid RegistrarId { get; set; }

    public string EmployeeNo { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName =>
        $"{FirstName} {LastName}";

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public Guid? DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public bool Status { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
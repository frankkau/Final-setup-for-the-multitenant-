namespace CouncilRevenueCollection.Models.Dtos.Accountant;

public class AccountantDto
{
    public Guid AccountantId { get; set; }

    public string EmployeeNo { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName =>
        $"{FirstName} {LastName}";

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public Guid? DepartmentId { get; set; }

    public bool Status { get; set; }

    public DateTime CreatedAt { get; set; }
}
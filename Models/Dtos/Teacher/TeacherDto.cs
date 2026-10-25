namespace CouncilRevenueCollection.Application.DTOs.Teacher;

public class TeacherDto
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = default!;

    public string EmployeeNo { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Qualification { get; set; } = string.Empty;

    public DateOnly HireDate { get; set; }

    public bool Status { get; set; }

    public Guid? DepartmentId { get; set; }
}
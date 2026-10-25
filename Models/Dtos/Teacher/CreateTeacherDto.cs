using System.ComponentModel.DataAnnotations;

namespace CouncilRevenueCollection.Application.DTOs.Teacher;
public class CreateTeacherDto
{
    public string EmployeeNo { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Qualification { get; set; } = string.Empty;

    public DateOnly HireDate { get; set; }
    public bool Status { get; set; } = true;

    public Guid? DepartmentId { get; set; }

    public string Password { get; set; } = string.Empty;
}
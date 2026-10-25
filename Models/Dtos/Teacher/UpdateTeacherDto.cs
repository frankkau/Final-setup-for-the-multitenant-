using System.ComponentModel.DataAnnotations;
namespace CouncilRevenueCollection.Application.DTOs.Teacher;

public class UpdateTeacherDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    
    public Guid? DepartmentId { get; set; }


    [Required]
    [MaxLength(200)]
    public string Qualification { get; set; } = string.Empty;

    [Required]
    public DateOnly HireDate { get; set; }

    public bool Status { get; set; }
}
namespace CouncilRevenueCollection.Models.Dtos.Student;

using System.ComponentModel.DataAnnotations;


public class UpdateStudentDto
{

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;



    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;



    [Required]
    public DateOnly DateOfBirth { get; set; }



    [Required]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;



    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;



    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;



    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;



    [Required]
    [MaxLength(150)]
    public string GuardianName { get; set; } = string.Empty;



    [Required]
    [MaxLength(20)]
    public string GuardianPhone { get; set; } = string.Empty;


    public bool Status { get; set; }
}

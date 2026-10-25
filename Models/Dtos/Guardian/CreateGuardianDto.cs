namespace CouncilRevenueCollection.Models.Dtos.Guardian;

using System.ComponentModel.DataAnnotations;
public class CreateGuardianDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;


    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;


    [Required]
    [MaxLength(50)]
    public string Relationship { get; set; } = string.Empty;


    [Required]
    [Phone]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;


    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;


    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;


    public bool Status { get; set; } = true;
}
namespace CouncilRevenueCollection.Models.Dtos.StudentGuardian;

using System.ComponentModel.DataAnnotations;


public class CreateStudentGuardianDto
{
    [Required]
    public Guid StudentId { get; set; }


    [Required]
    public Guid GuardianId { get; set; }


    public bool IsPrimary { get; set; }


    public bool CanPickup { get; set; }
}
namespace CouncilRevenueCollection.Models.Dtos;

using System.ComponentModel.DataAnnotations;


public class CreateStudentEnrollmentDto
{
    [Required]
    public Guid StudentId { get; set; }


    [Required]
    public Guid ClassId { get; set; }


    [Required]
    public Guid AcademicYearId { get; set; }


    public DateOnly EnrollmentDate { get; set; }
        = DateOnly.FromDateTime(DateTime.UtcNow);


    public bool Status { get; set; } = true;
}
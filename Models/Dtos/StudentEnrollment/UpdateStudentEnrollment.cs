namespace CouncilRevenueCollection.Models.Dtos.StudentEnrollment;

using System.ComponentModel.DataAnnotations;


public class UpdateStudentEnrollmentDto
{
    [Required]
    public Guid ClassId { get; set; }


    [Required]
    public Guid AcademicYearId { get; set; }


    public DateOnly EnrollmentDate { get; set; }


    public bool Status { get; set; }
}

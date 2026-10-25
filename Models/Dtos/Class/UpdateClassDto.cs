using System.ComponentModel.DataAnnotations;

namespace CouncilRevenueCollection.Application.DTOs.Class;

public class UpdateClassDto
{
    [Required]
    public Guid FormGradeId { get; set; }


    [Required]
    public Guid AcademicYearId { get; set; }


    [Required]
    public Guid ClassTeacherId { get; set; }


    [Required]
    [MaxLength(100)]
    public string ClassName { get; set; } = string.Empty;


    [Range(1, 200)]
    public int Capacity { get; set; }
}
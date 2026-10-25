using System.ComponentModel.DataAnnotations;

namespace CouncilRevenueCollection.Models.Dtos.Exam;

public class CreateExamDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; } = true;
}
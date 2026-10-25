using System.ComponentModel.DataAnnotations;

namespace CouncilRevenueCollection.Application.DTOs.ClassSubject;

public class CreateClassSubjectDto
{
    [Required]
    public Guid ClassId { get; set; }

    [Required]
    public Guid SubjectId { get; set; }

    public Guid? TeacherId { get; set; }

    [Range(1, 20)]
    public int PeriodsPerWeek { get; set; }

    public bool IsOptional { get; set; }

    public bool IsActive { get; set; } = true;
}
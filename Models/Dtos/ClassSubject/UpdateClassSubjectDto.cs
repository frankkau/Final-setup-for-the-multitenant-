using System.ComponentModel.DataAnnotations;

namespace CouncilRevenueCollection.Application.Models.Dtos.ClassSubject;

public class UpdateClassSubjectDto
{
    public Guid? TeacherId { get; set; }

    [Range(1, 20)]
    public int PeriodsPerWeek { get; set; }

    public bool IsOptional { get; set; }

    public bool IsActive { get; set; }
}
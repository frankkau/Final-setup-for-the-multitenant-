using System.ComponentModel.DataAnnotations;

namespace CouncilRevenueCollection.Models.Dtos.ExamSubject;

public class UpdateExamSubjectDto
{
    public Guid? TeacherId { get; set; }

    [Required]
    public DateOnly ExamDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Range(1, 1000)]
    public decimal TotalMarks { get; set; } = 100;

    public bool IsActive { get; set; } = true;
}
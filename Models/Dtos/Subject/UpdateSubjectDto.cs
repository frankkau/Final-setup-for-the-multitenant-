using System.ComponentModel.DataAnnotations;

namespace CouncilRevenueCollection.Models.Dtos.Subject;

public class UpdateSubjectDto
{
    [Required]
    [MaxLength(20)]
    public string SubjectCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string SubjectName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public bool IsCompulsory { get; set; }

    public bool IsActive { get; set; }
}
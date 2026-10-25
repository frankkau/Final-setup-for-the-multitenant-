namespace CouncilRevenueCollection.Models.Dtos.Subject;

using System.ComponentModel.DataAnnotations;

public class CreateSubjectDto
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

    public bool IsActive { get; set; } = true;
}
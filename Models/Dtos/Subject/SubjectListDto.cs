namespace CouncilRevenueCollection.Models.Dtos.Subject;

public class SubjectListDto
{
    public Guid SubjectId { get; set; }

    public string SubjectCode { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public bool IsCompulsory { get; set; }

    public bool IsActive { get; set; }
}
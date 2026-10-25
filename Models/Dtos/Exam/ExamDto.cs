namespace CouncilRevenueCollection.Models.Dtos;


public class ExamDto
{
    public Guid ExamId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; }
}
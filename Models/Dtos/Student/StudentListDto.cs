namespace CouncilRevenueCollection.Models.Dtos.Student;


public class StudentListDto
{
    public Guid StudentId { get; set; }

    public string StudentNo { get; set; } = string.Empty;


    public string FullName { get; set; } = string.Empty;


    public string Gender { get; set; } = string.Empty;


    public bool Status { get; set; }
}

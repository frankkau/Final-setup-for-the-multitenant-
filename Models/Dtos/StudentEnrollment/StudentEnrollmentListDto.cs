namespace CouncilRevenueCollection.Models.Dtos.StudentEnrollment;


public class StudentEnrollmentListDto
{
    public Guid StudentEnrollmentId { get; set; }


    public string StudentName { get; set; } = string.Empty;


    public string ClassName { get; set; } = string.Empty;


    public DateOnly EnrollmentDate { get; set; }


    public bool Status { get; set; }
}
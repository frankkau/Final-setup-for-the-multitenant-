namespace CouncilRevenueCollection.Models.Dtos.StudentEnrollment;


public class StudentEnrollmentDto
{
    public Guid StudentEnrollmentId { get; set; }


    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;


    public Guid ClassId { get; set; }

    public string ClassName { get; set; } = string.Empty;


    public Guid AcademicYearId { get; set; }


    public DateOnly EnrollmentDate { get; set; }


    public bool Status { get; set; }
}
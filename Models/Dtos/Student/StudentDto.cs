namespace CouncilRevenueCollection.Models.Dtos.Student;


public class StudentDto
{
    public Guid StudentId { get; set; }

    public string UserId { get; set; } = default!;

    public string StudentNo { get; set; } = string.Empty;

    public string FullName =>
        $"{FirstName} {LastName}";

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string GuardianName { get; set; } = string.Empty;

    public string GuardianPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    
    public bool Status { get; set; }
}
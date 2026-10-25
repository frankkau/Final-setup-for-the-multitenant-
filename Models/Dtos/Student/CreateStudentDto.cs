namespace CouncilRevenueCollection.Models.Dtos;

public class CreateStudentDto
{
    public string StudentNo { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
    public bool Status { get; set; } = true;

    public string Password { get; set; } = string.Empty;
}
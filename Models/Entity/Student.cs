using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Models.Entity;

namespace CouncilRevenueCollection.Domain.Entities;

public class Student : IMustHaveTenant
{
    public Guid StudentId { get; set; }

    public string TenantId { get; set; } = default!;

    // Human-facing identifier
    public string StudentNo { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    // Identity user
    public string UserId { get; set; } = default!;

    public User User { get; set; } = default!;

    public bool Status { get; set; }

    public DateTime CreatedAt { get; set; }


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;


    // Student → Enrollment → Class
    public ICollection<StudentEnrollment> Enrollments { get; set; }
        = new List<StudentEnrollment>();


    // Student → Guardians
    public ICollection<StudentGuardian> StudentGuardians { get; set; }
        = new List<StudentGuardian>();


    // Student → Exam Results
    public ICollection<StudentExamResult> ExamResults { get; set; }
        = new List<StudentExamResult>();
}
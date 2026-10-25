using CouncilRevenueCollection.Common;
using SchoolManagement.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class Teacher : IMustHaveTenant
{
    public Guid TeacherId { get; set; }

    public string TenantId { get; set; } = default!;

    public string EmployeeNo { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Qualification { get; set; } = string.Empty;

    public DateOnly HireDate { get; set; }

    public string UserId { get; set; } = default!;

    public User User { get; set; } = default!;

    public bool Status { get; set; }

    public Guid? DepartmentId { get; set; }

    public Department? Department { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = default!;

    public ICollection<Class> Classes { get; set; }
        = new List<Class>();

    public ICollection<ClassSubject> ClassSubjects { get; set; }
        = new List<ClassSubject>();

    public ICollection<TeacherSubject> TeacherSubjects { get; set; }
        = new List<TeacherSubject>();
}
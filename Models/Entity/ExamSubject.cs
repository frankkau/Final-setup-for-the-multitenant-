using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;
using SchoolManagement.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class ExamSubject : IMustHaveTenant
{
    public Guid ExamSubjectId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid ExamId { get; set; }

    public Guid ClassId { get; set; }

    public Guid SubjectId { get; set; }

    // Optional teacher responsible for this examination paper
    public Guid? TeacherId { get; set; }

    public DateOnly ExamDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public decimal TotalMarks { get; set; } = 100;

    public bool IsActive { get; set; } = true;


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public Exam Exam { get; set; } = default!;

    public Class Class { get; set; } = default!;

    public Subject Subject { get; set; } = default!;

    public Teacher? Teacher { get; set; }
    public ICollection<StudentExamResult> StudentExamResults { get; set; }
    = new List<StudentExamResult>();
}
using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class StudentExamResult : IMustHaveTenant
{
    public Guid StudentExamResultId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid StudentId { get; set; }

    public Guid ExamId { get; set; }

    public Guid ExamSubjectId { get; set; }

    public decimal MarksObtained { get; set; }

    public decimal? GradePoint { get; set; }

    public string? Grade { get; set; }

    public string? Remarks { get; set; }

    public bool IsPublished { get; set; }

    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public Student Student { get; set; } = default!;

    public Exam Exam { get; set; } = default!;

    public ExamSubject ExamSubject { get; set; } = default!;
}
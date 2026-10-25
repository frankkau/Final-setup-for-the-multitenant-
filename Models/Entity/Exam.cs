namespace CouncilRevenueCollection.Models.Entity;

using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Models.Entity;
public class Exam : IMustHaveTenant
{
    public Guid ExamId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid AcademicYearId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; } = true;


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    

    public ICollection<ExamSubject> ExamSubjects { get; set; }
        = new List<ExamSubject>();
    public ICollection<StudentExamResult> StudentExamResults { get; set; }
    = new List<StudentExamResult>();
}
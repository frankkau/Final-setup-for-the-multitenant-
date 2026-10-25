using System.ComponentModel.DataAnnotations.Schema;
using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;
using SchoolManagement.Domain.Entities;




namespace CouncilRevenueCollection.Models.Entity;

public class Class : IMustHaveTenant
{
    public Guid ClassId { get; set; }

    public string TenantId { get; set; } = default!;
    public Guid FormGradeId { get; set; }

    public Guid AcademicYearId { get; set; }

    public Guid ClassTeacherId { get; set; }


    public string ClassName { get; set; } = string.Empty;

    public int Capacity { get; set; }


    // Tenant relationship
    [ForeignKey(nameof(TenantId))]
    public Tenant Tenant { get; set; } = default!;


    // Academic relationship
    public FormGrade FormGrade { get; set; } = default!;


    // Teacher relationship
    public Teacher ClassTeacher { get; set; } = default!;



    // Future relationships
     public ICollection<Student> Students { get; set; }
        = new List<Student>();
    public ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();
    public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    // public ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
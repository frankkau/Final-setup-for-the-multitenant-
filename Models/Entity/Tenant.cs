using CouncilRevenueCollection.Domain.Entities;
using CouncilRevenueCollection.Models.Entity;
using SchoolManagement.Domain.Entities;

namespace CouncilRevenueCollection;

public class Tenant
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Subdomain { get; set; } = default!;

    public string? CustomDomain { get; set; }

    public bool IsActive { get; set; }
     public ICollection<AcademicStructure> AcademicStructures { get; set; } = new List<AcademicStructure>();
         
    

    public ICollection<ClassSubject> ClassSubjects { get; set; }
        = new List<ClassSubject>();
    public ICollection<Student> Students { get; set; }
        = new List<Student>();


    public ICollection<Teacher> Teachers { get; set; }
        = new List<Teacher>();


    public ICollection<Class> Classes { get; set; }
        = new List<Class>();
    public ICollection<Subject> Subjects { get; set; }
        = new List<Subject>();

    public ICollection<StudentEnrollment> StudentEnrollments { get; set; }
        = new List<StudentEnrollment>();
    public ICollection<Notification> Notifications { get; set; }
        = new List<Notification>();
}

public class TenantInfo
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Subdomain { get; set; } = default!;
}
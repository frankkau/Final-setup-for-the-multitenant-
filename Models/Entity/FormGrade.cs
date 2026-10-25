using CouncilRevenueCollection.Common;

namespace SchoolManagement.Domain.Entities;
using CouncilRevenueCollection.Models.Entity;
public class FormGrade : IMustHaveTenant
{
    public Guid FormGradeId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid AcademicStructureId { get; set; }
    public string Name { get; set; } = string.Empty;

    public int MinAge { get; set; }

    public int MaxAge { get; set; }

    public int Sequence { get; set; }

   public AcademicStructure AcademicStructure { get; set; } = default!;
    public ICollection<Class> Classes { get; set; } = new List<Class>();
    
}
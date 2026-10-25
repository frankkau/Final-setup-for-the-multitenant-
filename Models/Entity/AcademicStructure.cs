using System.ComponentModel.DataAnnotations.Schema;
using CouncilRevenueCollection.Common;
using SchoolManagement.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class AcademicStructure: IMustHaveTenant
{
    public Guid AcademicStructureId { get; set; }
    public string TenantId { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    public int Sequence { get; set; }

    public string Description { get; set; } = string.Empty;

    [ForeignKey(nameof(TenantId))]
    public Tenant Tenant { get; set; } = default!;

    

    public ICollection<FormGrade> FormGrades { get; set; } = new List<FormGrade>();

}
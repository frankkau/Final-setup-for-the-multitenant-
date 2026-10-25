namespace CouncilRevenueCollection.Application.DTOs.Class;

public class ClassDto
{
    public Guid ClassId { get; set; }

    public string TenantId { get; set; } = string.Empty;

    public Guid FormGradeId { get; set; }

    public string FormGradeName { get; set; } = string.Empty;


    public Guid AcademicYearId { get; set; }


    public Guid ClassTeacherId { get; set; }


    public string ClassName { get; set; } = string.Empty;


    public int Capacity { get; set; }
}
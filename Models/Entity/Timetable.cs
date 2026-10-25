namespace CouncilRevenueCollection.Models.Entity;

using CouncilRevenueCollection.Common;
using SchoolManagement.Domain.Entities;



public class Timetable : IMustHaveTenant
{
    public Guid TimetableId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid ClassSubjectId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? RoomNumber { get; set; }

    public bool IsActive { get; set; } = true;


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public ClassSubject ClassSubject { get; set; } = default!;
}

using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.StudentAttendance;
using Microsoft.EntityFrameworkCore;
using CouncilRevenueCollection.Common;
namespace CouncilRevenueCollection.Application.Implementation;

public class AttendanceSummaryService : IAttendanceSummaryService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public AttendanceSummaryService(
        ApplicationDbContext context,
        ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }


    public async Task<AttendanceSummaryDto?> GetByStudentAsync(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        var enrollment = await _context.StudentEnrollments
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.EnrollmentDate)
            .Select(x => new
            {
                x.StudentEnrollmentId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (enrollment is null)
            return null;

        return await GetByEnrollmentAsync(
            enrollment.StudentEnrollmentId,
            cancellationToken);
    }


    public async Task<AttendanceSummaryDto?> GetByEnrollmentAsync(
        Guid studentEnrollmentId,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        var enrollment = await _context.StudentEnrollments
            .AsNoTracking()
            .Where(x =>
                x.StudentEnrollmentId == studentEnrollmentId)
            .Select(x => new
            {
                x.StudentEnrollmentId,

                x.StudentId,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,

                ClassName = x.Class.ClassName
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (enrollment is null)
            return null;


        var attendance = await _context.StudentAttendances
            .AsNoTracking()
            .Where(x =>
                x.StudentEnrollmentId ==
                studentEnrollmentId)
            .Select(x => x.Status)
            .ToListAsync(cancellationToken);


        var totalDays = attendance.Count;

        var presentDays = attendance.Count(
            x => x == AttendanceStatus.Present);

        var absentDays = attendance.Count(
            x => x == AttendanceStatus.Absent);

        var lateDays = attendance.Count(
            x => x == AttendanceStatus.Late);


        var attendancePercentage =
            totalDays == 0
                ? 0
                : Math.Round(
                    (decimal)presentDays /
                    totalDays *
                    100,
                    2);


        return new AttendanceSummaryDto
        {
            StudentId = enrollment.StudentId,

            StudentEnrollmentId =
                enrollment.StudentEnrollmentId,

            StudentName =
                enrollment.StudentName,

            ClassName =
                enrollment.ClassName,

            TotalDays = totalDays,

            PresentDays = presentDays,

            AbsentDays = absentDays,

            LateDays = lateDays,

            AttendancePercentage =
                attendancePercentage
        };
    }


    private string GetTenantId()
    {
        var tenantId =
            _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException(
                "No tenant has been resolved.");

        return tenantId;
    }
}

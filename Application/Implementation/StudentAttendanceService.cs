using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.StudentAttendance;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class StudentAttendanceService : IStudentAttendanceService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<StudentAttendanceService> _logger;

    public StudentAttendanceService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<StudentAttendanceService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<StudentAttendanceListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.StudentAttendances
            .AsNoTracking()
            .OrderByDescending(x => x.AttendanceDate)
            .Select(x => new StudentAttendanceListDto
            {
                StudentAttendanceId = x.StudentAttendanceId,

                StudentEnrollmentId = x.StudentEnrollmentId,

                StudentId = x.StudentEnrollment.StudentId,

                StudentName =
                    x.StudentEnrollment.Student.FirstName +
                    " " +
                    x.StudentEnrollment.Student.LastName,

                ClassName =
                    x.StudentEnrollment.Class.ClassName,

                AttendanceDate = x.AttendanceDate,

                Status = x.Status
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<StudentAttendanceDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.StudentAttendances
            .AsNoTracking()
            .Where(x => x.StudentAttendanceId == id)
            .Select(x => new StudentAttendanceDto
            {
                StudentAttendanceId = x.StudentAttendanceId,

                StudentEnrollmentId = x.StudentEnrollmentId,

                StudentId =
                    x.StudentEnrollment.StudentId,

                StudentName =
                    x.StudentEnrollment.Student.FirstName +
                    " " +
                    x.StudentEnrollment.Student.LastName,

                ClassId =
                    x.StudentEnrollment.ClassId,

                ClassName =
                    x.StudentEnrollment.Class.ClassName,

                AttendanceDate =
                    x.AttendanceDate,

                Status =
                    x.Status,

                Remarks =
                    x.Remarks
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<StudentAttendanceDto> CreateAsync(
        CreateStudentAttendanceDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var enrollment = await _context.StudentEnrollments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.StudentEnrollmentId ==
                    dto.StudentEnrollmentId &&
                    x.TenantId == tenantId,
                cancellationToken);


        if (enrollment is null)
            throw new KeyNotFoundException(
                "Student enrollment not found.");


        if (!enrollment.Status)
            throw new InvalidOperationException(
                "The student enrollment is not active.");


        var attendanceExists =
            await _context.StudentAttendances
                .AnyAsync(
                    x =>
                        x.StudentEnrollmentId ==
                        dto.StudentEnrollmentId &&
                        x.AttendanceDate ==
                        dto.AttendanceDate,
                    cancellationToken);


        if (attendanceExists)
            throw new InvalidOperationException(
                "Attendance has already been recorded for this student on this date.");


        var attendance = new StudentAttendance
        {
            StudentAttendanceId = Guid.NewGuid(),

            TenantId = tenantId,

            StudentEnrollmentId =
                dto.StudentEnrollmentId,

            AttendanceDate =
                dto.AttendanceDate,

            Status =
                dto.Status,

            Remarks =
                string.IsNullOrWhiteSpace(dto.Remarks)
                    ? null
                    : dto.Remarks.Trim()
        };


        await _context.StudentAttendances.AddAsync(
            attendance,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Attendance {AttendanceId} created for enrollment {EnrollmentId}",
            attendance.StudentAttendanceId,
            attendance.StudentEnrollmentId);


        return await GetByIdAsync(
                attendance.StudentAttendanceId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load created attendance record.");
    }


    public async Task<StudentAttendanceDto> UpdateAsync(
        Guid id,
        UpdateStudentAttendanceDto dto,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var attendance =
            await _context.StudentAttendances
                .FirstOrDefaultAsync(
                    x =>
                        x.StudentAttendanceId == id,
                    cancellationToken);


        if (attendance is null)
            throw new KeyNotFoundException(
                "Attendance record not found.");


        attendance.Status = dto.Status;

        attendance.Remarks =
            string.IsNullOrWhiteSpace(dto.Remarks)
                ? null
                : dto.Remarks.Trim();


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Attendance {AttendanceId} updated",
            id);


        return await GetByIdAsync(
                id,
                cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load updated attendance record.");
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var attendance =
            await _context.StudentAttendances
                .FirstOrDefaultAsync(
                    x =>
                        x.StudentAttendanceId == id,
                    cancellationToken);


        if (attendance is null)
            throw new KeyNotFoundException(
                "Attendance record not found.");


        _context.StudentAttendances.Remove(
            attendance);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Attendance {AttendanceId} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.StudentAttendances
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.StudentAttendanceId == id,
                cancellationToken);
    }


    public async Task<IReadOnlyList<StudentAttendanceListDto>>
        GetByStudentAsync(
            Guid studentId,
            CancellationToken cancellationToken)
    {
        GetTenantId();


        return await _context.StudentAttendances
            .AsNoTracking()
            .Where(
                x =>
                    x.StudentEnrollment.StudentId ==
                    studentId)
            .OrderByDescending(
                x => x.AttendanceDate)
            .Select(x => new StudentAttendanceListDto
            {
                StudentAttendanceId =
                    x.StudentAttendanceId,

                StudentEnrollmentId =
                    x.StudentEnrollmentId,

                StudentId =
                    x.StudentEnrollment.StudentId,

                StudentName =
                    x.StudentEnrollment.Student.FirstName +
                    " " +
                    x.StudentEnrollment.Student.LastName,

                ClassName =
                    x.StudentEnrollment.Class.ClassName,

                AttendanceDate =
                    x.AttendanceDate,

                Status =
                    x.Status
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<IReadOnlyList<StudentAttendanceListDto>>
        GetByEnrollmentAsync(
            Guid studentEnrollmentId,
            CancellationToken cancellationToken)
    {
        GetTenantId();


        return await _context.StudentAttendances
            .AsNoTracking()
            .Where(
                x =>
                    x.StudentEnrollmentId ==
                    studentEnrollmentId)
            .OrderByDescending(
                x => x.AttendanceDate)
            .Select(x => new StudentAttendanceListDto
            {
                StudentAttendanceId =
                    x.StudentAttendanceId,

                StudentEnrollmentId =
                    x.StudentEnrollmentId,

                StudentId =
                    x.StudentEnrollment.StudentId,

                StudentName =
                    x.StudentEnrollment.Student.FirstName +
                    " " +
                    x.StudentEnrollment.Student.LastName,

                ClassName =
                    x.StudentEnrollment.Class.ClassName,

                AttendanceDate =
                    x.AttendanceDate,

                Status =
                    x.Status
            })
            .ToListAsync(cancellationToken);
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
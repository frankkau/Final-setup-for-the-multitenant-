using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.Timetable;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class TimetableService : ITimetableService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<TimetableService> _logger;

    public TimetableService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<TimetableService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<TimetableListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Timetables
            .AsNoTracking()

            .OrderBy(x => x.ClassSubject.Class.ClassName)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)

            .Select(x => new TimetableListDto
            {
                TimetableId = x.TimetableId,

                ClassSubjectId = x.ClassSubjectId,

                ClassName =
                    x.ClassSubject.Class.ClassName,

                SubjectName =
                    x.ClassSubject.Subject.SubjectName,

                TeacherName =
                    x.ClassSubject.Teacher != null
                        ? x.ClassSubject.Teacher.FirstName
                            + " " +
                          x.ClassSubject.Teacher.LastName
                        : null,

                DayOfWeek = x.DayOfWeek,

                StartTime = x.StartTime,

                EndTime = x.EndTime,

                RoomNumber = x.RoomNumber,

                IsActive = x.IsActive
            })

            .ToListAsync(cancellationToken);
    }


    public async Task<TimetableDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Timetables
            .AsNoTracking()

            .Where(x => x.TimetableId == id)

            .Select(x => new TimetableDto
            {
                TimetableId = x.TimetableId,

                ClassSubjectId = x.ClassSubjectId,

                ClassId =
                    x.ClassSubject.ClassId,

                ClassName =
                    x.ClassSubject.Class.ClassName,

                SubjectId =
                    x.ClassSubject.SubjectId,

                SubjectName =
                    x.ClassSubject.Subject.SubjectName,

                TeacherId =
                    x.ClassSubject.TeacherId,

                TeacherName =
                    x.ClassSubject.Teacher != null
                        ? x.ClassSubject.Teacher.FirstName
                            + " " +
                          x.ClassSubject.Teacher.LastName
                        : null,

                DayOfWeek = x.DayOfWeek,

                StartTime = x.StartTime,

                EndTime = x.EndTime,

                RoomNumber = x.RoomNumber,

                IsActive = x.IsActive
            })

            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<TimetableDto> CreateAsync(
        CreateTimetableDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        if (dto.StartTime >= dto.EndTime)
            throw new InvalidOperationException(
                "Start time must be earlier than end time.");


        var classSubject =
            await _context.ClassSubjects

                .AsNoTracking()

                .Where(x =>
                    x.ClassSubjectId ==
                    dto.ClassSubjectId)

                .Select(x => new
                {
                    x.ClassSubjectId,

                    x.ClassId,

                    x.SubjectId
                })

                .FirstOrDefaultAsync(
                    cancellationToken);


        if (classSubject is null)
            throw new KeyNotFoundException(
                "Class subject not found.");


        var overlapExists =
            await _context.Timetables

                .AnyAsync(x =>
                    x.ClassSubject.ClassId ==
                        classSubject.ClassId &&

                    x.DayOfWeek ==
                        dto.DayOfWeek &&

                    x.StartTime < dto.EndTime &&

                    x.EndTime > dto.StartTime,

                    cancellationToken);


        if (overlapExists)
            throw new InvalidOperationException(
                "The class already has a timetable entry during this time.");


        var timetable = new Timetable
        {
            TimetableId = Guid.NewGuid(),

            TenantId = tenantId,

            ClassSubjectId =
                dto.ClassSubjectId,

            DayOfWeek =
                dto.DayOfWeek,

            StartTime =
                dto.StartTime,

            EndTime =
                dto.EndTime,

            RoomNumber =
                string.IsNullOrWhiteSpace(dto.RoomNumber)
                    ? null
                    : dto.RoomNumber.Trim(),

            IsActive =
                dto.IsActive
        };


        await _context.Timetables.AddAsync(
            timetable,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Timetable {Id} created for ClassSubject {ClassSubjectId}",
            timetable.TimetableId,
            timetable.ClassSubjectId);


        return await GetByIdAsync(
            timetable.TimetableId,
            cancellationToken)

            ?? throw new InvalidOperationException(
                "Failed to load created timetable.");
    }


    public async Task<TimetableDto> UpdateAsync(
        Guid id,
        UpdateTimetableDto dto,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        if (dto.StartTime >= dto.EndTime)
            throw new InvalidOperationException(
                "Start time must be earlier than end time.");


        var timetable =
            await _context.Timetables

                .FirstOrDefaultAsync(
                    x => x.TimetableId == id,
                    cancellationToken);


        if (timetable is null)
            throw new KeyNotFoundException(
                "Timetable not found.");


        var classSubject =
            await _context.ClassSubjects

                .AsNoTracking()

                .Where(x =>
                    x.ClassSubjectId ==
                    dto.ClassSubjectId)

                .Select(x => new
                {
                    x.ClassId
                })

                .FirstOrDefaultAsync(
                    cancellationToken);


        if (classSubject is null)
            throw new KeyNotFoundException(
                "Class subject not found.");


        var overlapExists =
            await _context.Timetables

                .AnyAsync(x =>
                    x.TimetableId != id &&

                    x.ClassSubject.ClassId ==
                        classSubject.ClassId &&

                    x.DayOfWeek ==
                        dto.DayOfWeek &&

                    x.StartTime < dto.EndTime &&

                    x.EndTime > dto.StartTime,

                    cancellationToken);


        if (overlapExists)
            throw new InvalidOperationException(
                "The class already has a timetable entry during this time.");


        timetable.ClassSubjectId =
            dto.ClassSubjectId;

        timetable.DayOfWeek =
            dto.DayOfWeek;

        timetable.StartTime =
            dto.StartTime;

        timetable.EndTime =
            dto.EndTime;

        timetable.RoomNumber =
            string.IsNullOrWhiteSpace(dto.RoomNumber)
                ? null
                : dto.RoomNumber.Trim();

        timetable.IsActive =
            dto.IsActive;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Timetable {Id} updated",
            id);


        return await GetByIdAsync(
            id,
            cancellationToken)

            ?? throw new InvalidOperationException(
                "Failed to load updated timetable.");
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var timetable =
            await _context.Timetables

                .FirstOrDefaultAsync(
                    x => x.TimetableId == id,
                    cancellationToken);


        if (timetable is null)
            throw new KeyNotFoundException(
                "Timetable not found.");


        _context.Timetables.Remove(
            timetable);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Timetable {Id} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Timetables
            .AsNoTracking()
            .AnyAsync(
                x => x.TimetableId == id,
                cancellationToken);
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
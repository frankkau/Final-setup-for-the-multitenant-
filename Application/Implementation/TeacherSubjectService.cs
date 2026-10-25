namespace CouncilRevenueCollection.Application.Implementation;

using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.TeacherSubject;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


public class TeacherSubjectService : ITeacherSubjectService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<TeacherSubjectService> _logger;


    public TeacherSubjectService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<TeacherSubjectService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<TeacherSubjectListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.TeacherSubjects
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Teacher.FirstName)
            .ThenBy(x => x.Teacher.LastName)
            .ThenBy(x => x.Subject.SubjectName)
            .Select(x => new TeacherSubjectListDto
            {
                TeacherSubjectId = x.TeacherSubjectId,

                TeacherId = x.TeacherId,

                TeacherName =
                    x.Teacher.FirstName + " " +
                    x.Teacher.LastName,

                SubjectId = x.SubjectId,

                SubjectName = x.Subject.SubjectName,

                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<TeacherSubjectDto?> GetByIdAsync(
        Guid teacherSubjectId,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.TeacherSubjects
            .AsNoTracking()
            .Where(x =>
                x.TeacherSubjectId == teacherSubjectId &&
                x.TenantId == tenantId)
            .Select(x => new TeacherSubjectDto
            {
                TeacherSubjectId = x.TeacherSubjectId,

                TeacherId = x.TeacherId,

                TeacherName =
                    x.Teacher.FirstName + " " +
                    x.Teacher.LastName,

                SubjectId = x.SubjectId,

                SubjectName = x.Subject.SubjectName,

                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<TeacherSubjectDto> CreateAsync(
        CreateTeacherSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var teacherExists = await _context.Teachers
            .AnyAsync(
                x => x.TeacherId == dto.TeacherId && x.TenantId == tenantId,
                cancellationToken);


        if (!teacherExists)
            throw new KeyNotFoundException(
                "Teacher not found.");


        var subjectExists = await _context.Subjects
            .AnyAsync(
                x => x.SubjectId == dto.SubjectId && x.TenantId == tenantId,
                cancellationToken);


        if (!subjectExists)
            throw new KeyNotFoundException(
                "Subject not found.");


        var alreadyAssigned = await _context.TeacherSubjects
            .AnyAsync(
                x =>
                    x.TeacherId == dto.TeacherId &&
                    x.SubjectId == dto.SubjectId &&
                    x.TenantId == tenantId,
                cancellationToken);


        if (alreadyAssigned)
            throw new InvalidOperationException(
                "This subject is already assigned to this teacher.");


        var teacherSubject = new TeacherSubject
        {
            TeacherSubjectId = Guid.NewGuid(),

            TenantId = tenantId,

            TeacherId = dto.TeacherId,

            SubjectId = dto.SubjectId,

            IsActive = true
        };


        await _context.TeacherSubjects.AddAsync(
            teacherSubject,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Subject {SubjectId} assigned to teacher {TeacherId}",
            dto.SubjectId,
            dto.TeacherId);


        return await GetByIdAsync(
            teacherSubject.TeacherSubjectId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load created teacher subject.");
    }


    public async Task<TeacherSubjectDto> UpdateAsync(
        Guid teacherSubjectId,
        UpdateTeacherSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var teacherSubject = await _context.TeacherSubjects
            .FirstOrDefaultAsync(
                x => x.TeacherSubjectId == teacherSubjectId &&
                     x.TenantId == tenantId,
                cancellationToken);


        if (teacherSubject is null)
            throw new KeyNotFoundException(
                "Teacher subject assignment not found.");


        teacherSubject.IsActive = dto.IsActive;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Teacher subject {Id} updated",
            teacherSubjectId);


        return await GetByIdAsync(
            teacherSubjectId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load updated teacher subject.");
    }


    public async Task DeleteAsync(
        Guid teacherSubjectId,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var teacherSubject = await _context.TeacherSubjects
            .FirstOrDefaultAsync(
                x => x.TeacherSubjectId == teacherSubjectId &&
                     x.TenantId == tenantId,
                cancellationToken);


        if (teacherSubject is null)
            throw new KeyNotFoundException(
                "Teacher subject assignment not found.");


        _context.TeacherSubjects.Remove(teacherSubject);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Teacher subject {Id} deleted",
            teacherSubjectId);
    }


    public async Task<bool> ExistsAsync(
        Guid teacherSubjectId,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.TeacherSubjects
            .AsNoTracking()
            .AnyAsync(
                x => x.TeacherSubjectId == teacherSubjectId &&
                     x.TenantId == tenantId,
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
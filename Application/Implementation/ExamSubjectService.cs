namespace CouncilRevenueCollection.Application.Implementation;

using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.ExamSubject;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


public class ExamSubjectService : IExamSubjectService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<ExamSubjectService> _logger;

    public ExamSubjectService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<ExamSubjectService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<ExamSubjectListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
       var tenantId = GetTenantId();

        return await _context.ExamSubjects
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.ExamDate)
            .ThenBy(x => x.StartTime)
            .ThenBy(x => x.Class.ClassName)
            .ThenBy(x => x.Subject.SubjectName)
            .Select(x => new ExamSubjectListDto
            {
                ExamSubjectId = x.ExamSubjectId,

                ExamName = x.Exam.Name,

                ClassName = x.Class.ClassName,

                SubjectName = x.Subject.SubjectName,

                TeacherName = x.Teacher != null
                    ? x.Teacher.FirstName + " " + x.Teacher.LastName
                    : null,

                ExamDate = x.ExamDate,

                StartTime = x.StartTime,

                EndTime = x.EndTime,

                TotalMarks = x.TotalMarks,

                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<ExamSubjectDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.ExamSubjects
            .AsNoTracking()
            .Where(x => x.ExamSubjectId == id && x.TenantId == tenantId)
            .Select(x => new ExamSubjectDto
            {
                ExamSubjectId = x.ExamSubjectId,

                ExamId = x.ExamId,

                ExamName = x.Exam.Name,

                ClassId = x.ClassId,

                ClassName = x.Class.ClassName,

                SubjectId = x.SubjectId,

                SubjectName = x.Subject.SubjectName,

                TeacherId = x.TeacherId,

                TeacherName = x.Teacher != null
                    ? x.Teacher.FirstName + " " + x.Teacher.LastName
                    : null,

                ExamDate = x.ExamDate,

                StartTime = x.StartTime,

                EndTime = x.EndTime,

                TotalMarks = x.TotalMarks,

                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<ExamSubjectDto> CreateAsync(
        CreateExamSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        if (dto.StartTime >= dto.EndTime)
            throw new InvalidOperationException(
                "Exam start time must be before the end time.");


        // Verify exam
        var examExists = await _context.Exams
            .AnyAsync(
                x => x.ExamId == dto.ExamId,
                cancellationToken);

        if (!examExists)
            throw new KeyNotFoundException(
                "Exam not found.");


        // Verify class
        var classExists = await _context.Classes
            .AnyAsync(
                x => x.ClassId == dto.ClassId,
                cancellationToken);

        if (!classExists)
            throw new KeyNotFoundException(
                "Class not found.");


        // Verify subject
        var subjectExists = await _context.Subjects
            .AnyAsync(
                x => x.SubjectId == dto.SubjectId,
                cancellationToken);

        if (!subjectExists)
            throw new KeyNotFoundException(
                "Subject not found.");


        // Verify subject belongs to class
        var classSubjectExists = await _context.ClassSubjects
            .AnyAsync(
                x =>
                    x.ClassId == dto.ClassId &&
                    x.SubjectId == dto.SubjectId &&
                    x.IsActive,
                cancellationToken);

        if (!classSubjectExists)
            throw new InvalidOperationException(
                "This subject is not assigned to the selected class.");


        // Verify teacher if supplied
        if (dto.TeacherId.HasValue)
        {
            var teacherExists = await _context.Teachers
                .AnyAsync(
                    x => x.TeacherId == dto.TeacherId.Value,
                    cancellationToken);

            if (!teacherExists)
                throw new KeyNotFoundException(
                    "Teacher not found.");
        }


        // Prevent duplicate exam subject
        var duplicateExists = await _context.ExamSubjects
            .AnyAsync(
                x =>
                    x.ExamId == dto.ExamId &&
                    x.ClassId == dto.ClassId &&
                    x.SubjectId == dto.SubjectId,
                cancellationToken);

        if (duplicateExists)
            throw new InvalidOperationException(
                "This subject is already assigned to this class for this exam.");


        // Verify exam date falls within exam period
        var examDates = await _context.Exams
            .Where(x => x.ExamId == dto.ExamId)
            .Select(x => new
            {
                x.StartDate,
                x.EndDate
            })
            .FirstAsync(cancellationToken);


        if (dto.ExamDate < examDates.StartDate ||
            dto.ExamDate > examDates.EndDate)
        {
            throw new InvalidOperationException(
                "Exam subject date must fall within the exam period.");
        }


        var entity = new ExamSubject
        {
            ExamSubjectId = Guid.NewGuid(),

            TenantId = tenantId,

            ExamId = dto.ExamId,

            ClassId = dto.ClassId,

            SubjectId = dto.SubjectId,

            TeacherId = dto.TeacherId,

            ExamDate = dto.ExamDate,

            StartTime = dto.StartTime,

            EndTime = dto.EndTime,

            TotalMarks = dto.TotalMarks,

            IsActive = dto.IsActive
        };


        await _context.ExamSubjects.AddAsync(
            entity,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Exam subject {ExamSubjectId} created for exam {ExamId}",
            entity.ExamSubjectId,
            entity.ExamId);


        return await GetByIdAsync(
            entity.ExamSubjectId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load created exam subject.");
    }


    public async Task<ExamSubjectDto> UpdateAsync(
        Guid id,
        UpdateExamSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        if (dto.StartTime >= dto.EndTime)
            throw new InvalidOperationException(
                "Exam start time must be before the end time.");


        var entity = await _context.ExamSubjects
            .FirstOrDefaultAsync(
                x => x.ExamSubjectId == id && x.TenantId == tenantId,
                cancellationToken);


        if (entity is null)
            throw new KeyNotFoundException(
                "Exam subject not found.");


        // Verify teacher if supplied
        if (dto.TeacherId.HasValue)
        {
            var teacherExists = await _context.Teachers
                .AnyAsync(
                    x => x.TeacherId == dto.TeacherId.Value,
                    cancellationToken);

            if (!teacherExists)
                throw new KeyNotFoundException(
                    "Teacher not found.");
        }


        // Verify exam date is within exam period
        var examDates = await _context.Exams
            .Where(x => x.ExamId == entity.ExamId)
            .Select(x => new
            {
                x.StartDate,
                x.EndDate
            })
            .FirstAsync(cancellationToken);


        if (dto.ExamDate < examDates.StartDate ||
            dto.ExamDate > examDates.EndDate)
        {
            throw new InvalidOperationException(
                "Exam subject date must fall within the exam period.");
        }


        entity.TeacherId = dto.TeacherId;

        entity.ExamDate = dto.ExamDate;

        entity.StartTime = dto.StartTime;

        entity.EndTime = dto.EndTime;

        entity.TotalMarks = dto.TotalMarks;

        entity.IsActive = dto.IsActive;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Exam subject {ExamSubjectId} updated",
            id);


        return await GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load updated exam subject.");
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var entity = await _context.ExamSubjects
            .FirstOrDefaultAsync(
                x => x.ExamSubjectId == id && x.TenantId == tenantId,
                cancellationToken);


        if (entity is null)
            throw new KeyNotFoundException(
                "Exam subject not found.");


        _context.ExamSubjects.Remove(entity);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Exam subject {ExamSubjectId} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.ExamSubjects
            .AsNoTracking()
            .AnyAsync(
                x => x.ExamSubjectId == id && x.TenantId == tenantId,
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
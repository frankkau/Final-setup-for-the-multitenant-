using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.StudentExamResult;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class StudentExamResultService : IStudentExamResultService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<StudentExamResultService> _logger;

    public StudentExamResultService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<StudentExamResultService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<StudentExamResultListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.StudentExamResults
            .AsNoTracking()
            .Select(x => new StudentExamResultListDto
            {
                StudentExamResultId = x.StudentExamResultId,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,

                ExamName = x.Exam.Name,

                SubjectName =
                    x.ExamSubject.Subject.SubjectName,

                MarksObtained = x.MarksObtained,

                Grade = x.Grade,

                IsPublished = x.IsPublished
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<StudentExamResultDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.StudentExamResults
            .AsNoTracking()
            .Where(x => x.StudentExamResultId == id)
            .Select(x => new StudentExamResultDto
            {
                StudentExamResultId =
                    x.StudentExamResultId,

                StudentId =
                    x.StudentId,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,

                ExamId =
                    x.ExamId,

                ExamName =
                    x.Exam.Name,

                ExamSubjectId =
                    x.ExamSubjectId,

                SubjectId =
                    x.ExamSubject.SubjectId,

                SubjectName =
                    x.ExamSubject.Subject.SubjectName,

                MarksObtained =
                    x.MarksObtained,

                GradePoint =
                    x.GradePoint,

                Grade =
                    x.Grade,

                Remarks =
                    x.Remarks,

                IsPublished =
                    x.IsPublished
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<StudentExamResultDto> CreateAsync(
        CreateStudentExamResultDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        // --------------------------------------------------
        // 1. Verify student
        // --------------------------------------------------

        var studentExists = await _context.Students
            .AnyAsync(
                x => x.StudentId == dto.StudentId,
                cancellationToken);

        if (!studentExists)
            throw new KeyNotFoundException(
                "Student not found.");


        // --------------------------------------------------
        // 2. Verify exam
        // --------------------------------------------------

        var exam = await _context.Exams
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ExamId == dto.ExamId,
                cancellationToken);

        if (exam is null)
            throw new KeyNotFoundException(
                "Exam not found.");


        // --------------------------------------------------
        // 3. Verify exam subject belongs to exam
        // --------------------------------------------------

        var examSubject = await _context.ExamSubjects
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.ExamSubjectId == dto.ExamSubjectId &&
                    x.ExamId == dto.ExamId,
                cancellationToken);

        if (examSubject is null)
            throw new KeyNotFoundException(
                "Exam subject does not belong to this exam.");


        // --------------------------------------------------
        // 4. Prevent duplicate result
        // --------------------------------------------------

        var resultExists = await _context.StudentExamResults
            .AnyAsync(
                x =>
                    x.StudentId == dto.StudentId &&
                    x.ExamId == dto.ExamId &&
                    x.ExamSubjectId == dto.ExamSubjectId,
                cancellationToken);

        if (resultExists)
            throw new InvalidOperationException(
                "A result already exists for this student and subject.");


        // --------------------------------------------------
        // 5. Create result
        // --------------------------------------------------

        var result = new StudentExamResult
        {
            StudentExamResultId = Guid.NewGuid(),

            TenantId = tenantId,

            StudentId = dto.StudentId,

            ExamId = dto.ExamId,

            ExamSubjectId = dto.ExamSubjectId,

            MarksObtained = dto.MarksObtained,

            GradePoint = dto.GradePoint,

            Grade = dto.Grade?.Trim(),

            Remarks = dto.Remarks?.Trim(),

            IsPublished = dto.IsPublished
        };


        await _context.StudentExamResults.AddAsync(
            result,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Exam result {Id} created for student {StudentId}",
            result.StudentExamResultId,
            result.StudentId);


        return await GetByIdAsync(
            result.StudentExamResultId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load created student exam result.");
    }


    public async Task<StudentExamResultDto> UpdateAsync(
        Guid id,
        UpdateStudentExamResultDto dto,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var result = await _context.StudentExamResults
            .FirstOrDefaultAsync(
                x => x.StudentExamResultId == id,
                cancellationToken);


        if (result is null)
            throw new KeyNotFoundException(
                "Student exam result not found.");


        result.MarksObtained =
            dto.MarksObtained;

        result.GradePoint =
            dto.GradePoint;

        result.Grade =
            dto.Grade?.Trim();

        result.Remarks =
            dto.Remarks?.Trim();

        result.IsPublished =
            dto.IsPublished;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Exam result {Id} updated",
            id);


        return await GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load updated student exam result.");
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var result = await _context.StudentExamResults
            .FirstOrDefaultAsync(
                x => x.StudentExamResultId == id,
                cancellationToken);


        if (result is null)
            throw new KeyNotFoundException(
                "Student exam result not found.");


        _context.StudentExamResults.Remove(result);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Exam result {Id} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.StudentExamResults
            .AsNoTracking()
            .AnyAsync(
                x => x.StudentExamResultId == id,
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
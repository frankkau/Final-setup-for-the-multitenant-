namespace CouncilRevenueCollection.Application.Implementation;

using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.Exam;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


public class ExamService : IExamService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<ExamService> _logger;

    public ExamService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<ExamService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<ExamListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
       var tenantId = GetTenantId();

        return await _context.Exams
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.StartDate)
            .ThenBy(x => x.Name)
            .Select(x => new ExamListDto
            {
                ExamId = x.ExamId,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<ExamDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Exams
            .AsNoTracking()
            .Where(x => x.ExamId == id && x.TenantId == tenantId)
            .Select(x => new ExamDto
            {
                ExamId = x.ExamId,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<ExamDto> CreateAsync(
        CreateExamDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var name = dto.Name.Trim();

        if (dto.StartDate > dto.EndDate)
            throw new InvalidOperationException(
                "Exam start date cannot be after the end date.");


        var exists = await _context.Exams
            .AnyAsync(
                x => x.Name == name,
                cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "An exam with this name already exists.");


        var exam = new Exam
        {
            ExamId = Guid.NewGuid(),

            TenantId = tenantId,

            Name = name,

            StartDate = dto.StartDate,

            EndDate = dto.EndDate,

            IsActive = dto.IsActive
        };


        await _context.Exams.AddAsync(
            exam,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Exam {ExamId} created",
            exam.ExamId);


        return Map(exam);
    }


    public async Task<ExamDto> UpdateAsync(
        Guid id,
        UpdateExamDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var exam = await _context.Exams
            .FirstOrDefaultAsync(
                x => x.ExamId == id && x.TenantId == tenantId,
                cancellationToken);

        if (exam is null)
            throw new KeyNotFoundException(
                "Exam not found.");


        if (dto.StartDate > dto.EndDate)
            throw new InvalidOperationException(
                "Exam start date cannot be after the end date.");


        var name = dto.Name.Trim();


        var exists = await _context.Exams
            .AnyAsync(
                x =>
                    x.ExamId != id &&
                    x.Name == name,
                cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "An exam with this name already exists.");


        exam.Name = name;

        exam.StartDate = dto.StartDate;

        exam.EndDate = dto.EndDate;

        exam.IsActive = dto.IsActive;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Exam {ExamId} updated",
            id);


        return Map(exam);
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var exam = await _context.Exams
            .FirstOrDefaultAsync(
                x => x.ExamId == id && x.TenantId == tenantId,
                cancellationToken);

        if (exam is null)
            throw new KeyNotFoundException(
                "Exam not found.");


        _context.Exams.Remove(exam);

        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Exam {ExamId} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Exams
            .AsNoTracking()
            .AnyAsync(
                x => x.ExamId == id && x.TenantId == tenantId,
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


    private static ExamDto Map(Exam entity)
    {
        return new ExamDto
        {
            ExamId = entity.ExamId,
            Name = entity.Name,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsActive = entity.IsActive
        };
    }
}
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Application.Interfaces;
using CouncilRevenueCollection.Models.Dtos.Subject;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;


namespace CouncilRevenueCollection.Infrastructure.Services;

public class SubjectService : ISubjectService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<SubjectService> _logger;

    public SubjectService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<SubjectService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }

    public async Task<SubjectDto> CreateAsync(
        CreateSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var exists = await _context.Subjects
            .AnyAsync(x =>
                x.TenantId == tenantId &&
                x.SubjectCode == dto.SubjectCode,
                cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                $"Subject code '{dto.SubjectCode}' already exists.");

        var entity = new Subject
        {
            SubjectId = Guid.NewGuid(),
            TenantId = tenantId,
            SubjectCode = dto.SubjectCode.Trim(),
            SubjectName = dto.SubjectName.Trim(),
            Description = dto.Description.Trim(),
            IsCompulsory = dto.IsCompulsory,
            IsActive = dto.IsActive
        };

        _context.Subjects.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Subject {SubjectCode} created successfully.",
            entity.SubjectCode);

        return MapToDto(entity);
    }

    public async Task<IReadOnlyList<SubjectListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Subjects
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.SubjectName)
            .Select(x => new SubjectListDto
            {
                SubjectId = x.SubjectId,
                SubjectCode = x.SubjectCode,
                SubjectName = x.SubjectName,
                IsCompulsory = x.IsCompulsory,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<SubjectDto?> GetByIdAsync(
        Guid subjectId,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Subjects
            .AsNoTracking()
            .Where(x =>
                x.SubjectId == subjectId &&
                x.TenantId == tenantId)
            .Select(x => new SubjectDto
            {
                SubjectId = x.SubjectId,
                SubjectCode = x.SubjectCode,
                SubjectName = x.SubjectName,
                Description = x.Description,
                IsCompulsory = x.IsCompulsory,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SubjectDto> UpdateAsync(
        Guid subjectId,
        UpdateSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var entity = await _context.Subjects
            .FirstOrDefaultAsync(
                x => x.SubjectId == subjectId &&
                     x.TenantId == tenantId,
                cancellationToken);

        if (entity is null)
            throw new KeyNotFoundException("Subject not found.");

        var duplicate = await _context.Subjects
            .AnyAsync(x =>
                x.SubjectId != subjectId &&
                x.TenantId == tenantId &&
                x.SubjectCode == dto.SubjectCode,
                cancellationToken);

        if (duplicate)
            throw new InvalidOperationException(
                $"Subject code '{dto.SubjectCode}' already exists.");

        entity.SubjectCode = dto.SubjectCode.Trim();
        entity.SubjectName = dto.SubjectName.Trim();
        entity.Description = dto.Description.Trim();
        entity.IsCompulsory = dto.IsCompulsory;
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Subject {SubjectId} updated successfully.",
            subjectId);

        return MapToDto(entity);
    }

    public async Task DeleteAsync(
        Guid subjectId,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var entity = await _context.Subjects
            .FirstOrDefaultAsync(
                x => x.SubjectId == subjectId &&
                     x.TenantId == tenantId,
                cancellationToken);

        if (entity is null)
            throw new KeyNotFoundException("Subject not found.");

        _context.Subjects.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Subject {SubjectId} deleted successfully.",
            subjectId);
    }

    public async Task<bool> ExistsAsync(
        Guid subjectId,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Subjects
            .AsNoTracking()
            .AnyAsync(
                x => x.SubjectId == subjectId &&
                     x.TenantId == tenantId,
                cancellationToken);
    }

    private string GetTenantId()
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException(
                "No tenant has been resolved.");

        return tenantId;
    }

    private static SubjectDto MapToDto(
        Subject entity)
    {
        return new SubjectDto
        {
            SubjectId = entity.SubjectId,
            SubjectCode = entity.SubjectCode,
            SubjectName = entity.SubjectName,
            Description = entity.Description,
            IsCompulsory = entity.IsCompulsory,
            IsActive = entity.IsActive
        };
    }
}
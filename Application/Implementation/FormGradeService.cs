using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.FormGrade;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagement.Domain.Entities;

namespace CouncilRevenueCollection.Infrastructure.Services;

public class FormGradeService : IFormGradeService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<FormGradeService> _logger;

    public FormGradeService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<FormGradeService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }

    public async Task<FormGradeDto> CreateAsync(
        CreateFormGradeDto dto,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

        var academicStructure = await _context.AcademicStructures
            .FirstOrDefaultAsync(
                x => x.AcademicStructureId == dto.AcademicStructureId,
                cancellationToken);

        if (academicStructure is null)
            throw new KeyNotFoundException("Academic Structure not found.");

        var name = dto.Name.Trim();

        var exists = await _context.FormGrades
            .AnyAsync(x =>
                x.AcademicStructureId == dto.AcademicStructureId &&
                x.Name == name,
                cancellationToken);

        if (exists)
            throw new InvalidOperationException($"Form/Grade '{name}' already exists.");

        var entity = new FormGrade
        {
            FormGradeId = Guid.NewGuid(),
            TenantId = tenantId,
            AcademicStructureId = dto.AcademicStructureId,
            Name = name,
            MinAge = dto.MinAge,
            MaxAge = dto.MaxAge,
            Sequence = dto.Sequence
        };

        await _context.FormGrades.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "FormGrade {Name} created for tenant {TenantId}",
            entity.Name,
            tenantId);

        return Map(entity, academicStructure.Name);
    }

    public async Task<FormGradeDto?> GetByIdAsync(
        Guid formGradeId,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

        return await _context.FormGrades
            .AsNoTracking()
            .Include(x => x.AcademicStructure)
            .Where(x => x.FormGradeId == formGradeId && x.TenantId == tenantId)
            .Select(x => new FormGradeDto
            {
                FormGradeId = x.FormGradeId,
                TenantId = x.TenantId,
                AcademicStructureId = x.AcademicStructureId,
                AcademicStructureName = x.AcademicStructure.Name,
                Name = x.Name,
                MinAge = x.MinAge,
                MaxAge = x.MaxAge,
                Sequence = x.Sequence
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FormGradeListDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

        return await _context.FormGrades
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Sequence)
            .Select(x => new FormGradeListDto
            {
                FormGradeId = x.FormGradeId,
                Name = x.Name,
                MinAge = x.MinAge,
                MaxAge = x.MaxAge,
                Sequence = x.Sequence
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<FormGradeDto> UpdateAsync(
        Guid formGradeId,
        UpdateFormGradeDto dto,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

        var entity = await _context.FormGrades
            .Include(x => x.AcademicStructure)
            .FirstOrDefaultAsync(
                x => x.FormGradeId == formGradeId && x.TenantId == tenantId,
                cancellationToken);

        if (entity is null)
            throw new KeyNotFoundException("Form/Grade not found.");

        var academicStructure = await _context.AcademicStructures
            .FirstOrDefaultAsync(
                x => x.AcademicStructureId == dto.AcademicStructureId,
                cancellationToken);

        if (academicStructure is null)
            throw new KeyNotFoundException("Academic Structure not found.");

        var name = dto.Name.Trim();

        var exists = await _context.FormGrades
            .AnyAsync(x =>
                x.FormGradeId != formGradeId &&
                x.AcademicStructureId == dto.AcademicStructureId &&
                x.Name == name,
                cancellationToken);

        if (exists)
            throw new InvalidOperationException($"Form/Grade '{name}' already exists.");

        entity.AcademicStructureId = dto.AcademicStructureId;
        entity.Name = name;
        entity.MinAge = dto.MinAge;
        entity.MaxAge = dto.MaxAge;
        entity.Sequence = dto.Sequence;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "FormGrade {Id} updated.",
            entity.FormGradeId);

        return Map(entity, academicStructure.Name);
    }

    public async Task DeleteAsync(
        Guid formGradeId,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

        var entity = await _context.FormGrades
            .FirstOrDefaultAsync(
                x => x.FormGradeId == formGradeId && x.TenantId == tenantId,
                cancellationToken);

        if (entity is null)
            throw new KeyNotFoundException("Form/Grade not found.");

        var hasClasses = await _context.Classes
            .AnyAsync(
                x => x.FormGradeId == formGradeId && x.TenantId == tenantId,
                cancellationToken);

        if (hasClasses)
            throw new InvalidOperationException(
                "Cannot delete this Form/Grade because it is assigned to one or more classes.");

        _context.FormGrades.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "FormGrade {Id} deleted.",
            formGradeId);
    }

    private string GetTenantId()
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException("No tenant has been resolved.");

        return tenantId;
    }

    private static FormGradeDto Map(
    FormGrade entity,
    string academicStructureName)
{
    return new FormGradeDto
    {
        FormGradeId = entity.FormGradeId,
        TenantId = entity.TenantId,
        AcademicStructureId = entity.AcademicStructureId,
        AcademicStructureName = academicStructureName,
        Name = entity.Name,
        MinAge = entity.MinAge,
        MaxAge = entity.MaxAge,
        Sequence = entity.Sequence
    };
}
}
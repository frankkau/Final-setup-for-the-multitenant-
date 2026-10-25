using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.FeeStructure;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class FeeStructureService : IFeeStructureService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<FeeStructureService> _logger;


    public FeeStructureService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<FeeStructureService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<FeeStructureListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
       var tenantId = GetTenantId();

        return await _context.FeeStructures
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Class.ClassName)
            .ThenBy(x => x.Name)
            .Select(x => new FeeStructureListDto
            {
                FeeStructureId = x.FeeStructureId,

                ClassId = x.ClassId,

                ClassName = x.Class.ClassName,

                Name = x.Name,

                Amount = x.Amount,

                IsMandatory = x.IsMandatory,

                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<FeeStructureDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.FeeStructures
            .AsNoTracking()
            .Where(x => x.FeeStructureId == id && x.TenantId == tenantId)
            .Select(x => new FeeStructureDto
            {
                FeeStructureId = x.FeeStructureId,

                ClassId = x.ClassId,

                ClassName = x.Class.ClassName,

                Name = x.Name,

                Amount = x.Amount,

                IsMandatory = x.IsMandatory,

                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<FeeStructureDto> CreateAsync(
        CreateFeeStructureDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        if (dto.Amount <= 0)
            throw new ArgumentException(
                "Fee amount must be greater than zero.");


        var classExists = await _context.Classes
            .AnyAsync(
                x => x.ClassId == dto.ClassId,
                cancellationToken);


        if (!classExists)
            throw new KeyNotFoundException(
                "Class not found.");


        var name = dto.Name.Trim();


        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Fee name is required.");


        var duplicate = await _context.FeeStructures
            .AnyAsync(
                x =>
                    x.ClassId == dto.ClassId &&
                    x.Name == name,
                cancellationToken);


        if (duplicate)
            throw new InvalidOperationException(
                "A fee with this name already exists for this class.");


        var entity = new FeeStructure
        {
            FeeStructureId = Guid.NewGuid(),

            TenantId = tenantId,

            ClassId = dto.ClassId,

            Name = name,

            Amount = dto.Amount,

            IsMandatory = dto.IsMandatory,

            IsActive = dto.IsActive
        };


        await _context.FeeStructures.AddAsync(
            entity,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Fee structure {Id} created for class {ClassId}.",
            entity.FeeStructureId,
            entity.ClassId);


        return Map(entity);
    }


    public async Task<FeeStructureDto> UpdateAsync(
        Guid id,
        UpdateFeeStructureDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        if (dto.Amount <= 0)
            throw new ArgumentException(
                "Fee amount must be greater than zero.");


        var entity = await _context.FeeStructures
            .FirstOrDefaultAsync(
                x => x.FeeStructureId == id && x.TenantId == tenantId,
                cancellationToken);


        if (entity is null)
            throw new KeyNotFoundException(
                "Fee structure not found.");


        var name = dto.Name.Trim();


        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Fee name is required.");


        var duplicate = await _context.FeeStructures
            .AnyAsync(
                x =>
                    x.FeeStructureId != id &&
                    x.ClassId == entity.ClassId &&
                    x.Name == name,
                cancellationToken);


        if (duplicate)
            throw new InvalidOperationException(
                "A fee with this name already exists for this class.");


        entity.Name = name;

        entity.Amount = dto.Amount;

        entity.IsMandatory = dto.IsMandatory;

        entity.IsActive = dto.IsActive;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Fee structure {Id} updated.",
            id);


        return Map(entity);
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var entity = await _context.FeeStructures
            .FirstOrDefaultAsync(
                x => x.FeeStructureId == id && x.TenantId == tenantId,
                cancellationToken);


        if (entity is null)
            throw new KeyNotFoundException(
                "Fee structure not found.");


        var hasPayments = await _context.FeePayments
            .AnyAsync(
                x => x.FeeStructureId == id,
                cancellationToken);


        if (hasPayments)
            throw new InvalidOperationException(
                "This fee structure cannot be deleted because payments already exist.");


        _context.FeeStructures.Remove(entity);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Fee structure {Id} deleted.",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.FeeStructures
            .AnyAsync(
                x => x.FeeStructureId == id && x.TenantId == tenantId,
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


    private static FeeStructureDto Map(
        FeeStructure entity)
    {
        return new FeeStructureDto
        {
            FeeStructureId = entity.FeeStructureId,

            ClassId = entity.ClassId,

            ClassName = entity.Class?.ClassName ?? string.Empty,

            Name = entity.Name,

            Amount = entity.Amount,

            IsMandatory = entity.IsMandatory,

            IsActive = entity.IsActive
        };
    }
}
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.AcademicStructure;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Infrastructure.Services;

public class AcademicStructureService : IAcademicStructureService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<AcademicStructureService> _logger;


    public AcademicStructureService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<AcademicStructureService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<AcademicStructureDto> CreateAsync(
        CreateAcademicStructureDto dto,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();


        var name = dto.Name.Trim();


        var exists = await _context.AcademicStructures
            .AnyAsync(
                x => x.Name == name,
                cancellationToken);


        if (exists)
            throw new InvalidOperationException(
                $"Academic structure '{name}' already exists.");


        var entity = new AcademicStructure
        {
            AcademicStructureId = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            Sequence = dto.Sequence,
            Description = dto.Description.Trim()
        };


        await _context.AcademicStructures.AddAsync(
            entity,
            cancellationToken);


        await _context.SaveChangesAsync(cancellationToken);


        _logger.LogInformation(
            "Academic structure {Name} created for tenant {TenantId}",
            entity.Name,
            tenantId);


        return Map(entity);
    }



    public async Task<AcademicStructureDto?> GetByIdAsync(
        Guid academicStructureId,
        CancellationToken cancellationToken = default)
    {

        var tenantId = GetTenantId();


        return await _context.AcademicStructures
            .AsNoTracking()
            .Where(x => x.AcademicStructureId == academicStructureId && x.TenantId == tenantId)
            .Select(x => new AcademicStructureDto
            {
                AcademicStructureId = x.AcademicStructureId,
                TenantId = x.TenantId,
                Name = x.Name,
                Sequence = x.Sequence,
                Description = x.Description
            })
            .FirstOrDefaultAsync(cancellationToken);
    }



    public async Task<IReadOnlyList<AcademicStructureListDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {

        var tenantId = GetTenantId();


        return await _context.AcademicStructures
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Sequence)
            .Select(x => new AcademicStructureListDto
            {
                AcademicStructureId = x.AcademicStructureId,
                Name = x.Name,
                Sequence = x.Sequence,
                Description = x.Description
            })
            .ToListAsync(cancellationToken);
    }




    public async Task<AcademicStructureDto> UpdateAsync(
        Guid academicStructureId,
        UpdateAcademicStructureDto dto,
        CancellationToken cancellationToken = default)
    {

        var tenantId = GetTenantId();


        var entity = await _context.AcademicStructures
            .FirstOrDefaultAsync(
                x => x.AcademicStructureId == academicStructureId && x.TenantId == tenantId,
                cancellationToken);



        if (entity is null)
            throw new KeyNotFoundException(
                "Academic structure not found.");



        var name = dto.Name.Trim();


        var exists = await _context.AcademicStructures
            .AnyAsync(
                x => x.AcademicStructureId != academicStructureId &&
                     x.Name == name,
                cancellationToken);



        if (exists)
            throw new InvalidOperationException(
                $"Academic structure '{name}' already exists.");



        entity.Name = name;
        entity.Sequence = dto.Sequence;
        entity.Description = dto.Description.Trim();



        await _context.SaveChangesAsync(cancellationToken);



        _logger.LogInformation(
            "Academic structure {Id} updated",
            academicStructureId);



        return Map(entity);
    }




    public async Task DeleteAsync(
        Guid academicStructureId,
        CancellationToken cancellationToken = default)
    {

        var tenantId = GetTenantId();



        var entity = await _context.AcademicStructures
            .FirstOrDefaultAsync(
                x => x.AcademicStructureId == academicStructureId && x.TenantId == tenantId ,
                cancellationToken);



        if (entity is null)
            throw new KeyNotFoundException(
                "Academic structure not found.");



        _context.AcademicStructures.Remove(entity);



        await _context.SaveChangesAsync(cancellationToken);



        _logger.LogInformation(
            "Academic structure {Id} deleted",
            academicStructureId);
    }





    private string GetTenantId()
    {
        var tenantId = _tenantService.GetCurrentTenantId();


        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException(
                "No tenant has been resolved.");


        return tenantId;
    }




    private static AcademicStructureDto Map(
        AcademicStructure entity)
    {
        return new AcademicStructureDto
        {
            AcademicStructureId = entity.AcademicStructureId,
            TenantId = entity.TenantId,
            Name = entity.Name,
            Sequence = entity.Sequence,
            Description = entity.Description
        };
    }
}
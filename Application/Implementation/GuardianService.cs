namespace CouncilRevenueCollection.Application.Implementation;

using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models;
using CouncilRevenueCollection.Models.Dtos.Guardian;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
public class GuardianService : IGuardianService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<GuardianService> _logger;



    public GuardianService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<GuardianService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }



    public async Task<GuardianDto> CreateAsync(
        CreateGuardianDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();



        var guardian = new Guardian
        {
            GuardianId = Guid.NewGuid(),

            TenantId = tenantId,

            FirstName = dto.FirstName,

            LastName = dto.LastName,

            Relationship = dto.Relationship,

            Phone = dto.Phone,

            Email = dto.Email,

            Address = dto.Address,

            Status = dto.Status
        };


        await _context.Guardians.AddAsync(
            guardian,
            cancellationToken);



        await _context.SaveChangesAsync(
            cancellationToken);



        return Map(guardian);
    }





    public async Task<GuardianDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.Guardians
            .AsNoTracking()
            .Where(x => x.GuardianId == id &&  x.TenantId == tenantId)
            .Select(x => new GuardianDto
            {
                GuardianId = x.GuardianId,

                FirstName = x.FirstName,

                LastName = x.LastName,

                Relationship = x.Relationship,

                Phone = x.Phone,

                Email = x.Email,

                Status = x.Status
            })
            .FirstOrDefaultAsync(cancellationToken);
    }





    public async Task<IEnumerable<GuardianDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.Guardians
            .AsNoTracking()
            .Where(x=>x.TenantId == tenantId)
            .Select(x => new GuardianDto
            {
                GuardianId = x.GuardianId,

                FirstName = x.FirstName,

                LastName = x.LastName,

                Relationship = x.Relationship,

                Phone = x.Phone,

                Email = x.Email,

                Status = x.Status
            })
            .ToListAsync(cancellationToken);
    }





    public async Task<GuardianDto> UpdateAsync(
        Guid id,
        UpdateGuardianDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var guardian = await _context.Guardians
            .FirstOrDefaultAsync(
                x => x.GuardianId == id  &&  x.TenantId == tenantId,
                cancellationToken);



        if (guardian is null)
            throw new KeyNotFoundException(
                "Guardian not found.");



        guardian.FirstName = dto.FirstName;

        guardian.LastName = dto.LastName;

        guardian.Relationship = dto.Relationship;

        guardian.Phone = dto.Phone;

        guardian.Email = dto.Email;

        guardian.Address = dto.Address;

        guardian.Status = dto.Status;



        await _context.SaveChangesAsync(
            cancellationToken);



        return Map(guardian);
    }





    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
       var tenantId = GetTenantId();


        var guardian = await _context.Guardians
            .FirstOrDefaultAsync(
                x => x.GuardianId == id  &&  x.TenantId == tenantId,
                cancellationToken);



        if (guardian is null)
            throw new KeyNotFoundException();



        _context.Guardians.Remove(guardian);


        await _context.SaveChangesAsync(
            cancellationToken);
    }





    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        return await _context.Guardians
            .AnyAsync(
                x => x.GuardianId == id  &&  x.TenantId == tenantId,
                cancellationToken);
    }




    private string GetTenantId()
    {
        var tenantId = _tenantService.GetCurrentTenantId();


        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException(
                "Tenant not resolved.");


        return tenantId;
    }




    private static GuardianDto Map(Guardian entity)
    {
        return new GuardianDto
        {
            GuardianId = entity.GuardianId,

            FirstName = entity.FirstName,

            LastName = entity.LastName,

            Relationship = entity.Relationship,

            Phone = entity.Phone,

            Email = entity.Email,

            Status = entity.Status
        };
    }
}
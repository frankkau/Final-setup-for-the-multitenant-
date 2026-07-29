
using CouncilRevenueCollection;
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;

public class TenantManagementService : ITenantManagementService
{
    private readonly ApplicationDbContext _db;

    public TenantManagementService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TenantResponseDto>> GetAllAsync()
    {
        return await _db.Tenants
            .AsNoTracking()
                .Select(x => new TenantResponseDto(
                    x.Id,
                    x.Name,
                    x.Subdomain,
                    x.CustomDomain ?? string.Empty,
                    x.IsActive))
                .ToListAsync();
        }

        public async Task<TenantResponseDto?> GetByIdAsync(string id)
        {
            return await _db.Tenants
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new TenantResponseDto(
                    x.Id,
                    x.Name,
                    x.Subdomain,
                    x.CustomDomain ?? string.Empty,
                    x.IsActive))
                .FirstOrDefaultAsync();
        }

        public async Task<TenantResponseDto> CreateAsync(CreateTenantDto dto)
        {
            if (await _db.Tenants.AnyAsync(x => x.Subdomain == dto.Subdomain))
                    throw new Exception("Subdomain already exists.");

        var tenant = new Tenant
        {
            Id = dto.Id.Trim().ToLower(),
            Name = dto.Name,
            Subdomain = dto.Subdomain.Trim().ToLower(),
            CustomDomain = dto.CustomDomain?.Trim().ToLower(),
            IsActive = true
        };

        _db.Tenants.Add(tenant);

        await _db.SaveChangesAsync();

        return new TenantResponseDto(
            tenant.Id,
            tenant.Name,
            tenant.Subdomain,
            tenant.CustomDomain ?? string.Empty,
            tenant.IsActive);
    }

    public async Task<TenantResponseDto> UpdateAsync(string id, UpdateTenantDto dto)
    {
        var tenant = await _db.Tenants.FindAsync(id);

        if (tenant == null)
            throw new Exception("Tenant not found.");

        tenant.Name = dto.Name;
        tenant.Subdomain = dto.Subdomain.Trim().ToLower();
        tenant.CustomDomain = dto.CustomDomain?.Trim().ToLower();
        tenant.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();

        return new TenantResponseDto(
            tenant.Id,
            tenant.Name,
            tenant.Subdomain,
            tenant.CustomDomain ?? string.Empty,
            tenant.IsActive);
    }

    public async Task DeleteAsync(string id)
    {
        var tenant = await _db.Tenants.FindAsync(id);

        if (tenant == null)
            throw new Exception("Tenant not found.");

        _db.Tenants.Remove(tenant);

        await _db.SaveChangesAsync();
    }

    public Task<TenantResponseDto?> GetByDomainAsync(string domain)
    {
        return _db.Tenants
            .AsNoTracking()
            .Where(x => x.CustomDomain == domain && x.IsActive)
                .Select(x => new TenantResponseDto(
                x.Id,
                x.Name,
                x.Subdomain,
                x.CustomDomain ?? string.Empty,
                x.IsActive))
            .FirstOrDefaultAsync();
    }

    Task ITenantManagementService.UpdateAsync(string id, UpdateTenantDto dto)
    {
        return UpdateAsync(id, dto);
    }

}
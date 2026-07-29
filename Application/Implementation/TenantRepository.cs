using CouncilRevenueCollection.Application.InterfaceClass;
using Microsoft.EntityFrameworkCore;

namespace CouncilRevenueCollection.Application.Implementation;

public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _db;

    public TenantRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Tenant?> GetBySubdomainAsync(string subdomain)
    {
        return await _db.Tenants
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Subdomain == subdomain &&
                x.IsActive);
    }
}
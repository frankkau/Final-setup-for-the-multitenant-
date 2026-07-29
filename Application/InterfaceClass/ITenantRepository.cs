namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface ITenantRepository
{
    Task<Tenant?> GetBySubdomainAsync(string subdomain);
}

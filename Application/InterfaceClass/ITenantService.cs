namespace CouncilRevenueCollection.Application.InterfaceClass;

// namespace CouncilRevenueCollection.Application.IserviceInterface;

public interface ITenantService
{
    string? GetCurrentTenantId();

    void SetCurrentTenant(string tenantId);
}
using CouncilRevenueCollection.Application.InterfaceClass;
using Microsoft.AspNetCore.Http;

namespace CouncilRevenueCollection.Application.Implementation;

public class TenantService : ITenantService
{
    private const string TenantKey = "TenantId";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentTenantId()
    {
        var tenant = _httpContextAccessor.HttpContext?.Items[TenantKey]?.ToString();

        Console.WriteLine($"TenantService.GetCurrentTenantId() = {tenant}");

        return tenant;
    }

    public void SetCurrentTenant(string tenantId)
    {
        Console.WriteLine($"TenantService.SetCurrentTenant({tenantId})");

        _httpContextAccessor.HttpContext!.Items[TenantKey] = tenantId;
    }
}
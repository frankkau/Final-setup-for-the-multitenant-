using CouncilRevenueCollection;
using CouncilRevenueCollection.Application.InterfaceClass;
using Microsoft.EntityFrameworkCore;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IServiceScopeFactory scopeFactory,
        ITenantService tenantService)
    {
        Tenant? tenant = null;

        if (context.Request.Headers.TryGetValue("X-Tenant", out var value))
        {
            var subdomain = value.FirstOrDefault();

            using var scope = scopeFactory.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            tenant = await db.Tenants
                .AsNoTracking()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x =>
                    x.Subdomain == subdomain &&
                    x.IsActive);
        }

        if (tenant == null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Tenant not found.");
            return;
        }

        tenantService.SetCurrentTenant(tenant.Id);

        Console.WriteLine($"Tenant = {tenantService.GetCurrentTenantId()}");

        await _next(context);
    }
}
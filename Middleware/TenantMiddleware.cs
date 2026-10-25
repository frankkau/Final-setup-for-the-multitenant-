using CouncilRevenueCollection;
using CouncilRevenueCollection.Application.InterfaceClass;
using Microsoft.EntityFrameworkCore;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    // Routes that do NOT require an existing tenant
    private static readonly string[] TenantFreePaths =
    {
        "/api/management/tenants",
       
    };

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IServiceScopeFactory scopeFactory,
        ITenantService tenantService)
    {
        // -------------------------------------------------
        // Tenant-free routes
        // -------------------------------------------------

        if (IsTenantFreeRoute(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // -------------------------------------------------
        // Resolve tenant
        // -------------------------------------------------

        if (!context.Request.Headers.TryGetValue(
                "X-Tenant",
                out var value))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(new
            {
                message = "Tenant is required."
            });

            return;
        }

        var subdomain = value.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(subdomain))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(new
            {
                message = "Tenant is required."
            });

            return;
        }

        using var scope = scopeFactory.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var tenant = await db.Tenants
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x =>
                    x.Subdomain == subdomain &&
                    x.IsActive);

        if (tenant == null)
        {
            context.Response.StatusCode =
                StatusCodes.Status404NotFound;

            await context.Response.WriteAsJsonAsync(new
            {
                message = "Tenant not found."
            });

            return;
        }

        tenantService.SetCurrentTenant(tenant.Id);

        Console.WriteLine(
            $"Tenant = {tenantService.GetCurrentTenantId()}");

        await _next(context);
    }

    private static bool IsTenantFreeRoute(PathString path)
    {
        var requestPath = path.Value;

        if (string.IsNullOrWhiteSpace(requestPath))
            return false;

        return TenantFreePaths.Any(
            x => requestPath.Equals(
                x,
                StringComparison.OrdinalIgnoreCase));
    }
}
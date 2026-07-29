using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CouncilRevenueCollection.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        await context.Database.MigrateAsync();

        await SeedTenants(context);

        await SeedTenantAsync(context, userManager, roleManager,
            "tenant-a", "Council A", "council-a");

        await SeedTenantAsync(context, userManager, roleManager,
            "tenant-b", "Council B", "council-b");
    }

    private static async Task SeedTenants(ApplicationDbContext context)
    {
        if (await context.Tenants.IgnoreQueryFilters().AnyAsync())
            return;

        context.Tenants.AddRange(
            new Tenant
            {
                Id = "tenant-a",
                Name = "Council A",
                Subdomain = "council-a",
                CustomDomain = null,
                IsActive = true
            },
            new Tenant
            {
                Id = "tenant-b",
                Name = "Council B",
                Subdomain = "council-b",
                CustomDomain = null,
                IsActive = true
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedTenantAsync(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<ApplicationRole> roleManager,
        string tenantId,
        string councilName,
        string prefix)
    {
        await SeedRoles(context, tenantId);

        await SeedUser(context, userManager, tenantId, prefix, 1, "TenantAdmin");
        await SeedUser(context, userManager, tenantId, prefix, 2, "RevenueManager");
        await SeedUser(context, userManager, tenantId, prefix, 3, "RevenueOfficer");
        await SeedUser(context, userManager, tenantId, prefix, 4, "Cashier");
        await SeedUser(context, userManager, tenantId, prefix, 5, "Auditor");
    }

   private static async Task SeedRoles(
    ApplicationDbContext context,
    string tenantId)
{
    string[] roles =
    {
        "TenantAdmin",
        "RevenueManager",
        "RevenueOfficer",
        "Cashier",
        "Auditor"
    };

    foreach (var roleName in roles)
    {
        var normalized = roleName.ToUpperInvariant();

        var exists = await context.Roles
            .IgnoreQueryFilters()
            .AnyAsync(r =>
                r.TenantId == tenantId &&
                r.NormalizedName == normalized);

        if (exists)
            continue;

        context.Roles.Add(new ApplicationRole
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = tenantId,
            Name = roleName,
            NormalizedName = normalized,
            Description = roleName,
            IsSystemRole = true,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        });
    }

    await context.SaveChangesAsync();
}

private static async Task SeedUser(
    ApplicationDbContext context,
    UserManager<User> userManager,
    string tenantId,
    string prefix,
    int number,
    string roleName)
{
    var email = $"user{number}@{prefix}.com";

    var user = await context.Users
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(u =>
            u.TenantId == tenantId &&
            u.Email == email);

    if (user == null)
    {
        user = new User
        {
            Id = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),

            TenantId = tenantId,

            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),

            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),

            EmailConfirmed = true,

            FirstName = $"User{number}",
            LastName = prefix.Replace("-", " ")
        };

        var result = await userManager.CreateAsync(user, "Password@123");

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(
                Environment.NewLine,
                result.Errors.Select(e => e.Description)));
        }
    }

    // Find the tenant-specific role
    var role = await context.Roles
        .IgnoreQueryFilters()
        .SingleOrDefaultAsync(r =>
            r.TenantId == tenantId &&
            r.NormalizedName == roleName.ToUpperInvariant());

    if (role == null)
        throw new Exception($"Role '{roleName}' not found for tenant '{tenantId}'.");

    // Check if the user is already assigned
    var alreadyAssigned = await context.UserRoles
        .AnyAsync(ur =>
            ur.UserId == user.Id &&
            ur.RoleId == role.Id);

    if (!alreadyAssigned)
    {
        context.UserRoles.Add(new IdentityUserRole<string>
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await context.SaveChangesAsync();
    }
}
}   
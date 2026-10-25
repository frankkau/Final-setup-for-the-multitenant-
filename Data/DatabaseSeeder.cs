using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CouncilRevenueCollection.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        var userManager =
            scope.ServiceProvider
                .GetRequiredService<UserManager<User>>();

        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<RoleManager<ApplicationRole>>();

        // -----------------------------------------------------
        // Apply migrations
        // -----------------------------------------------------

        await context.Database.MigrateAsync();

        // -----------------------------------------------------
        // Seed tenants
        // -----------------------------------------------------

        await SeedTenantAsync(
            context,
            userManager,
            roleManager,
            "tenant-a",
            "Council A",
            "council-a");

        await SeedTenantAsync(
            context,
            userManager,
            roleManager,
            "tenant-b",
            "Council B",
            "council-b");
    }

    // =========================================================
    // TENANT
    // =========================================================

    private static async Task SeedTenantAsync(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<ApplicationRole> roleManager,
        string tenantId,
        string tenantName,
        string subdomain)
    {
        // -----------------------------------------------------
        // 1. Create tenant
        // -----------------------------------------------------

        var tenant =
            await context.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    x => x.Id == tenantId);

        if (tenant is null)
        {
            tenant = new Tenant
            {
                Id = tenantId,

                Name = tenantName,

                Subdomain = subdomain,

                CustomDomain = null,

                IsActive = true
            };

            await context.Tenants.AddAsync(tenant);

            await context.SaveChangesAsync();
        }

        // -----------------------------------------------------
        // 2. Create tenant-specific roles
        // -----------------------------------------------------

        await SeedRolesAsync(
            context,
            roleManager,
            tenantId);

        // -----------------------------------------------------
        // 3. Create users
        // -----------------------------------------------------

        await SeedUserAsync(
            context,
            userManager,
            tenantId,
            subdomain,
            1,
            "TenantAdmin");

        await SeedUserAsync(
            context,
            userManager,
            tenantId,
            subdomain,
            2,
            "RevenueManager");

        await SeedUserAsync(
            context,
            userManager,
            tenantId,
            subdomain,
            3,
            "RevenueOfficer");

        await SeedUserAsync(
            context,
            userManager,
            tenantId,
            subdomain,
            4,
            "Cashier");

        await SeedUserAsync(
            context,
            userManager,
            tenantId,
            subdomain,
            5,
            "Auditor");
    }

    // =========================================================
    // ROLES
    // =========================================================

    private static async Task SeedRolesAsync(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager,
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

        foreach (var roleCode in roles)
        {
            /*
             * ASP.NET Identity requires the Identity role name
             * to be globally unique.
             *
             * Therefore:
             *
             * tenant-a:TenantAdmin
             * tenant-b:TenantAdmin
             *
             * are different Identity role names.
             */

            var identityRoleName =
                $"{tenantId}:{roleCode}";

            var normalizedName =
                identityRoleName.ToUpperInvariant();

            var role =
                await context.Roles
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(
                        x =>
                            x.TenantId == tenantId &&
                            x.NormalizedName == normalizedName);

            if (role is not null)
                continue;

            role = new ApplicationRole
            {
                Id = Guid.NewGuid().ToString(),

                TenantId = tenantId,

                // Identity role name
                Name = identityRoleName,

                NormalizedName = normalizedName,

                // Application-level role name
                RoleCode = roleCode,

                Description = roleCode,

                IsSystemRole = true,

                ConcurrencyStamp =
                    Guid.NewGuid().ToString()
            };

            var result =
                await roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errors =
                    string.Join(
                        Environment.NewLine,
                        result.Errors.Select(
                            x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to create role '{roleCode}' " +
                    $"for tenant '{tenantId}': {errors}");
            }
        }
    }

    // =========================================================
    // USERS
    // =========================================================

    private static async Task SeedUserAsync(
        ApplicationDbContext context,
        UserManager<User> userManager,
        string tenantId,
        string prefix,
        int number,
        string roleCode)
    {
        var email =
            $"user{number}@{prefix}.com";

        // -----------------------------------------------------
        // Find user inside tenant
        // -----------------------------------------------------

        var user =
            await context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    x =>
                        x.TenantId == tenantId &&
                        x.Email == email);

        // -----------------------------------------------------
        // Create user
        // -----------------------------------------------------

        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid().ToString(),

                TenantId = tenantId,

                UserName = email,

                Email = email,

                FirstName = $"User{number}",

                LastName =
                    prefix.Replace(
                        "-",
                        " "),

                EmailConfirmed = true
            };

            var result =
                await userManager.CreateAsync(
                    user,
                    "Password@123");

            if (!result.Succeeded)
            {
                var errors =
                    string.Join(
                        Environment.NewLine,
                        result.Errors.Select(
                            x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to create user '{email}': {errors}");
            }
        }

        // -----------------------------------------------------
        // Build tenant-specific Identity role name
        // -----------------------------------------------------

        var identityRoleName =
            $"{tenantId}:{roleCode}";

        var normalizedRoleName =
            identityRoleName.ToUpperInvariant();

        // -----------------------------------------------------
        // Find tenant-specific role
        // -----------------------------------------------------

        var role =
            await context.Roles
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(
                    x =>
                        x.TenantId == tenantId &&
                        x.NormalizedName ==
                            normalizedRoleName);

        if (role is null)
        {
            throw new InvalidOperationException(
                $"Role '{roleCode}' does not exist " +
                $"for tenant '{tenantId}'.");
        }

        // -----------------------------------------------------
        // Check role assignment
        // -----------------------------------------------------

        var alreadyAssigned =
            await context.UserRoles
                .AnyAsync(
                    x =>
                        x.UserId == user.Id &&
                        x.RoleId == role.Id);

        if (alreadyAssigned)
            return;

        // -----------------------------------------------------
        // Assign role
        // -----------------------------------------------------

        var roleResult =
            await userManager.AddToRoleAsync(
                user,
                identityRoleName);

        if (!roleResult.Succeeded)
        {
            var errors =
                string.Join(
                    Environment.NewLine,
                    roleResult.Errors.Select(
                        x => x.Description));

            throw new InvalidOperationException(
                $"Unable to assign role '{roleCode}' " +
                $"to user '{email}': {errors}");
        }
    }
}
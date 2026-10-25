using CouncilRevenueCollection;
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class TenantManagementService : ITenantManagementService
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public TenantManagementService(
        ApplicationDbContext db,
        UserManager<User> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }


    // =========================================================
    // GET ALL
    // =========================================================

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


    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<TenantResponseDto?> GetByIdAsync(
        string id)
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


    // =========================================================
    // CREATE TENANT + FIRST ADMIN
    // =========================================================

    public async Task<TenantResponseDto> CreateAsync(
    CreateTenantDto dto,
    CancellationToken cancellationToken = default)
{
    // -----------------------------------------------------
    // Normalize input
    // -----------------------------------------------------

    var tenantId = dto.Id.Trim().ToLowerInvariant();

    var subdomain = dto.Subdomain.Trim().ToLowerInvariant();

    var email = dto.AdminEmail.Trim().ToLowerInvariant();

    var name = dto.Name.Trim();

    var adminFirstName = dto.AdminFirstName.Trim();

    var adminLastName = dto.AdminLastName.Trim();


    // -----------------------------------------------------
    // Validation
    // -----------------------------------------------------

    if (string.IsNullOrWhiteSpace(tenantId))
        throw new ArgumentException(
            "Tenant ID is required.");

    if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(
            "Tenant name is required.");

    if (string.IsNullOrWhiteSpace(subdomain))
        throw new ArgumentException(
            "Subdomain is required.");

    if (string.IsNullOrWhiteSpace(email))
        throw new ArgumentException(
            "Admin email is required.");

    if (string.IsNullOrWhiteSpace(dto.AdminPassword))
        throw new ArgumentException(
            "Admin password is required.");

    if (string.IsNullOrWhiteSpace(adminFirstName))
        throw new ArgumentException(
            "Admin first name is required.");

    if (string.IsNullOrWhiteSpace(adminLastName))
        throw new ArgumentException(
            "Admin last name is required.");


    // -----------------------------------------------------
    // Check tenant uniqueness
    // -----------------------------------------------------

    var tenantExists = await _db.Tenants
        .IgnoreQueryFilters()
        .AnyAsync(
            x => x.Id == tenantId,
            cancellationToken);

    if (tenantExists)
        throw new InvalidOperationException(
            "Tenant ID already exists.");


    var subdomainExists = await _db.Tenants
        .IgnoreQueryFilters()
        .AnyAsync(
            x => x.Subdomain == subdomain,
            cancellationToken);

    if (subdomainExists)
        throw new InvalidOperationException(
            "Subdomain already exists.");


    // -----------------------------------------------------
    // Begin transaction
    // -----------------------------------------------------

    await using var transaction =
        await _db.Database.BeginTransactionAsync(
            cancellationToken);

    try
    {
        // =================================================
        // 1. CREATE TENANT
        // =================================================

        var tenant = new Tenant
        {
            Id = tenantId,

            Name = name,

            Subdomain = subdomain,

            CustomDomain = string.IsNullOrWhiteSpace(
                dto.CustomDomain)
                ? null
                : dto.CustomDomain
                    .Trim()
                    .ToLowerInvariant(),

            IsActive = true
        };

        await _db.Tenants.AddAsync(
            tenant,
            cancellationToken);

        await _db.SaveChangesAsync(
            cancellationToken);


        // =================================================
        // 2. CREATE TENANT-SPECIFIC ADMIN ROLE
        // =================================================

        const string roleCode = "Admin";

        /*
         * Identity requires role names to be unique.
         *
         * Therefore:
         *
         * tenant-a → tenant-a:Admin
         * tenant-b → tenant-b:Admin
         *
         * RoleCode remains simply "Admin".
         */

        var identityRoleName = roleCode;

        var normalizedRoleName =
            identityRoleName.ToUpperInvariant();


        var adminRole = await _db.Roles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x =>
                    x.TenantId == tenant.Id &&
                    x.RoleCode == roleCode,
                cancellationToken);


        if (adminRole is null)
        {
            adminRole = new ApplicationRole
            {
                Id = Guid.NewGuid().ToString(),

                TenantId = tenant.Id,

                Name = identityRoleName,

                NormalizedName = normalizedRoleName,

                RoleCode = roleCode,

                Description =
                    "Administrator role for tenant",

                IsSystemRole = true,

                ConcurrencyStamp =
                    Guid.NewGuid().ToString()
            };

            var roleResult =
                await _roleManager.CreateAsync(
                    adminRole);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to create Admin role: {errors}");
            }
        }


        // =================================================
        // 3. CHECK ADMIN EMAIL
        // =================================================

        var emailExists = await _userManager.Users
            .IgnoreQueryFilters()
            .AnyAsync(
                x =>
                    x.TenantId == tenant.Id &&
                    x.Email == email,
                cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Admin email already exists for this tenant.");
        }


        // =================================================
        // 4. CREATE ADMIN USER
        // =================================================

        var adminUser = new User
        {
            Id = Guid.NewGuid().ToString(),

            TenantId = tenant.Id,

            UserName = email,

            NormalizedUserName =
                email.ToUpperInvariant(),

            Email = email,

            NormalizedEmail =
                email.ToUpperInvariant(),

            FirstName = adminFirstName,

            LastName = adminLastName,

            EmailConfirmed = true,

            SecurityStamp =
                Guid.NewGuid().ToString()
        };


        var userResult =
            await _userManager.CreateAsync(
                adminUser,
                dto.AdminPassword);


        if (!userResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                userResult.Errors.Select(
                    x => x.Description));

            throw new InvalidOperationException(
                $"Unable to create administrator account: {errors}");
        }


        // =================================================
        // 5. ASSIGN ADMIN ROLE
        // =================================================

        var alreadyAssigned =
            await _db.UserRoles
                .AnyAsync(
                    x =>
                        x.UserId == adminUser.Id &&
                        x.RoleId == adminRole.Id,
                    cancellationToken);


        if (!alreadyAssigned)
        {
            _db.UserRoles.Add(
                new IdentityUserRole<string>
                {
                    UserId = adminUser.Id,

                    RoleId = adminRole.Id
                });

            await _db.SaveChangesAsync(
                cancellationToken);
        }


        // =================================================
        // 6. COMMIT TRANSACTION
        // =================================================

        await transaction.CommitAsync(
            cancellationToken);


        // =================================================
        // 7. RETURN RESPONSE
        // =================================================

        return new TenantResponseDto(
            tenant.Id,
            tenant.Name,
            tenant.Subdomain,
            tenant.CustomDomain ?? string.Empty,
            tenant.IsActive);
    }
    catch
    {
        await transaction.RollbackAsync(
            cancellationToken);

        throw;
    }
}

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<TenantResponseDto> UpdateAsync(
        string id,
        UpdateTenantDto dto)
    {
        var tenant =
            await _db.Tenants
                .FirstOrDefaultAsync(
                    x => x.Id == id);


        if (tenant is null)
        {
            throw new KeyNotFoundException(
                "Tenant not found.");
        }


        var subdomain =
            dto.Subdomain.Trim()
                .ToLowerInvariant();


        // Prevent another tenant from using
        // the same subdomain.

        var subdomainExists =
            await _db.Tenants.AnyAsync(
                x =>
                    x.Id != id &&
                    x.Subdomain == subdomain);


        if (subdomainExists)
        {
            throw new InvalidOperationException(
                "Subdomain already exists.");
        }


        tenant.Name =
            dto.Name.Trim();

        tenant.Subdomain =
            subdomain;

        tenant.CustomDomain =
            dto.CustomDomain?
                .Trim()
                .ToLowerInvariant();

        tenant.IsActive =
            dto.IsActive;


        await _db.SaveChangesAsync();


        return new TenantResponseDto(
            tenant.Id,
            tenant.Name,
            tenant.Subdomain,
            tenant.CustomDomain ?? string.Empty,
            tenant.IsActive);
    }


    // =========================================================
    // DELETE
    // =========================================================

    public async Task DeleteAsync(
        string id)
    {
        var tenant =
            await _db.Tenants
                .FirstOrDefaultAsync(
                    x => x.Id == id);


        if (tenant is null)
        {
            throw new KeyNotFoundException(
                "Tenant not found.");
        }


        _db.Tenants.Remove(tenant);

        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<UserResponseDto>> GetUsersByTenantAsync(
    string tenantId,
    CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(tenantId))
        throw new ArgumentException(
            "Tenant ID is required.",
            nameof(tenantId));

    tenantId = tenantId.Trim().ToLowerInvariant();

    // Make sure the tenant exists
    var tenantExists = await _db.Tenants
        .IgnoreQueryFilters()
        .AnyAsync(
            x => x.Id == tenantId,
            cancellationToken);

    if (!tenantExists)
        throw new KeyNotFoundException(
            "Tenant not found.");

    return await _db.Users
        .IgnoreQueryFilters()
        .AsNoTracking()
        .Where(x => x.TenantId == tenantId)
        .OrderBy(x => x.FirstName)
        .ThenBy(x => x.LastName)
        .Select(x => new UserResponseDto(
            x.Id,
            x.FirstName,
            x.LastName,
            x.TenantId,
            x.Email ?? string.Empty,
            new List<string>()))
        .ToListAsync(cancellationToken);
}

    // =========================================================
    // GET BY DOMAIN
    // =========================================================

    public async Task<TenantResponseDto?> GetByDomainAsync(
        string domain)
    {
        var normalizedDomain =
            domain.Trim().ToLowerInvariant();


        return await _db.Tenants
            .AsNoTracking()
            .Where(
                x =>
                    x.CustomDomain == normalizedDomain &&
                    x.IsActive)
            .Select(x => new TenantResponseDto(
                x.Id,
                x.Name,
                x.Subdomain,
                x.CustomDomain ?? string.Empty,
                x.IsActive))
            .FirstOrDefaultAsync();
    }


    // =========================================================
    // EXPLICIT INTERFACE IMPLEMENTATION
    // =========================================================

    Task ITenantManagementService.UpdateAsync(
        string id,
        UpdateTenantDto dto)
    {
        return UpdateAsync(id, dto);
    }
}
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CouncilRevenueCollection.Application.Service;

public class RoleManagementService : IRoleManagementService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly ITenantService _tenantService;

    public RoleManagementService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<User> userManager,
        ITenantService tenantService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _tenantService = tenantService;
    }

   public async Task<IEnumerable<RoleDto>> GetAllAsync()
{
    var tenantId = _tenantService.GetCurrentTenantId();

    return await _roleManager.Roles
        .Where(r => r.TenantId == tenantId)
        .Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name!,
            Description = r.Description,
            TenantId = r.TenantId,
            IsSystemRole = r.IsSystemRole
        })
        .ToListAsync();
}public async Task CreateAsync(CreateRoleDto dto)
{
    var tenantId = _tenantService.GetCurrentTenantId();

    if (string.IsNullOrWhiteSpace(tenantId))
        throw new InvalidOperationException("No tenant was resolved.");

    if (dto == null)
        throw new ArgumentNullException(nameof(dto));

    var roleName = dto.RoleName?.Trim();

    if (string.IsNullOrWhiteSpace(roleName))
        throw new Exception("Role name is required.");

    var normalizedName = _roleManager.NormalizeKey(roleName);

    // Check if the role already exists for this tenant
    var exists = await _roleManager.Roles
        .IgnoreQueryFilters()
        .AnyAsync(r =>
            r.TenantId == tenantId &&
            r.NormalizedName == normalizedName);

    if (exists)
        throw new Exception($"Role '{roleName}' already exists.");

    var role = new ApplicationRole
    {
        Id = Guid.NewGuid().ToString(),
        Name = roleName,
        NormalizedName = normalizedName,

        TenantId = tenantId,

        Description = dto.Description ?? string.Empty,
        IsSystemRole = false,

        ConcurrencyStamp = Guid.NewGuid().ToString()
    };

    IdentityResult result;

    try
    {
        result = await _roleManager.CreateAsync(role);
    }
    catch (Exception ex)
    {
        throw new Exception(
            $"Failed to create role '{roleName}'. {ex.InnerException?.Message ?? ex.Message}",
            ex);
    }

    if (!result.Succeeded)
    {
        var errors = string.Join(
            Environment.NewLine,
            result.Errors.Select(e => $"{e.Code}: {e.Description}"));

        throw new Exception(errors);
    }
}

    public async Task DeleteAsync(string roleName)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new Exception("No tenant was resolved.");

        var normalizedName = _roleManager.NormalizeKey(roleName);

        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r =>
                r.TenantId == tenantId &&
                r.NormalizedName == normalizedName);

        if (role == null)
            throw new Exception("Role not found.");

        if (role.IsSystemRole)
            throw new Exception("System roles cannot be deleted.");

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
    public async Task AssignUserAsync(string userId, string roleName)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new Exception("No tenant was resolved.");

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u =>
                u.Id == userId &&
                u.TenantId == tenantId);

        if (user == null)
            throw new Exception("User not found.");

        var normalizedName = _roleManager.NormalizeKey(roleName);

        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r =>
                r.TenantId == tenantId &&
                r.NormalizedName == normalizedName);

        if (role == null)
            throw new Exception("Role not found.");

        if (await _userManager.IsInRoleAsync(user, role.Name!))
            throw new Exception("User already has this role.");

        var result = await _userManager.AddToRoleAsync(user, role.Name!);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
    public async Task RemoveUserAsync(string userId, string roleName)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new Exception("No tenant was resolved.");

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u =>
                u.Id == userId &&
                u.TenantId == tenantId);

        if (user == null)
            throw new Exception("User not found.");

        var normalizedName = _roleManager.NormalizeKey(roleName);

        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r =>
                r.TenantId == tenantId &&
                r.NormalizedName == normalizedName);

        if (role == null)
            throw new Exception("Role not found.");

        var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new Exception("No tenant was resolved.");

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u =>
                u.Id == userId &&
                u.TenantId == tenantId);

        if (user == null)
            throw new Exception("User not found.");

        return await _userManager.GetRolesAsync(user);
    }
}
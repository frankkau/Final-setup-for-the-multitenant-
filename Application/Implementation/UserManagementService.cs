using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CouncilRevenueCollection.Application.Service;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<User> _userManager;
    private readonly ITenantService _tenantService;

    public UserManagementService(UserManager<User> userManager, ITenantService tenantService)
    {
        _userManager = userManager;
        _tenantService = tenantService;
    }

   public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
{
    var tenantId = _tenantService.GetCurrentTenantId();

    if (string.IsNullOrWhiteSpace(tenantId))
        throw new InvalidOperationException("No tenant has been resolved.");

    var users = await _userManager.Users
        .Where(x => x.TenantId == tenantId)
        .ToListAsync();

    var response = new List<UserResponseDto>();

    foreach (var user in users)
    {
        var roles = await _userManager.GetRolesAsync(user);

        response.Add(new UserResponseDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.TenantId,
            user.Email!,
            roles.ToList()));
    }

    return response;
}
    public async Task<UserResponseDto?> GetByIdAsync(string id)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException("No tenant has been resolved.");

        return await _userManager.Users
            .Where(x => x.TenantId == tenantId && x.Id == id)
            .Select(x => new UserResponseDto(
                x.Id,
                x.FirstName,
                x.LastName,
                x.TenantId,
                x.Email!,
                new List<string>()
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException("No tenant has been resolved.");
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = tenantId,
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));

        return await GetByIdAsync(user.Id)
            ?? throw new Exception("User could not be loaded.");
    }

    public async Task<UserResponseDto> UpdateAsync(string id, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.UserName = dto.Email;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));

        return await GetByIdAsync(id)
            ?? throw new Exception("User could not be loaded.");
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            throw new Exception("User not found.");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
    }

    public async Task ResetPasswordAsync(string userId, string newPassword)
{
    var tenantId = _tenantService.GetCurrentTenantId();

    if (string.IsNullOrWhiteSpace(tenantId))
        throw new InvalidOperationException("No tenant has been resolved.");

    var user = await _userManager.Users
        .Where(x => x.TenantId == tenantId)
        .FirstOrDefaultAsync(x => x.Id == userId);

    if (user == null)
        throw new Exception("User not found.");

    // Remove existing password
    var removeResult = await _userManager.RemovePasswordAsync(user);

    if (!removeResult.Succeeded)
    {
        throw new Exception(string.Join(", ",
            removeResult.Errors.Select(x => x.Description)));
    }

    // Set new password
    var addResult = await _userManager.AddPasswordAsync(user, newPassword);

    if (!addResult.Succeeded)
    {
        throw new Exception(string.Join(", ",
            addResult.Errors.Select(x => x.Description)));
    }
}

  public async Task LockUserAsync(string userId)
{
    var user = await GetTenantUserAsync(userId);

    if (!await _userManager.GetLockoutEnabledAsync(user))
        await _userManager.SetLockoutEnabledAsync(user, true);

    var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

    if (!result.Succeeded)
        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
}
public async Task UnlockUserAsync(string userId)
{
    var user = await GetTenantUserAsync(userId);

    var result = await _userManager.SetLockoutEndDateAsync(user, null);

    if (!result.Succeeded)
        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

    await _userManager.ResetAccessFailedCountAsync(user);
}
private async Task<User> GetTenantUserAsync(string userId)
{
    var tenantId = _tenantService.GetCurrentTenantId();

    if (string.IsNullOrWhiteSpace(tenantId))
        throw new InvalidOperationException("No tenant has been resolved.");

    var user = await _userManager.Users
        .FirstOrDefaultAsync(x => x.Id == userId && x.TenantId == tenantId);

    return user ?? throw new Exception("User not found.");
}
}
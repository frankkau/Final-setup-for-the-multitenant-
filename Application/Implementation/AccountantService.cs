using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.Accountant;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CouncilRevenueCollection.Infrastructure.Services;

public class AccountantService : IAccountantService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AccountantService> _logger;

    private const string AccountantRole = "Accountant";

    public AccountantService(
        ApplicationDbContext context,
        ITenantService tenantService,
        UserManager<User> userManager,
        ILogger<AccountantService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _userManager = userManager;
        _logger = logger;
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<AccountantDto> CreateAsync(
        CreateAccountantDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var employeeNo = dto.EmployeeNo.Trim();
        var firstName = dto.FirstName.Trim();
        var lastName = dto.LastName.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();
        var phone = dto.Phone.Trim();

        if (string.IsNullOrWhiteSpace(employeeNo))
            throw new ArgumentException(
                "Employee number is required.");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException(
                "First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException(
                "Last name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "Email is required.");

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException(
                "Password is required.");

        // -----------------------------------------------------
        // Employee number uniqueness
        // -----------------------------------------------------

        var employeeExists =
            await _context.Accountants
                .AnyAsync(
                    x =>
                        x.TenantId == tenantId &&
                        x.EmployeeNo == employeeNo,
                    cancellationToken);

        if (employeeExists)
        {
            throw new InvalidOperationException(
                "Employee number already exists.");
        }

        // -----------------------------------------------------
        // Email uniqueness in Accountant table
        // -----------------------------------------------------

        var accountantEmailExists =
            await _context.Accountants
                .AnyAsync(
                    x =>
                        x.TenantId == tenantId &&
                        x.Email == email,
                    cancellationToken);

        if (accountantEmailExists)
        {
            throw new InvalidOperationException(
                "Email already belongs to another accountant.");
        }

        // -----------------------------------------------------
        // Identity email uniqueness
        // -----------------------------------------------------

        var userEmailExists =
            await _userManager.Users
                .AnyAsync(
                    x =>
                        x.TenantId == tenantId &&
                        x.Email == email,
                    cancellationToken);

        if (userEmailExists)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        // -----------------------------------------------------
        // Department validation
        // -----------------------------------------------------

        if (dto.DepartmentId.HasValue)
        {
            var departmentExists =
                await _context.Departments
                    .AnyAsync(
                        x =>
                            x.DepartmentId ==
                                dto.DepartmentId.Value &&
                            x.TenantId == tenantId,
                        cancellationToken);

            if (!departmentExists)
            {
                throw new KeyNotFoundException(
                    "Department not found.");
            }
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // -------------------------------------------------
            // 1. Find tenant-specific Accountant role
            // -------------------------------------------------

            var role =
                await _context.Roles
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(
                        x =>
                            x.TenantId == tenantId &&
                            x.NormalizedName ==
                                AccountantRole.ToUpperInvariant(),
                        cancellationToken);

            if (role is null)
            {
                throw new InvalidOperationException(
                    $"The '{AccountantRole}' role has not been " +
                    $"configured for tenant '{tenantId}'.");
            }

            // -------------------------------------------------
            // 2. Create Identity User
            // -------------------------------------------------

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),

                TenantId = tenantId,

                UserName = email,

                Email = email,

                FirstName = firstName,

                LastName = lastName,

                EmailConfirmed = false
            };

            var userResult =
                await _userManager.CreateAsync(
                    user,
                    dto.Password);

            if (!userResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    userResult.Errors.Select(
                        x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to create accountant account: {errors}");
            }

            // -------------------------------------------------
            // 3. Assign Accountant role
            // -------------------------------------------------

            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    AccountantRole);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to assign Accountant role: {errors}");
            }

            // -------------------------------------------------
            // 4. Create Accountant
            // -------------------------------------------------

            var accountant = new Accountant
            {
                AccountantId = Guid.NewGuid(),

                TenantId = tenantId,

                EmployeeNo = employeeNo,

                FirstName = firstName,

                LastName = lastName,

                Email = email,

                Phone = phone,

                UserId = user.Id,

                DepartmentId = dto.DepartmentId,

                Status = dto.Status,

                CreatedAt = DateTime.UtcNow
            };

            await _context.Accountants.AddAsync(
                accountant,
                cancellationToken);

            // -------------------------------------------------
            // 5. Save
            // -------------------------------------------------

            await _context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            _logger.LogInformation(
                "Accountant {AccountantId} created with User {UserId} for tenant {TenantId}",
                accountant.AccountantId,
                user.Id,
                tenantId);

            return Map(accountant);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<AccountantDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Accountants
            .AsNoTracking()
            .Where(
                x =>
                    x.AccountantId == id &&
                    x.TenantId == tenantId)
            .Select(x => new AccountantDto
            {
                AccountantId = x.AccountantId,

                EmployeeNo = x.EmployeeNo,

                FirstName = x.FirstName,

                LastName = x.LastName,

                Email = x.Email,

                Phone = x.Phone,

                UserId = x.UserId,

                DepartmentId = x.DepartmentId,

                Status = x.Status,

                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync(
                cancellationToken);
    }

    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<IReadOnlyList<AccountantDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Accountants
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.EmployeeNo)
            .Select(x => new AccountantDto
            {
                AccountantId = x.AccountantId,

                EmployeeNo = x.EmployeeNo,

                FirstName = x.FirstName,

                LastName = x.LastName,

                Email = x.Email,

                Phone = x.Phone,

                UserId = x.UserId,

                DepartmentId = x.DepartmentId,

                Status = x.Status,

                CreatedAt = x.CreatedAt
            })
            .ToListAsync(
                cancellationToken);
    }

    // =========================================================
    // GET BY EMPLOYEE NUMBER
    // =========================================================

    public async Task<AccountantDto?> GetByEmployeeNoAsync(
        string employeeNo,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var normalizedEmployeeNo =
            employeeNo.Trim();

        if (string.IsNullOrWhiteSpace(normalizedEmployeeNo))
            return null;

        return await _context.Accountants
            .AsNoTracking()
            .Where(
                x =>
                    x.TenantId == tenantId &&
                    x.EmployeeNo == normalizedEmployeeNo)
            .Select(x => new AccountantDto
            {
                AccountantId = x.AccountantId,

                EmployeeNo = x.EmployeeNo,

                FirstName = x.FirstName,

                LastName = x.LastName,

                Email = x.Email,

                Phone = x.Phone,

                UserId = x.UserId,

                DepartmentId = x.DepartmentId,

                Status = x.Status,

                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync(
                cancellationToken);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<AccountantDto> UpdateAsync(
        Guid id,
        UpdateAccountantDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var accountant =
            await _context.Accountants
                .FirstOrDefaultAsync(
                    x =>
                        x.AccountantId == id &&
                        x.TenantId == tenantId,
                    cancellationToken);

        if (accountant is null)
        {
            throw new KeyNotFoundException(
                "Accountant not found.");
        }

        var employeeNo =
            dto.EmployeeNo.Trim();

        var firstName =
            dto.FirstName.Trim();

        var lastName =
            dto.LastName.Trim();

        var email =
            dto.Email.Trim().ToLowerInvariant();

        var phone =
            dto.Phone.Trim();

        // -----------------------------------------------------
        // Employee number uniqueness
        // -----------------------------------------------------

        var employeeExists =
            await _context.Accountants
                .AnyAsync(
                    x =>
                        x.AccountantId != id &&
                        x.TenantId == tenantId &&
                        x.EmployeeNo == employeeNo,
                    cancellationToken);

        if (employeeExists)
        {
            throw new InvalidOperationException(
                "Employee number already exists.");
        }

        // -----------------------------------------------------
        // Email uniqueness
        // -----------------------------------------------------

        var emailExists =
            await _context.Accountants
                .AnyAsync(
                    x =>
                        x.AccountantId != id &&
                        x.TenantId == tenantId &&
                        x.Email == email,
                    cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Email already belongs to another accountant.");
        }

        // -----------------------------------------------------
        // Department validation
        // -----------------------------------------------------

        if (dto.DepartmentId.HasValue)
        {
            var departmentExists =
                await _context.Departments
                    .AnyAsync(
                        x =>
                            x.DepartmentId ==
                                dto.DepartmentId.Value &&
                            x.TenantId == tenantId,
                        cancellationToken);

            if (!departmentExists)
            {
                throw new KeyNotFoundException(
                    "Department not found.");
            }
        }

        // -----------------------------------------------------
        // Find Identity User
        // -----------------------------------------------------

        var user =
            await _userManager.FindByIdAsync(
                accountant.UserId);

        if (user is null)
        {
            throw new InvalidOperationException(
                "Accountant's user account could not be found.");
        }

        // -----------------------------------------------------
        // Check Identity email
        // -----------------------------------------------------

        var identityEmailExists =
            await _userManager.Users
                .AnyAsync(
                    x =>
                        x.TenantId == tenantId &&
                        x.Id != accountant.UserId &&
                        x.Email == email,
                    cancellationToken);

        if (identityEmailExists)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        // -----------------------------------------------------
        // Update Accountant
        // -----------------------------------------------------

        accountant.EmployeeNo = employeeNo;

        accountant.FirstName = firstName;

        accountant.LastName = lastName;

        accountant.Email = email;

        accountant.Phone = phone;

        accountant.DepartmentId = dto.DepartmentId;

        accountant.Status = dto.Status;

        // -----------------------------------------------------
        // Update Identity User
        // -----------------------------------------------------

        user.FirstName = firstName;

        user.LastName = lastName;

        user.Email = email;

        user.UserName = email;

        var userResult =
            await _userManager.UpdateAsync(user);

        if (!userResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                userResult.Errors.Select(
                    x => x.Description));

            throw new InvalidOperationException(
                $"Unable to update accountant account: {errors}");
        }

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Accountant {AccountantId} updated",
            id);

        return Map(accountant);
    }

    // =========================================================
    // DELETE
    // =========================================================

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var accountant =
            await _context.Accountants
                .FirstOrDefaultAsync(
                    x =>
                        x.AccountantId == id &&
                        x.TenantId == tenantId,
                    cancellationToken);

        if (accountant is null)
        {
            throw new KeyNotFoundException(
                "Accountant not found.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // -------------------------------------------------
            // Find Identity User
            // -------------------------------------------------

            var user =
                await _userManager.FindByIdAsync(
                    accountant.UserId);

            // -------------------------------------------------
            // Delete Accountant
            // -------------------------------------------------

            _context.Accountants.Remove(accountant);

            await _context.SaveChangesAsync(
                cancellationToken);

            // -------------------------------------------------
            // Delete Identity User
            // -------------------------------------------------

            if (user is not null)
            {
                var userResult =
                    await _userManager.DeleteAsync(user);

                if (!userResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        userResult.Errors.Select(
                            x => x.Description));

                    throw new InvalidOperationException(
                        $"Unable to delete accountant account: {errors}");
                }
            }

            await transaction.CommitAsync(
                cancellationToken);

            _logger.LogInformation(
                "Accountant {AccountantId} and User {UserId} deleted",
                id,
                accountant.UserId);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    // =========================================================
    // TENANT
    // =========================================================

    private string GetTenantId()
    {
        var tenantId =
            _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new InvalidOperationException(
                "No tenant resolved.");
        }

        return tenantId;
    }

    // =========================================================
    // MAPPING
    // =========================================================

    private static AccountantDto Map(
        Accountant entity)
    {
        return new AccountantDto
        {
            AccountantId = entity.AccountantId,

            EmployeeNo = entity.EmployeeNo,

            FirstName = entity.FirstName,

            LastName = entity.LastName,

            Email = entity.Email,

            Phone = entity.Phone,

            UserId = entity.UserId,

            DepartmentId = entity.DepartmentId,

            Status = entity.Status,

            CreatedAt = entity.CreatedAt
        };
    }
}

using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.Registrar;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Infrastructure.Services;

public class RegistrarService : IRegistrarService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RegistrarService> _logger;

    private const string RegistrarRole = "Registrar";

    public RegistrarService(
        ApplicationDbContext context,
        ITenantService tenantService,
        UserManager<User> userManager,
        ILogger<RegistrarService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _userManager = userManager;
        _logger = logger;
    }

    // =========================================================
    // CREATE
    // =========================================================

    public async Task<RegistrarDto> CreateAsync(
        CreateRegistrarDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var employeeNo = dto.EmployeeNo.Trim();
        var firstName = dto.FirstName.Trim();
        var lastName = dto.LastName.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();
        var phone = dto.Phone.Trim();

        if (string.IsNullOrWhiteSpace(employeeNo))
            throw new ArgumentException("Employee number is required.");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Password is required.");

        // -----------------------------------------------------
        // Employee number uniqueness
        // -----------------------------------------------------

        var employeeExists = await _context.Registrars
            .AnyAsync(
                x =>
                    x.TenantId == tenantId &&
                    x.EmployeeNo == employeeNo,
                cancellationToken);

        if (employeeExists)
            throw new InvalidOperationException(
                "Employee number already exists.");

        // -----------------------------------------------------
        // Department validation
        // -----------------------------------------------------

        if (dto.DepartmentId.HasValue)
        {
            var departmentExists = await _context.Departments
                .AnyAsync(
                    x =>
                        x.DepartmentId == dto.DepartmentId.Value &&
                        x.TenantId == tenantId,
                    cancellationToken);

            if (!departmentExists)
                throw new KeyNotFoundException(
                    "Department not found.");
        }

        // -----------------------------------------------------
        // Registrar email uniqueness
        // -----------------------------------------------------

        var registrarEmailExists = await _context.Registrars
            .AnyAsync(
                x =>
                    x.TenantId == tenantId &&
                    x.Email == email,
                cancellationToken);

        if (registrarEmailExists)
            throw new InvalidOperationException(
                "Email already belongs to another registrar.");

        // -----------------------------------------------------
        // Identity email uniqueness within tenant
        // -----------------------------------------------------

        var identityEmailExists = await _userManager.Users
            .AnyAsync(
                x =>
                    x.TenantId == tenantId &&
                    x.Email == email,
                cancellationToken);

        if (identityEmailExists)
            throw new InvalidOperationException(
                "A user with this email already exists.");

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // -------------------------------------------------
            // 1. Verify Registrar role
            // -------------------------------------------------

            var role = await _context.Roles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    x =>
                        x.TenantId == tenantId &&
                        x.NormalizedName ==
                            RegistrarRole.ToUpperInvariant(),
                    cancellationToken);

            if (role is null)
            {
                throw new InvalidOperationException(
                    $"The '{RegistrarRole}' role has not been configured " +
                    $"for tenant '{tenantId}'.");
            }

            // -------------------------------------------------
            // 2. Create Identity user
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

            var userResult = await _userManager.CreateAsync(
                user,
                dto.Password);

            if (!userResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to create registrar account: " +
                    FormatIdentityErrors(userResult.Errors));
            }

            // -------------------------------------------------
            // 3. Assign Registrar role
            // -------------------------------------------------

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                RegistrarRole);

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to assign Registrar role: " +
                    FormatIdentityErrors(roleResult.Errors));
            }

            // -------------------------------------------------
            // 4. Create Registrar
            // -------------------------------------------------

            var registrar = new Registrar
            {
                RegistrarId = Guid.NewGuid(),

                TenantId = tenantId,

                EmployeeNo = employeeNo,

                FirstName = firstName,

                LastName = lastName,

                Email = email,

                Phone = phone,

                DepartmentId = dto.DepartmentId,

                Status = dto.Status,

                UserId = user.Id,

                CreatedAt = DateTime.UtcNow
            };

            await _context.Registrars.AddAsync(
                registrar,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            _logger.LogInformation(
                "Registrar {RegistrarId} created with User {UserId} for Tenant {TenantId}",
                registrar.RegistrarId,
                user.Id,
                tenantId);

            return Map(registrar);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<IReadOnlyList<RegistrarDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Registrars
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new RegistrarDto
            {
                RegistrarId = x.RegistrarId,
                UserId = x.UserId,
                EmployeeNo = x.EmployeeNo,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                DepartmentId = x.DepartmentId,
                DepartmentName = x.Department != null
                    ? x.Department.Name
                    : string.Empty,
                Status = x.Status,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<RegistrarDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Registrars
            .AsNoTracking()
            .Where(
                x =>
                    x.TenantId == tenantId &&
                    x.RegistrarId == id)
            .Select(x => new RegistrarDto
            {
                RegistrarId = x.RegistrarId,
                UserId = x.UserId,
                EmployeeNo = x.EmployeeNo,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                DepartmentId = x.DepartmentId,
                DepartmentName = x.Department != null
                    ? x.Department.Name
                    : string.Empty,
                Status = x.Status,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<RegistrarDto> UpdateAsync(
        Guid id,
        UpdateRegistrarDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var registrar = await _context.Registrars
            .FirstOrDefaultAsync(
                x =>
                    x.RegistrarId == id &&
                    x.TenantId == tenantId,
                cancellationToken);

        if (registrar is null)
            throw new KeyNotFoundException(
                "Registrar not found.");

        var employeeNo = dto.EmployeeNo.Trim();
        var firstName = dto.FirstName.Trim();
        var lastName = dto.LastName.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();
        var phone = dto.Phone.Trim();

        // -----------------------------------------------------
        // Employee number uniqueness
        // -----------------------------------------------------

        var employeeExists = await _context.Registrars
            .AnyAsync(
                x =>
                    x.RegistrarId != id &&
                    x.TenantId == tenantId &&
                    x.EmployeeNo == employeeNo,
                cancellationToken);

        if (employeeExists)
            throw new InvalidOperationException(
                "Employee number already exists.");

        // -----------------------------------------------------
        // Department validation
        // -----------------------------------------------------

        if (dto.DepartmentId.HasValue)
        {
            var departmentExists = await _context.Departments
                .AnyAsync(
                    x =>
                        x.DepartmentId == dto.DepartmentId.Value &&
                        x.TenantId == tenantId,
                    cancellationToken);

            if (!departmentExists)
                throw new KeyNotFoundException(
                    "Department not found.");
        }

        // -----------------------------------------------------
        // Email uniqueness
        // -----------------------------------------------------

        var emailExists = await _context.Registrars
            .AnyAsync(
                x =>
                    x.RegistrarId != id &&
                    x.TenantId == tenantId &&
                    x.Email == email,
                cancellationToken);

        if (emailExists)
            throw new InvalidOperationException(
                "Email already belongs to another registrar.");

        // -----------------------------------------------------
        // Load Identity user
        // -----------------------------------------------------

        var user = await _userManager.Users
            .FirstOrDefaultAsync(
                x =>
                    x.Id == registrar.UserId &&
                    x.TenantId == tenantId,
                cancellationToken);

        if (user is null)
            throw new InvalidOperationException(
                "Registrar's user account could not be found.");

        // -----------------------------------------------------
        // Update registrar
        // -----------------------------------------------------

        registrar.EmployeeNo = employeeNo;
        registrar.FirstName = firstName;
        registrar.LastName = lastName;
        registrar.Email = email;
        registrar.Phone = phone;
        registrar.DepartmentId = dto.DepartmentId;
        registrar.Status = dto.Status;

        // -----------------------------------------------------
        // Update Identity user
        // -----------------------------------------------------

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        user.UserName = email;

        var userResult = await _userManager.UpdateAsync(user);

        if (!userResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Unable to update registrar account: " +
                FormatIdentityErrors(userResult.Errors));
        }

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Registrar {RegistrarId} updated for Tenant {TenantId}",
            id,
            tenantId);

        return Map(registrar);
    }

    // =========================================================
    // GET ALL WITH FILTERS
    // =========================================================
public async Task<RegistrarDto?> GetByEmployeeNoAsync(
    string employeeNo,
    CancellationToken cancellationToken)
{
    var tenantId = GetTenantId();

    var normalizedEmployeeNo = employeeNo.Trim();

    return await _context.Registrars
        .AsNoTracking()
        .Where(x =>
            x.TenantId == tenantId &&
            x.EmployeeNo == normalizedEmployeeNo)
        .Select(x => new RegistrarDto
        {
            RegistrarId = x.RegistrarId,
            UserId = x.UserId,
            EmployeeNo = x.EmployeeNo,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            Phone = x.Phone,
            DepartmentId = x.DepartmentId,
            DepartmentName = x.Department != null
                ? x.Department.Name
                : string.Empty,
            Status = x.Status,
            CreatedAt = x.CreatedAt
        })
        .FirstOrDefaultAsync(cancellationToken);
}
    // =========================================================
    // DELETE
    // =========================================================

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var registrar = await _context.Registrars
            .FirstOrDefaultAsync(
                x =>
                    x.RegistrarId == id &&
                    x.TenantId == tenantId,
                cancellationToken);

        if (registrar is null)
            throw new KeyNotFoundException(
                "Registrar not found.");

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // Find the Identity account belonging to this tenant.
            var user = await _userManager.Users
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == registrar.UserId &&
                        x.TenantId == tenantId,
                    cancellationToken);

            // Delete registrar
            _context.Registrars.Remove(registrar);

            await _context.SaveChangesAsync(
                cancellationToken);

            // Delete Identity account
            if (user is not null)
            {
                var userResult =
                    await _userManager.DeleteAsync(user);

                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Unable to delete registrar account: " +
                        FormatIdentityErrors(userResult.Errors));
                }
            }

            await transaction.CommitAsync(
                cancellationToken);

            _logger.LogInformation(
                "Registrar {RegistrarId} and User {UserId} deleted for Tenant {TenantId}",
                id,
                registrar.UserId,
                tenantId);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    // =========================================================
    // EXISTS
    // =========================================================

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Registrars
            .AnyAsync(
                x =>
                    x.RegistrarId == id &&
                    x.TenantId == tenantId,
                cancellationToken);
    }

    // =========================================================
    // HELPERS
    // =========================================================

    private string GetTenantId()
    {
        var tenantId =
            _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException(
                "No tenant resolved.");

        return tenantId;
    }

    private static string FormatIdentityErrors(
        IEnumerable<IdentityError> errors)
    {
        return string.Join(
            ", ",
            errors.Select(x => x.Description));
    }

    private static RegistrarDto Map(
        Registrar entity)
    {
        return new RegistrarDto
        {
            RegistrarId = entity.RegistrarId,

            UserId = entity.UserId,

            EmployeeNo = entity.EmployeeNo,

            FirstName = entity.FirstName,

            LastName = entity.LastName,

            Email = entity.Email,

            Phone = entity.Phone,

            DepartmentId = entity.DepartmentId,

            DepartmentName =
                entity.Department?.Name
                ?? string.Empty,

            Status = entity.Status,

            CreatedAt = entity.CreatedAt
        };
    }
}

using CouncilRevenueCollection.Application.DTOs.Teacher;
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class TeacherService : ITeacherService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<TeacherService> _logger;

    public TeacherService(
        ApplicationDbContext context,
        ITenantService tenantService,
        UserManager<User> userManager,
        ILogger<TeacherService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _userManager = userManager;
        _logger = logger;
    }


    public async Task<IEnumerable<TeacherDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Teachers
            .AsNoTracking()
            .Select(x => new TeacherDto
            {
                Id = x.TeacherId,
                UserId = x.UserId,
                EmployeeNo = x.EmployeeNo,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                Qualification = x.Qualification,
                HireDate = x.HireDate,
                Status = x.Status,
                DepartmentId = x.DepartmentId
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<TeacherDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Teachers
            .AsNoTracking()
            .Where(x => x.TeacherId == id)
            .Select(x => new TeacherDto
            {
                Id = x.TeacherId,
                UserId = x.UserId,
                EmployeeNo = x.EmployeeNo,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                Qualification = x.Qualification,
                HireDate = x.HireDate,
                Status = x.Status,
                DepartmentId = x.DepartmentId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<TeacherDto> CreateAsync(
        CreateTeacherDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var employeeNo = dto.EmployeeNo.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();
        var firstName = dto.FirstName.Trim();
        var lastName = dto.LastName.Trim();

        // Check employee number
        var employeeExists = await _context.Teachers
            .AnyAsync(
                x => x.EmployeeNo == employeeNo,
                cancellationToken);

        if (employeeExists)
        {
            throw new InvalidOperationException(
                "Employee number already exists.");
        }


        // Check teacher email
        var emailExists = await _context.Teachers
            .AnyAsync(
                x => x.Email == email,
                cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Email already belongs to another teacher.");
        }


        // Check Identity user within tenant
        var userExists = await _userManager.Users
            .AnyAsync(
                x =>
                    x.TenantId == tenantId &&
                    x.Email == email,
                cancellationToken);

        if (userExists)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }


        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // Create Identity User
            var user = new User
            {
                TenantId = tenantId,
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            var userResult = await _userManager.CreateAsync(
                user,
                dto.Password);

            if (!userResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    userResult.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to create teacher account: {errors}");
            }


            // Assign Teacher role
            var roleResult = await _userManager.AddToRoleAsync(
                user,
                "Teacher");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to assign Teacher role: {errors}");
            }


            // Create Teacher
            var teacher = new Teacher
            {
                TeacherId = Guid.NewGuid(),

                TenantId = tenantId,

                UserId = user.Id,

                EmployeeNo = employeeNo,

                FirstName = firstName,

                LastName = lastName,

                Email = email,

                Phone = dto.Phone.Trim(),

                Qualification = dto.Qualification.Trim(),

                HireDate = dto.HireDate,

                Status = dto.Status,

                DepartmentId = dto.DepartmentId
            };

            await _context.Teachers.AddAsync(
                teacher,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);


            _logger.LogInformation(
                "Teacher {TeacherId} created with User {UserId}",
                teacher.TeacherId,
                user.Id);

            return Map(teacher);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }


    public async Task<TeacherDto> UpdateAsync(
        Guid id,
        UpdateTeacherDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(
                x =>
                    x.TeacherId == id &&
                    x.TenantId == tenantId,
                cancellationToken);

        if (teacher is null)
        {
            throw new KeyNotFoundException(
                "Teacher not found.");
        }


        var email = dto.Email.Trim().ToLowerInvariant();


        // Check duplicate email
        var emailExists = await _context.Teachers
            .AnyAsync(
                x =>
                    x.TeacherId != id &&
                    x.TenantId == tenantId &&
                    x.Email == email,
                cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Email already belongs to another teacher.");
        }


        // Load Identity user
        var user = await _userManager.FindByIdAsync(
            teacher.UserId);

        if (user is null)
        {
            throw new InvalidOperationException(
                "Teacher's user account could not be found.");
        }


        // Update Teacher
        teacher.FirstName = dto.FirstName.Trim();

        teacher.LastName = dto.LastName.Trim();

        teacher.Email = email;

        teacher.Phone = dto.Phone.Trim();

        teacher.Qualification =
            dto.Qualification.Trim();

        teacher.HireDate = dto.HireDate;

        teacher.Status = dto.Status;

        teacher.DepartmentId = dto.DepartmentId;


        // Update Identity User
        user.FirstName = teacher.FirstName;

        user.LastName = teacher.LastName;

        user.Email = email;

        user.UserName = email;


        var userResult =
            await _userManager.UpdateAsync(user);

        if (!userResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                userResult.Errors.Select(x => x.Description));

            throw new InvalidOperationException(
                $"Unable to update teacher account: {errors}");
        }


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Teacher {TeacherId} updated",
            id);

        return Map(teacher);
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(
                x =>
                    x.TeacherId == id &&
                    x.TenantId == tenantId,
                cancellationToken);

        if (teacher is null)
        {
            throw new KeyNotFoundException(
                "Teacher not found.");
        }


        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // Find Identity user
            var user = await _userManager.FindByIdAsync(
                teacher.UserId);


            // Delete Teacher first
            _context.Teachers.Remove(teacher);

            await _context.SaveChangesAsync(
                cancellationToken);


            // Delete User
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
                        $"Unable to delete teacher account: {errors}");
                }
            }


            await transaction.CommitAsync(
                cancellationToken);


            _logger.LogInformation(
                "Teacher {TeacherId} and User {UserId} deleted",
                id,
                teacher.UserId);
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        return await _context.Teachers
            .AnyAsync(
                x =>
                    x.TeacherId == id &&
                    x.TenantId == tenantId,
                cancellationToken);
    }


    private string GetTenantId()
    {
        var tenantId =
            _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new InvalidOperationException(
                "No tenant has been resolved.");
        }

        return tenantId;
    }


    private static TeacherDto Map(
        Teacher entity)
    {
        return new TeacherDto
        {
            Id = entity.TeacherId,
            UserId = entity.UserId,
            EmployeeNo = entity.EmployeeNo,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Phone = entity.Phone,
            Qualification = entity.Qualification,
            HireDate = entity.HireDate,
            Status = entity.Status,
            DepartmentId = entity.DepartmentId
        };
    }
}
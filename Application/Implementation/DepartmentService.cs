using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.Department;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class DepartmentService : IDepartmentService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<DepartmentService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Departments
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new DepartmentDto
            {
                DepartmentId = x.DepartmentId,

                Name = x.Name,

                Description = x.Description,

                IsActive = x.IsActive,

                TeacherCount = x.Teachers.Count
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<DepartmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Departments
            .AsNoTracking()
            .Where(x => x.DepartmentId == id)
            .Select(x => new DepartmentDto
            {
                DepartmentId = x.DepartmentId,

                Name = x.Name,

                Description = x.Description,

                IsActive = x.IsActive,

                TeacherCount = x.Teachers.Count
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<DepartmentDto> CreateAsync(
        CreateDepartmentDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Department name is required.");


        var exists = await _context.Departments
            .AnyAsync(
                x => x.Name == name,
                cancellationToken);


        if (exists)
            throw new InvalidOperationException(
                "A department with this name already exists.");


        var department = new Department
        {
            DepartmentId = Guid.NewGuid(),

            TenantId = tenantId,

            Name = name,

            Description =
                dto.Description?.Trim(),

            IsActive = true
        };


        await _context.Departments.AddAsync(
            department,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Department {DepartmentId} created",
            department.DepartmentId);


        return await GetByIdAsync(
            department.DepartmentId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load created department.");
    }


    public async Task<DepartmentDto> UpdateAsync(
        Guid id,
        UpdateDepartmentDto dto,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        var department = await _context.Departments
            .FirstOrDefaultAsync(
                x => x.DepartmentId == id,
                cancellationToken);


        if (department is null)
            throw new KeyNotFoundException(
                "Department not found.");


        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Department name is required.");


        var duplicate = await _context.Departments
            .AnyAsync(
                x =>
                    x.DepartmentId != id &&
                    x.Name == name,
                cancellationToken);


        if (duplicate)
            throw new InvalidOperationException(
                "A department with this name already exists.");


        department.Name = name;

        department.Description =
            dto.Description?.Trim();

        department.IsActive =
            dto.IsActive;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Department {DepartmentId} updated",
            id);


        return await GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load updated department.");
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        var department = await _context.Departments
            .FirstOrDefaultAsync(
                x => x.DepartmentId == id,
                cancellationToken);


        if (department is null)
            throw new KeyNotFoundException(
                "Department not found.");


        var hasTeachers = await _context.Teachers
            .AnyAsync(
                x => x.DepartmentId == id,
                cancellationToken);


        if (hasTeachers)
            throw new InvalidOperationException(
                "A department with assigned teachers cannot be deleted. " +
                "Reassign the teachers first.");
        

        _context.Departments.Remove(
            department);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Department {DepartmentId} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Departments
            .AsNoTracking()
            .AnyAsync(
                x => x.DepartmentId == id,
                cancellationToken);
    }


    private string GetTenantId()
    {
        var tenantId =
            _tenantService.GetCurrentTenantId();


        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException(
                "No tenant has been resolved.");


        return tenantId;
    }
}
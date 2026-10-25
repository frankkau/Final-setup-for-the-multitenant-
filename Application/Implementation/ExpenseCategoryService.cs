using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.ExpenseCategory;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class ExpenseCategoryService : IExpenseCategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<ExpenseCategoryService> _logger;

    public ExpenseCategoryService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<ExpenseCategoryService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<ExpenseCategoryDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.ExpenseCategories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ExpenseCategoryDto
            {
                ExpenseCategoryId = x.ExpenseCategoryId,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<ExpenseCategoryDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.ExpenseCategories
            .AsNoTracking()
            .Where(x => x.ExpenseCategoryId == id)
            .Select(x => new ExpenseCategoryDto
            {
                ExpenseCategoryId = x.ExpenseCategoryId,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<ExpenseCategoryDto> CreateAsync(
        CreateExpenseCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Expense category name is required.");


        var exists = await _context.ExpenseCategories
            .AnyAsync(
                x => x.Name == name,
                cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "Expense category already exists.");


        var entity = new ExpenseCategory
        {
            ExpenseCategoryId = Guid.NewGuid(),

            TenantId = tenantId,

            Name = name,

            Description = dto.Description?.Trim(),

            IsActive = true
        };


        await _context.ExpenseCategories.AddAsync(
            entity,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Expense category {Id} created",
            entity.ExpenseCategoryId);


        return Map(entity);
    }


    public async Task<ExpenseCategoryDto> UpdateAsync(
        Guid id,
        UpdateExpenseCategoryDto dto,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        var entity = await _context.ExpenseCategories
            .FirstOrDefaultAsync(
                x => x.ExpenseCategoryId == id,
                cancellationToken);

        if (entity is null)
            throw new KeyNotFoundException(
                "Expense category not found.");


        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Expense category name is required.");


        var duplicate = await _context.ExpenseCategories
            .AnyAsync(
                x =>
                    x.ExpenseCategoryId != id &&
                    x.Name == name,
                cancellationToken);

        if (duplicate)
            throw new InvalidOperationException(
                "Expense category already exists.");


        entity.Name = name;

        entity.Description = dto.Description?.Trim();

        entity.IsActive = dto.IsActive;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Expense category {Id} updated",
            id);


        return Map(entity);
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        var entity = await _context.ExpenseCategories
            .FirstOrDefaultAsync(
                x => x.ExpenseCategoryId == id,
                cancellationToken);

        if (entity is null)
            throw new KeyNotFoundException(
                "Expense category not found.");


        var hasExpenses = await _context.SchoolExpenses
            .AnyAsync(
                x => x.ExpenseCategoryId == id,
                cancellationToken);

        if (hasExpenses)
            throw new InvalidOperationException(
                "This category cannot be deleted because it has expenses.");


        _context.ExpenseCategories.Remove(entity);

        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Expense category {Id} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.ExpenseCategories
            .AsNoTracking()
            .AnyAsync(
                x => x.ExpenseCategoryId == id,
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


    private static ExpenseCategoryDto Map(
        ExpenseCategory entity)
    {
        return new ExpenseCategoryDto
        {
            ExpenseCategoryId = entity.ExpenseCategoryId,

            Name = entity.Name,

            Description = entity.Description,

            IsActive = entity.IsActive
        };
    }
}
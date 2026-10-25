using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.SchoolExpense;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class SchoolExpenseService : ISchoolExpenseService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<SchoolExpenseService> _logger;

    public SchoolExpenseService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<SchoolExpenseService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<SchoolExpenseListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.SchoolExpenses
            .AsNoTracking()
            .OrderByDescending(x => x.ExpenseDate)
            .Select(x => new SchoolExpenseListDto
            {
                SchoolExpenseId = x.SchoolExpenseId,

                ExpenseNumber = x.ExpenseNumber,

                ExpenseCategory = x.ExpenseCategory.Name,

                Description = x.Description,

                Amount = x.Amount,

                ExpenseDate = x.ExpenseDate,

                PaymentMethod = x.PaymentMethod,

                Payee = x.Payee,

                IsApproved = x.IsApproved,

                IsCancelled = x.IsCancelled,

                HasReceipt = x.Receipts.Any()
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<SchoolExpenseDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.SchoolExpenses
            .AsNoTracking()
            .Where(x => x.SchoolExpenseId == id)
            .Select(x => new SchoolExpenseDto
            {
                SchoolExpenseId = x.SchoolExpenseId,

                ExpenseNumber = x.ExpenseNumber,

                ExpenseCategoryId = x.ExpenseCategoryId,

                ExpenseCategoryName =
                    x.ExpenseCategory.Name,

                AcademicYearId = x.AcademicYearId,

                TermId = x.TermId,

                Description = x.Description,

                Amount = x.Amount,

                ExpenseDate = x.ExpenseDate,

                PaymentMethod = x.PaymentMethod,

                ReferenceNumber = x.ReferenceNumber,

                Payee = x.Payee,

                Remarks = x.Remarks,

                IsApproved = x.IsApproved,

                IsCancelled = x.IsCancelled,

                Receipts = x.Receipts
                    .OrderByDescending(r => r.UploadedAtUtc)
                    .Select(r => new ExpenseReceiptDto
                    {
                        ExpenseReceiptId =
                            r.ExpenseReceiptId,

                        OriginalFileName =
                            r.OriginalFileName,

                        ContentType =
                            r.ContentType,

                        FileSize =
                            r.FileSize,

                        ReceiptDate =
                            r.ReceiptDate,

                        ReceiptNumber =
                            r.ReceiptNumber,

                        ReceiptAmount =
                            r.ReceiptAmount,

                        UploadedAtUtc =
                            r.UploadedAtUtc
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<SchoolExpenseDto> CreateAsync(
        CreateSchoolExpenseDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        if (dto.Amount <= 0)
            throw new ArgumentException(
                "Expense amount must be greater than zero.");


        var description = dto.Description.Trim();

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Expense description is required.");


        var categoryExists =
            await _context.ExpenseCategories
                .AnyAsync(
                    x =>
                        x.ExpenseCategoryId ==
                        dto.ExpenseCategoryId &&
                        x.IsActive,
                    cancellationToken);

        if (!categoryExists)
            throw new KeyNotFoundException(
                "Expense category not found or inactive.");


        var expense = new SchoolExpense
        {
            SchoolExpenseId = Guid.NewGuid(),

            TenantId = tenantId,

            ExpenseCategoryId =
                dto.ExpenseCategoryId,

            AcademicYearId =
                dto.AcademicYearId,

            TermId =
                dto.TermId,

            ExpenseNumber =
                await GenerateExpenseNumberAsync(
                    tenantId,
                    cancellationToken),

            Description = description,

            Amount = dto.Amount,

            ExpenseDate = dto.ExpenseDate,

            PaymentMethod =
                dto.PaymentMethod.Trim(),

            ReferenceNumber =
                dto.ReferenceNumber?.Trim(),

            Payee =
                dto.Payee?.Trim(),

            Remarks =
                dto.Remarks?.Trim(),

            IsApproved = false,

            IsCancelled = false
        };


        await _context.SchoolExpenses.AddAsync(
            expense,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "School expense {ExpenseId} created",
            expense.SchoolExpenseId);


        return await GetByIdAsync(
            expense.SchoolExpenseId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load created expense.");
    }


    public async Task<SchoolExpenseDto> UpdateAsync(
        Guid id,
        UpdateSchoolExpenseDto dto,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var expense =
            await _context.SchoolExpenses
                .FirstOrDefaultAsync(
                    x => x.SchoolExpenseId == id,
                    cancellationToken);


        if (expense is null)
            throw new KeyNotFoundException(
                "School expense not found.");


        if (expense.IsCancelled)
            throw new InvalidOperationException(
                "A cancelled expense cannot be updated.");


        if (expense.IsApproved)
            throw new InvalidOperationException(
                "An approved expense cannot be updated.");


        var categoryExists =
            await _context.ExpenseCategories
                .AnyAsync(
                    x =>
                        x.ExpenseCategoryId ==
                        dto.ExpenseCategoryId &&
                        x.IsActive,
                    cancellationToken);


        if (!categoryExists)
            throw new KeyNotFoundException(
                "Expense category not found or inactive.");


        expense.ExpenseCategoryId =
            dto.ExpenseCategoryId;

        expense.AcademicYearId =
            dto.AcademicYearId;

        expense.TermId =
            dto.TermId;

        expense.Description =
            dto.Description.Trim();

        expense.ExpenseDate =
            dto.ExpenseDate;

        expense.PaymentMethod =
            dto.PaymentMethod.Trim();

        expense.ReferenceNumber =
            dto.ReferenceNumber?.Trim();

        expense.Payee =
            dto.Payee?.Trim();

        expense.Remarks =
            dto.Remarks?.Trim();


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "School expense {ExpenseId} updated",
            id);


        return await GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load updated expense.");
    }


    public async Task CancelAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var expense =
            await _context.SchoolExpenses
                .FirstOrDefaultAsync(
                    x => x.SchoolExpenseId == id,
                    cancellationToken);


        if (expense is null)
            throw new KeyNotFoundException(
                "School expense not found.");


        if (expense.IsCancelled)
            throw new InvalidOperationException(
                "Expense is already cancelled.");


        expense.IsCancelled = true;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "School expense {ExpenseId} cancelled",
            id);
    }


    public async Task ApproveAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var expense =
            await _context.SchoolExpenses
                .FirstOrDefaultAsync(
                    x => x.SchoolExpenseId == id,
                    cancellationToken);


        if (expense is null)
            throw new KeyNotFoundException(
                "School expense not found.");


        if (expense.IsCancelled)
            throw new InvalidOperationException(
                "A cancelled expense cannot be approved.");


        if (expense.IsApproved)
            throw new InvalidOperationException(
                "Expense is already approved.");


        expense.IsApproved = true;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "School expense {ExpenseId} approved",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.SchoolExpenses
            .AsNoTracking()
            .AnyAsync(
                x => x.SchoolExpenseId == id,
                cancellationToken);
    }


    public async Task<IReadOnlyList<SchoolExpenseListDto>>
        GetByTermAsync(
            Guid termId,
            CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.SchoolExpenses
            .AsNoTracking()
            .Where(x =>
                x.TermId == termId &&
                !x.IsCancelled)
            .OrderByDescending(x => x.ExpenseDate)
            .Select(x => new SchoolExpenseListDto
            {
                SchoolExpenseId =
                    x.SchoolExpenseId,

                ExpenseNumber =
                    x.ExpenseNumber,

                ExpenseCategory =
                    x.ExpenseCategory.Name,

                Description =
                    x.Description,

                Amount =
                    x.Amount,

                ExpenseDate =
                    x.ExpenseDate,

                PaymentMethod =
                    x.PaymentMethod,

                Payee =
                    x.Payee,

                IsApproved =
                    x.IsApproved,

                IsCancelled =
                    x.IsCancelled,

                HasReceipt =
                    x.Receipts.Any()
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<IReadOnlyList<SchoolExpenseListDto>>
        GetByAcademicYearAsync(
            Guid academicYearId,
            CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.SchoolExpenses
            .AsNoTracking()
            .Where(x =>
                x.AcademicYearId == academicYearId &&
                !x.IsCancelled)
            .OrderByDescending(x => x.ExpenseDate)
            .Select(x => new SchoolExpenseListDto
            {
                SchoolExpenseId =
                    x.SchoolExpenseId,

                ExpenseNumber =
                    x.ExpenseNumber,

                ExpenseCategory =
                    x.ExpenseCategory.Name,

                Description =
                    x.Description,

                Amount =
                    x.Amount,

                ExpenseDate =
                    x.ExpenseDate,

                PaymentMethod =
                    x.PaymentMethod,

                Payee =
                    x.Payee,

                IsApproved =
                    x.IsApproved,

                IsCancelled =
                    x.IsCancelled,

                HasReceipt =
                    x.Receipts.Any()
            })
            .ToListAsync(cancellationToken);
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


    private async Task<string> GenerateExpenseNumberAsync(
        string tenantId,
        CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;

        var prefix = $"EXP-{year}-";


        var lastNumber =
            await _context.SchoolExpenses
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.ExpenseNumber.StartsWith(prefix))
                .OrderByDescending(x => x.ExpenseNumber)
                .Select(x => x.ExpenseNumber)
                .FirstOrDefaultAsync(
                    cancellationToken);


        var nextNumber = 1;


        if (!string.IsNullOrWhiteSpace(lastNumber))
        {
            var numberPart =
                lastNumber[prefix.Length..];

            if (int.TryParse(
                numberPart,
                out var currentNumber))
            {
                nextNumber = currentNumber + 1;
            }
        }


        return $"{prefix}{nextNumber:D6}";
    }
}
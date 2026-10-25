using System.Security.Cryptography;
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.ExpenseReceipt;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class ExpenseReceiptService : IExpenseReceiptService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ExpenseReceiptService> _logger;

    private const long MaxFileSize = 10 * 1024 * 1024;

    private static readonly string[] AllowedContentTypes =
    {
        "application/pdf",
        "image/jpeg",
        "image/png"
    };


    public ExpenseReceiptService(
        ApplicationDbContext context,
        ITenantService tenantService,
        IWebHostEnvironment environment,
        ILogger<ExpenseReceiptService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _environment = environment;
        _logger = logger;
    }


    public async Task<IReadOnlyList<ExpenseReceiptDto>>
        GetByExpenseAsync(
            Guid schoolExpenseId,
            CancellationToken cancellationToken)
    {
        GetTenantId();


        return await _context.ExpenseReceipts
            .AsNoTracking()
            .Where(x =>
                x.SchoolExpenseId == schoolExpenseId)
            .OrderByDescending(x => x.UploadedAtUtc)
            .Select(x => new ExpenseReceiptDto
            {
                ExpenseReceiptId =
                    x.ExpenseReceiptId,

                SchoolExpenseId =
                    x.SchoolExpenseId,

                OriginalFileName =
                    x.OriginalFileName,

                ContentType =
                    x.ContentType,

                FileSize =
                    x.FileSize,

                ReceiptDate =
                    x.ReceiptDate,

                ReceiptNumber =
                    x.ReceiptNumber,

                ReceiptAmount =
                    x.ReceiptAmount,

                UploadedAtUtc =
                    x.UploadedAtUtc
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<ExpenseReceiptDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        return await _context.ExpenseReceipts
            .AsNoTracking()
            .Where(x =>
                x.ExpenseReceiptId == id)
            .Select(x => new ExpenseReceiptDto
            {
                ExpenseReceiptId =
                    x.ExpenseReceiptId,

                SchoolExpenseId =
                    x.SchoolExpenseId,

                OriginalFileName =
                    x.OriginalFileName,

                ContentType =
                    x.ContentType,

                FileSize =
                    x.FileSize,

                ReceiptDate =
                    x.ReceiptDate,

                ReceiptNumber =
                    x.ReceiptNumber,

                ReceiptAmount =
                    x.ReceiptAmount,

                UploadedAtUtc =
                    x.UploadedAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<ExpenseReceiptDto> CreateAsync(
        CreateExpenseReceiptDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        if (dto.Receipt is null)
            throw new ArgumentException(
                "Receipt file is required.");


        if (dto.Receipt.Length <= 0)
            throw new ArgumentException(
                "Receipt file is empty.");


        if (dto.Receipt.Length > MaxFileSize)
            throw new ArgumentException(
                "Receipt file cannot exceed 10 MB.");


        if (!AllowedContentTypes.Contains(
            dto.Receipt.ContentType,
            StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Only PDF, JPEG and PNG receipts are allowed.");
        }


        var expense = await _context.SchoolExpenses
            .FirstOrDefaultAsync(
                x =>
                    x.SchoolExpenseId ==
                    dto.SchoolExpenseId,
                cancellationToken);


        if (expense is null)
            throw new KeyNotFoundException(
                "School expense not found.");


        if (expense.IsCancelled)
            throw new InvalidOperationException(
                "A receipt cannot be added to a cancelled expense.");


        if (expense.IsApproved)
            throw new InvalidOperationException(
                "A receipt cannot be added after the expense has been approved.");


        if (dto.ReceiptAmount.HasValue &&
            dto.ReceiptAmount.Value <= 0)
        {
            throw new ArgumentException(
                "Receipt amount must be greater than zero.");
        }


        if (dto.ReceiptAmount.HasValue &&
            dto.ReceiptAmount.Value != expense.Amount)
        {
            throw new InvalidOperationException(
                "Receipt amount does not match the expense amount.");
        }


        await using var stream =
            dto.Receipt.OpenReadStream();


        var hashBytes =
            await SHA256.HashDataAsync(
                stream,
                cancellationToken);


        var fileHash =
            Convert.ToHexString(hashBytes);


        var duplicate = await _context.ExpenseReceipts
            .AnyAsync(
                x => x.FileHash == fileHash,
                cancellationToken);


        if (duplicate)
            throw new InvalidOperationException(
                "This receipt has already been uploaded.");


        var extension =
            Path.GetExtension(
                dto.Receipt.FileName);


        var storedFileName =
            $"{Guid.NewGuid():N}{extension}";


        var uploadDirectory =
            Path.Combine(
                _environment.ContentRootPath,
                "Storage",
                "Receipts",
                tenantId);


        Directory.CreateDirectory(
            uploadDirectory);


        var filePath =
            Path.Combine(
                uploadDirectory,
                storedFileName);


        await using (var fileStream =
            new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
        {
            await dto.Receipt.CopyToAsync(
                fileStream,
                cancellationToken);
        }


        var receipt = new ExpenseReceipt
        {
            ExpenseReceiptId =
                Guid.NewGuid(),

            TenantId =
                tenantId,

            SchoolExpenseId =
                dto.SchoolExpenseId,

            OriginalFileName =
                Path.GetFileName(
                    dto.Receipt.FileName),

            StoredFileName =
                storedFileName,

            ContentType =
                dto.Receipt.ContentType,

            FileSize =
                dto.Receipt.Length,

            FileHash =
                fileHash,

            ReceiptDate =
                dto.ReceiptDate,

            ReceiptNumber =
                dto.ReceiptNumber?.Trim(),

            ReceiptAmount =
                dto.ReceiptAmount,

            UploadedAtUtc =
                DateTime.UtcNow
        };


        try
        {
            await _context.ExpenseReceipts.AddAsync(
                receipt,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            throw;
        }


        _logger.LogInformation(
            "Receipt {ReceiptId} uploaded for expense {ExpenseId}",
            receipt.ExpenseReceiptId,
            receipt.SchoolExpenseId);


        return Map(receipt);
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var receipt =
            await _context.ExpenseReceipts
                .Include(x => x.SchoolExpense)
                .FirstOrDefaultAsync(
                    x =>
                        x.ExpenseReceiptId == id,
                    cancellationToken);


        if (receipt is null)
            throw new KeyNotFoundException(
                "Expense receipt not found.");


        if (receipt.SchoolExpense.IsApproved)
            throw new InvalidOperationException(
                "A receipt belonging to an approved expense cannot be deleted.");


        var filePath =
            Path.Combine(
                _environment.ContentRootPath,
                "Storage",
                "Receipts",
                receipt.TenantId,
                receipt.StoredFileName);


        _context.ExpenseReceipts.Remove(receipt);


        await _context.SaveChangesAsync(
            cancellationToken);


        if (File.Exists(filePath))
            File.Delete(filePath);


        _logger.LogInformation(
            "Receipt {ReceiptId} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        return await _context.ExpenseReceipts
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.ExpenseReceiptId == id,
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


    private static ExpenseReceiptDto Map(
        ExpenseReceipt entity)
    {
        return new ExpenseReceiptDto
        {
            ExpenseReceiptId =
                entity.ExpenseReceiptId,

            SchoolExpenseId =
                entity.SchoolExpenseId,

            OriginalFileName =
                entity.OriginalFileName,

            ContentType =
                entity.ContentType,

            FileSize =
                entity.FileSize,

            ReceiptDate =
                entity.ReceiptDate,

            ReceiptNumber =
                entity.ReceiptNumber,

            ReceiptAmount =
                entity.ReceiptAmount,

            UploadedAtUtc =
                entity.UploadedAtUtc
        };
    }
}
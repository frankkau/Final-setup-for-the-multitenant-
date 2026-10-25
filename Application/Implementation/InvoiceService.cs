using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.Invoice;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<InvoiceService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<InvoiceListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Invoices
            .AsNoTracking()
            .OrderByDescending(x => x.InvoiceDate)
            .Select(x => new InvoiceListDto
            {
                InvoiceId = x.InvoiceId,

                InvoiceNumber = x.InvoiceNumber,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,

                FeeName = x.FeeStructure.Name,

                Amount = x.Amount,

                AmountPaid = x.AmountPaid,

                Balance = x.Balance,

                InvoiceDate = x.InvoiceDate,

                IsPaid = x.IsPaid
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<InvoiceDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Invoices
            .AsNoTracking()
            .Where(x => x.InvoiceId == id)
            .Select(x => new InvoiceDto
            {
                InvoiceId = x.InvoiceId,

                InvoiceNumber = x.InvoiceNumber,

                StudentId = x.StudentId,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,

                FeeStructureId = x.FeeStructureId,

                FeeName = x.FeeStructure.Name,

                Amount = x.Amount,

                AmountPaid = x.AmountPaid,

                Balance = x.Balance,

                InvoiceDate = x.InvoiceDate,

                DueDate = x.DueDate,

                IsPaid = x.IsPaid,

                IsCancelled = x.IsCancelled,

                Remarks = x.Remarks
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<InvoiceDto> CreateAsync(
        CreateInvoiceDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        if (dto.Amount <= 0)
            throw new InvalidOperationException(
                "Invoice amount must be greater than zero.");


        var studentExists = await _context.Students
            .AnyAsync(
                x => x.StudentId == dto.StudentId,
                cancellationToken);

        if (!studentExists)
            throw new KeyNotFoundException(
                "Student not found.");


        var feeStructure = await _context.FeeStructures
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.FeeStructureId == dto.FeeStructureId,
                cancellationToken);

        if (feeStructure is null)
            throw new KeyNotFoundException(
                "Fee structure not found.");


        var invoice = new Invoice
        {
            InvoiceId = Guid.NewGuid(),

            TenantId = tenantId,

            StudentId = dto.StudentId,

            FeeStructureId = dto.FeeStructureId,

            InvoiceNumber = GenerateInvoiceNumber(),

            Amount = dto.Amount,

            AmountPaid = 0,

            Balance = dto.Amount,

            InvoiceDate = dto.InvoiceDate,

            DueDate = dto.DueDate,

            IsPaid = false,

            IsCancelled = false,

            Remarks = dto.Remarks?.Trim()
        };


        await _context.Invoices.AddAsync(
            invoice,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Invoice {InvoiceId} created for student {StudentId}",
            invoice.InvoiceId,
            invoice.StudentId);


        return await GetByIdAsync(
            invoice.InvoiceId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load created invoice.");
    }


    public async Task<InvoiceDto> UpdateAsync(
        Guid id,
        UpdateInvoiceDto dto,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(
                x => x.InvoiceId == id,
                cancellationToken);

        if (invoice is null)
            throw new KeyNotFoundException(
                "Invoice not found.");


        if (invoice.IsPaid)
            throw new InvalidOperationException(
                "A paid invoice cannot be modified.");


        if (invoice.IsCancelled)
            throw new InvalidOperationException(
                "A cancelled invoice cannot be modified.");


        invoice.DueDate = dto.DueDate;

        invoice.Remarks = dto.Remarks?.Trim();


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Invoice {InvoiceId} updated",
            id);


        return await GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load updated invoice.");
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(
                x => x.InvoiceId == id,
                cancellationToken);

        if (invoice is null)
            throw new KeyNotFoundException(
                "Invoice not found.");


        if (invoice.AmountPaid > 0)
            throw new InvalidOperationException(
                "An invoice with payments cannot be deleted.");


        _context.Invoices.Remove(invoice);

        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Invoice {InvoiceId} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Invoices
            .AsNoTracking()
            .AnyAsync(
                x => x.InvoiceId == id,
                cancellationToken);
    }


    public async Task<IReadOnlyList<InvoiceListDto>> GetByStudentIdAsync(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Invoices
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.InvoiceDate)
            .Select(x => new InvoiceListDto
            {
                InvoiceId = x.InvoiceId,

                InvoiceNumber = x.InvoiceNumber,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,

                FeeName = x.FeeStructure.Name,

                Amount = x.Amount,

                AmountPaid = x.AmountPaid,

                Balance = x.Balance,

                InvoiceDate = x.InvoiceDate,

                IsPaid = x.IsPaid
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


    private static string GenerateInvoiceNumber()
    {
        return $"INV-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }
}
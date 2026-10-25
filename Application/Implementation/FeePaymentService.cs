using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.FeePayment;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class FeePaymentService : IFeePaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<FeePaymentService> _logger;

    public FeePaymentService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<FeePaymentService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<FeePaymentListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.FeePayments
            .AsNoTracking()
            .OrderByDescending(x => x.PaymentDate)
            .Select(x => new FeePaymentListDto
            {
                FeePaymentId = x.FeePaymentId,

                StudentId = x.StudentId,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,

                FeeName = x.FeeStructure.Name,

                AmountPaid = x.AmountPaid,

                PaymentDate = x.PaymentDate,

                PaymentMethod = x.PaymentMethod,

                ReferenceNumber = x.ReferenceNumber
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<FeePaymentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.FeePayments
            .AsNoTracking()
            .Where(x => x.FeePaymentId == id)
            .Select(x => new FeePaymentDto
            {
                FeePaymentId = x.FeePaymentId,

                StudentId = x.StudentId,

                StudentName =
                    x.Student.FirstName + " " +
                    x.Student.LastName,

                FeeStructureId = x.FeeStructureId,

                FeeName = x.FeeStructure.Name,

                AmountPaid = x.AmountPaid,

                PaymentDate = x.PaymentDate,

                PaymentMethod = x.PaymentMethod,

                ReferenceNumber = x.ReferenceNumber,

                Remarks = x.Remarks
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<FeePaymentDto> CreateAsync(
        CreateFeePaymentDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        if (dto.AmountPaid <= 0)
            throw new ArgumentException(
                "Payment amount must be greater than zero.");


        if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            throw new ArgumentException(
                "Payment method is required.");


        var studentExists = await _context.Students
            .AnyAsync(
                x => x.StudentId == dto.StudentId,
                cancellationToken);


        if (!studentExists)
            throw new KeyNotFoundException(
                "Student not found.");


        var feeStructureExists = await _context.FeeStructures
            .AnyAsync(
                x => x.FeeStructureId == dto.FeeStructureId,
                cancellationToken);


        if (!feeStructureExists)
            throw new KeyNotFoundException(
                "Fee structure not found.");


        var payment = new FeePayment
        {
            FeePaymentId = Guid.NewGuid(),

            TenantId = tenantId,

            StudentId = dto.StudentId,

            FeeStructureId = dto.FeeStructureId,

            AmountPaid = dto.AmountPaid,

            PaymentDate = dto.PaymentDate,

            PaymentMethod = dto.PaymentMethod.Trim(),

            ReferenceNumber =
                string.IsNullOrWhiteSpace(dto.ReferenceNumber)
                    ? null
                    : dto.ReferenceNumber.Trim(),

            Remarks =
                string.IsNullOrWhiteSpace(dto.Remarks)
                    ? null
                    : dto.Remarks.Trim()
        };


        await _context.FeePayments.AddAsync(
            payment,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Fee payment {PaymentId} created for student {StudentId}",
            payment.FeePaymentId,
            payment.StudentId);


        return await GetByIdAsync(
            payment.FeePaymentId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load created fee payment.");
    }


    public async Task<FeePaymentDto> UpdateAsync(
        Guid id,
        UpdateFeePaymentDto dto,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var payment = await _context.FeePayments
            .FirstOrDefaultAsync(
                x => x.FeePaymentId == id,
                cancellationToken);


        if (payment is null)
            throw new KeyNotFoundException(
                "Fee payment not found.");


        if (dto.AmountPaid <= 0)
            throw new ArgumentException(
                "Payment amount must be greater than zero.");


        if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            throw new ArgumentException(
                "Payment method is required.");


        payment.AmountPaid = dto.AmountPaid;

        payment.PaymentDate = dto.PaymentDate;

        payment.PaymentMethod =
            dto.PaymentMethod.Trim();

        payment.ReferenceNumber =
            string.IsNullOrWhiteSpace(dto.PaymentReference)
                ? null
                : dto.PaymentReference.Trim();

        payment.Remarks =
            string.IsNullOrWhiteSpace(dto.Remarks)
                ? null
                : dto.Remarks.Trim();


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Fee payment {PaymentId} updated",
            id);


        return await GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load updated fee payment.");
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var payment = await _context.FeePayments
            .FirstOrDefaultAsync(
                x => x.FeePaymentId == id,
                cancellationToken);


        if (payment is null)
            throw new KeyNotFoundException(
                "Fee payment not found.");


        _context.FeePayments.Remove(payment);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Fee payment {PaymentId} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        return await _context.FeePayments
            .AsNoTracking()
            .AnyAsync(
                x => x.FeePaymentId == id,
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
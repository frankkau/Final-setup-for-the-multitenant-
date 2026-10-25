using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.LibraryLoan;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class LibraryLoanService : ILibraryLoanService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<LibraryLoanService> _logger;

    public LibraryLoanService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<LibraryLoanService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<LibraryLoanDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.LibraryLoans
            .AsNoTracking()
            .OrderByDescending(x => x.BorrowedDate)
            .Select(x => new LibraryLoanDto
            {
                LibraryLoanId = x.LibraryLoanId,

                LibraryBookCopyId =
                    x.LibraryBookCopyId,

                AccessionNumber =
                    x.LibraryBookCopy.AccessionNumber,

                BookTitle =
                    x.LibraryBookCopy.LibraryBook.Title,

                StudentId =
                    x.StudentId,

                StudentName =
                    x.Student.FirstName +
                    " " +
                    x.Student.LastName,

                BorrowedDate =
                    x.BorrowedDate,

                DueDate =
                    x.DueDate,

                ReturnedDate =
                    x.ReturnedDate,

                FineAmount =
                    x.FineAmount,

                Remarks =
                    x.Remarks,

                IsReturned =
                    x.IsReturned
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<IReadOnlyList<LibraryLoanDto>> GetByStudentAsync(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var studentExists = await _context.Students
            .AnyAsync(
                x => x.StudentId == studentId,
                cancellationToken);


        if (!studentExists)
            throw new KeyNotFoundException(
                "Student not found.");


        return await _context.LibraryLoans
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.BorrowedDate)
            .Select(x => new LibraryLoanDto
            {
                LibraryLoanId =
                    x.LibraryLoanId,

                LibraryBookCopyId =
                    x.LibraryBookCopyId,

                AccessionNumber =
                    x.LibraryBookCopy.AccessionNumber,

                BookTitle =
                    x.LibraryBookCopy.LibraryBook.Title,

                StudentId =
                    x.StudentId,

                StudentName =
                    x.Student.FirstName +
                    " " +
                    x.Student.LastName,

                BorrowedDate =
                    x.BorrowedDate,

                DueDate =
                    x.DueDate,

                ReturnedDate =
                    x.ReturnedDate,

                FineAmount =
                    x.FineAmount,

                Remarks =
                    x.Remarks,

                IsReturned =
                    x.IsReturned
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<LibraryLoanDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        return await _context.LibraryLoans
            .AsNoTracking()
            .Where(x => x.LibraryLoanId == id)
            .Select(x => new LibraryLoanDto
            {
                LibraryLoanId =
                    x.LibraryLoanId,

                LibraryBookCopyId =
                    x.LibraryBookCopyId,

                AccessionNumber =
                    x.LibraryBookCopy.AccessionNumber,

                BookTitle =
                    x.LibraryBookCopy.LibraryBook.Title,

                StudentId =
                    x.StudentId,

                StudentName =
                    x.Student.FirstName +
                    " " +
                    x.Student.LastName,

                BorrowedDate =
                    x.BorrowedDate,

                DueDate =
                    x.DueDate,

                ReturnedDate =
                    x.ReturnedDate,

                FineAmount =
                    x.FineAmount,

                Remarks =
                    x.Remarks,

                IsReturned =
                    x.IsReturned
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<LibraryLoanDto> BorrowAsync(
        CreateLibraryLoanDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        if (dto.DueDate < dto.BorrowedDate)
            throw new InvalidOperationException(
                "Due date cannot be earlier than borrowed date.");


        var studentExists = await _context.Students
            .AnyAsync(
                x => x.StudentId == dto.StudentId,
                cancellationToken);


        if (!studentExists)
            throw new KeyNotFoundException(
                "Student not found.");


        var copy = await _context.LibraryBookCopies
            .FirstOrDefaultAsync(
                x => x.LibraryBookCopyId ==
                     dto.LibraryBookCopyId,
                cancellationToken);


        if (copy is null)
            throw new KeyNotFoundException(
                "Library book copy not found.");


        if (!copy.IsActive)
            throw new InvalidOperationException(
                "This book copy is inactive.");


        if (!copy.IsAvailable)
            throw new InvalidOperationException(
                "This book copy is currently borrowed.");


        // Prevent the same student from
        // borrowing the same physical copy twice.
        var existingLoan = await _context.LibraryLoans
            .AnyAsync(
                x =>
                    x.StudentId == dto.StudentId &&
                    x.LibraryBookCopyId ==
                        dto.LibraryBookCopyId &&
                    !x.IsReturned,
                cancellationToken);


        if (existingLoan)
            throw new InvalidOperationException(
                "The student already has this book copy.");


        var loan = new LibraryLoan
        {
            LibraryLoanId = Guid.NewGuid(),

            TenantId = tenantId,

            LibraryBookCopyId =
                dto.LibraryBookCopyId,

            StudentId =
                dto.StudentId,

            BorrowedDate =
                dto.BorrowedDate,

            DueDate =
                dto.DueDate,

            ReturnedDate = null,

            FineAmount = 0,

            Remarks =
                dto.Remarks?.Trim(),

            IsReturned = false
        };


        copy.IsAvailable = false;


        await _context.LibraryLoans.AddAsync(
            loan,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Library book copy {CopyId} borrowed by student {StudentId}. Loan {LoanId}",
            dto.LibraryBookCopyId,
            dto.StudentId,
            loan.LibraryLoanId);


        return await GetByIdAsync(
            loan.LibraryLoanId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load created library loan.");
    }


    public async Task ReturnAsync(
        Guid id,
        decimal fineAmount,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        if (fineAmount < 0)
            throw new InvalidOperationException(
                "Fine amount cannot be negative.");


        var loan = await _context.LibraryLoans
            .Include(x => x.LibraryBookCopy)
            .FirstOrDefaultAsync(
                x => x.LibraryLoanId == id,
                cancellationToken);


        if (loan is null)
            throw new KeyNotFoundException(
                "Library loan not found.");


        if (loan.IsReturned)
            throw new InvalidOperationException(
                "This book has already been returned.");


        loan.ReturnedDate = DateOnly.FromDateTime(
            DateTime.UtcNow);

        loan.FineAmount = fineAmount;

        loan.IsReturned = true;


        loan.LibraryBookCopy.IsAvailable = true;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Library loan {LoanId} returned",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.LibraryLoans
            .AsNoTracking()
            .AnyAsync(
                x => x.LibraryLoanId == id,
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
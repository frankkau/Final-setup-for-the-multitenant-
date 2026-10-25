using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.LibraryBookCopy;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class LibraryBookCopyService : ILibraryBookCopyService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<LibraryBookCopyService> _logger;

    public LibraryBookCopyService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<LibraryBookCopyService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<LibraryBookCopyDto>> GetByBookAsync(
        Guid libraryBookId,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.LibraryBookCopies
            .AsNoTracking()
            .Where(x => x.LibraryBookId == libraryBookId)
            .OrderBy(x => x.AccessionNumber)
            .Select(x => new LibraryBookCopyDto
            {
                LibraryBookCopyId = x.LibraryBookCopyId,

                LibraryBookId = x.LibraryBookId,

                BookTitle = x.LibraryBook.Title,

                AccessionNumber = x.AccessionNumber,

                ShelfLocation = x.ShelfLocation,

                IsAvailable = x.IsAvailable,

                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<LibraryBookCopyDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.LibraryBookCopies
            .AsNoTracking()
            .Where(x => x.LibraryBookCopyId == id)
            .Select(x => new LibraryBookCopyDto
            {
                LibraryBookCopyId = x.LibraryBookCopyId,

                LibraryBookId = x.LibraryBookId,

                BookTitle = x.LibraryBook.Title,

                AccessionNumber = x.AccessionNumber,

                ShelfLocation = x.ShelfLocation,

                IsAvailable = x.IsAvailable,

                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<LibraryBookCopyDto> CreateAsync(
        CreateLibraryBookCopyDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        var bookExists = await _context.LibraryBooks
            .AnyAsync(
                x => x.LibraryBookId == dto.LibraryBookId &&
                     x.IsActive,
                cancellationToken);


        if (!bookExists)
            throw new KeyNotFoundException(
                "Library book not found.");


        var accessionNumber =
            dto.AccessionNumber.Trim();


        var accessionExists = await _context.LibraryBookCopies
            .AnyAsync(
                x => x.AccessionNumber == accessionNumber,
                cancellationToken);


        if (accessionExists)
            throw new InvalidOperationException(
                "Accession number already exists.");


        var copy = new LibraryBookCopy
        {
            LibraryBookCopyId = Guid.NewGuid(),

            TenantId = tenantId,

            LibraryBookId = dto.LibraryBookId,

            AccessionNumber = accessionNumber,

            ShelfLocation = dto.ShelfLocation?.Trim(),

            IsAvailable = true,

            IsActive = true
        };


        await _context.LibraryBookCopies.AddAsync(
            copy,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Library book copy {Id} created for book {BookId}",
            copy.LibraryBookCopyId,
            copy.LibraryBookId);


        return await GetByIdAsync(
            copy.LibraryBookCopyId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load created library book copy.");
    }


    public async Task<LibraryBookCopyDto> UpdateAsync(
        Guid id,
        UpdateLibraryBookCopyDto dto,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var copy = await _context.LibraryBookCopies
            .FirstOrDefaultAsync(
                x => x.LibraryBookCopyId == id,
                cancellationToken);


        if (copy is null)
            throw new KeyNotFoundException(
                "Library book copy not found.");


        var accessionNumber =
            dto.AccessionNumber.Trim();


        var accessionExists = await _context.LibraryBookCopies
            .AnyAsync(
                x =>
                    x.LibraryBookCopyId != id &&
                    x.AccessionNumber == accessionNumber,
                cancellationToken);


        if (accessionExists)
            throw new InvalidOperationException(
                "Accession number already belongs to another book copy.");


        // Do not allow an inactive copy to become
        // available while it is currently borrowed.
        if (!dto.IsActive)
            copy.IsAvailable = false;


        copy.AccessionNumber = accessionNumber;

        copy.ShelfLocation =
            dto.ShelfLocation?.Trim();

        copy.IsActive = dto.IsActive;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Library book copy {Id} updated",
            id);


        return await GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load updated library book copy.");
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var copy = await _context.LibraryBookCopies
            .FirstOrDefaultAsync(
                x => x.LibraryBookCopyId == id,
                cancellationToken);


        if (copy is null)
            throw new KeyNotFoundException(
                "Library book copy not found.");


        var hasLoans = await _context.LibraryLoans
            .AnyAsync(
                x => x.LibraryBookCopyId == id,
                cancellationToken);


        if (hasLoans)
            throw new InvalidOperationException(
                "A book copy with borrowing history cannot be deleted.");


        _context.LibraryBookCopies.Remove(copy);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Library book copy {Id} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.LibraryBookCopies
            .AsNoTracking()
            .AnyAsync(
                x => x.LibraryBookCopyId == id,
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
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.LibraryBook;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class LibraryBookService : ILibraryBookService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<LibraryBookService> _logger;

    public LibraryBookService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<LibraryBookService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<LibraryBookDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.LibraryBooks
            .AsNoTracking()
            .OrderBy(x => x.Title)
            .Select(x => new LibraryBookDto
            {
                LibraryBookId = x.LibraryBookId,
                ISBN = x.ISBN,
                Title = x.Title,
                Author = x.Author,
                Publisher = x.Publisher,
                Edition = x.Edition,
                PublicationYear = x.PublicationYear,
                Category = x.Category,
                Description = x.Description,
                IsActive = x.IsActive,
                TotalCopies = x.Copies.Count,
                AvailableCopies =
                    x.Copies.Count(c => c.IsAvailable && c.IsActive)
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<LibraryBookDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.LibraryBooks
            .AsNoTracking()
            .Where(x => x.LibraryBookId == id)
            .Select(x => new LibraryBookDto
            {
                LibraryBookId = x.LibraryBookId,
                ISBN = x.ISBN,
                Title = x.Title,
                Author = x.Author,
                Publisher = x.Publisher,
                Edition = x.Edition,
                PublicationYear = x.PublicationYear,
                Category = x.Category,
                Description = x.Description,
                IsActive = x.IsActive,
                TotalCopies = x.Copies.Count,
                AvailableCopies =
                    x.Copies.Count(c => c.IsAvailable && c.IsActive)
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<LibraryBookDto> CreateAsync(
        CreateLibraryBookDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        var isbn = dto.ISBN.Trim();

        var exists = await _context.LibraryBooks
            .AnyAsync(
                x => x.ISBN == isbn,
                cancellationToken);

        if (exists)
            throw new InvalidOperationException(
                "A book with this ISBN already exists.");


        var book = new LibraryBook
        {
            LibraryBookId = Guid.NewGuid(),
            TenantId = tenantId,
            ISBN = isbn,
            Title = dto.Title.Trim(),
            Author = dto.Author.Trim(),
            Publisher = dto.Publisher?.Trim(),
            Edition = dto.Edition?.Trim(),
            PublicationYear = dto.PublicationYear,
            Category = dto.Category?.Trim(),
            Description = dto.Description?.Trim(),
            IsActive = true
        };


        await _context.LibraryBooks.AddAsync(
            book,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Library book {Id} created",
            book.LibraryBookId);

        return await GetByIdAsync(
            book.LibraryBookId,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load created library book.");
    }


    public async Task<LibraryBookDto> UpdateAsync(
        Guid id,
        UpdateLibraryBookDto dto,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        var book = await _context.LibraryBooks
            .FirstOrDefaultAsync(
                x => x.LibraryBookId == id,
                cancellationToken);

        if (book is null)
            throw new KeyNotFoundException(
                "Library book not found.");


        var isbn = dto.ISBN.Trim();

        var duplicate = await _context.LibraryBooks
            .AnyAsync(
                x =>
                    x.LibraryBookId != id &&
                    x.ISBN == isbn,
                cancellationToken);

        if (duplicate)
            throw new InvalidOperationException(
                "A book with this ISBN already exists.");


        book.ISBN = isbn;
        book.Title = dto.Title.Trim();
        book.Author = dto.Author.Trim();
        book.Publisher = dto.Publisher?.Trim();
        book.Edition = dto.Edition?.Trim();
        book.PublicationYear = dto.PublicationYear;
        book.Category = dto.Category?.Trim();
        book.Description = dto.Description?.Trim();
        book.IsActive = dto.IsActive;


        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Library book {Id} updated",
            id);

        return await GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Failed to load updated library book.");
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        var book = await _context.LibraryBooks
            .FirstOrDefaultAsync(
                x => x.LibraryBookId == id,
                cancellationToken);

        if (book is null)
            throw new KeyNotFoundException(
                "Library book not found.");

        var hasLoans = await _context.LibraryLoans
            .AnyAsync(
                x => x.LibraryBookCopy.LibraryBookId == id,
                cancellationToken);

        if (hasLoans)
            throw new InvalidOperationException(
                "A book with borrowing history cannot be deleted.");

        _context.LibraryBooks.Remove(book);

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Library book {Id} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.LibraryBooks
            .AsNoTracking()
            .AnyAsync(
                x => x.LibraryBookId == id,
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
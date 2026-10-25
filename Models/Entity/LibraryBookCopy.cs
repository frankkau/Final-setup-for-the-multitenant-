using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class LibraryBookCopy : IMustHaveTenant
{
    public Guid LibraryBookCopyId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid LibraryBookId { get; set; }

    public string AccessionNumber { get; set; } = string.Empty;

    public string? ShelfLocation { get; set; }

    public bool IsAvailable { get; set; } = true;

    public bool IsActive { get; set; } = true;


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public LibraryBook LibraryBook { get; set; } = default!;

    public ICollection<LibraryLoan> Loans { get; set; }
        = new List<LibraryLoan>();
}
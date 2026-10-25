using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class LibraryBook : IMustHaveTenant
{
    public Guid LibraryBookId { get; set; }

    public string TenantId { get; set; } = default!;

    public string ISBN { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string? Publisher { get; set; }

    public string? Edition { get; set; }

    public int PublicationYear { get; set; }

    public string? Category { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public ICollection<LibraryBookCopy> Copies { get; set; }
        = new List<LibraryBookCopy>();
}
namespace CouncilRevenueCollection.Models.Dtos.LibraryBookCopy;

public class LibraryBookCopyDto
{
    public Guid LibraryBookCopyId { get; set; }

    public Guid LibraryBookId { get; set; }

    public string BookTitle { get; set; } = string.Empty;

    public string AccessionNumber { get; set; } = string.Empty;

    public string? ShelfLocation { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsActive { get; set; }
}
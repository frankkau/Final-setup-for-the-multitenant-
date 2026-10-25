namespace CouncilRevenueCollection.Models.Dtos.LibraryBookCopy;

public class CreateLibraryBookCopyDto
{
    public Guid LibraryBookId { get; set; }

    public string AccessionNumber { get; set; } = string.Empty;

    public string? ShelfLocation { get; set; }
}
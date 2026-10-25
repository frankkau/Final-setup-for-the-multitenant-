namespace CouncilRevenueCollection.Models.Dtos.LibraryBookCopy;

public class UpdateLibraryBookCopyDto
{
    public string AccessionNumber { get; set; } = string.Empty;

    public string? ShelfLocation { get; set; }

    public bool IsActive { get; set; }
}
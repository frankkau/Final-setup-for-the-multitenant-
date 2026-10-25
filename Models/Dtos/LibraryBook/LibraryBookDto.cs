namespace CouncilRevenueCollection.Models.Dtos.LibraryBook;

public class LibraryBookDto
{
    public Guid LibraryBookId { get; set; }

    public string ISBN { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string? Publisher { get; set; }

    public string? Edition { get; set; }

    public int PublicationYear { get; set; }

    public string? Category { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int TotalCopies { get; set; }

    public int AvailableCopies { get; set; }
}
namespace CouncilRevenueCollection.Models.Dtos.LibraryBook;

public class CreateLibraryBookDto
{
    public string ISBN { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string? Publisher { get; set; }

    public string? Edition { get; set; }

    public int PublicationYear { get; set; }

    public string? Category { get; set; }

    public string? Description { get; set; }
}
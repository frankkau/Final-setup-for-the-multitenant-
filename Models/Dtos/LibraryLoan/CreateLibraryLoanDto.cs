namespace CouncilRevenueCollection.Models.Dtos.LibraryLoan;

public class CreateLibraryLoanDto
{
    public Guid LibraryBookCopyId { get; set; }

    public Guid StudentId { get; set; }

    public DateOnly BorrowedDate { get; set; }

    public DateOnly DueDate { get; set; }

    public string? Remarks { get; set; }
}
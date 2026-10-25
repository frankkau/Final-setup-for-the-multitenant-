namespace CouncilRevenueCollection.Models.Dtos.LibraryLoan;

public class LibraryLoanDto
{
    public Guid LibraryLoanId { get; set; }

    public Guid LibraryBookCopyId { get; set; }

    public string AccessionNumber { get; set; } = string.Empty;

    public string BookTitle { get; set; } = string.Empty;

    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public DateOnly BorrowedDate { get; set; }

    public DateOnly DueDate { get; set; }

    public DateOnly? ReturnedDate { get; set; }

    public decimal FineAmount { get; set; }

    public string? Remarks { get; set; }

    public bool IsReturned { get; set; }
}
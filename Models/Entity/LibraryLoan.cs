using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class LibraryLoan : IMustHaveTenant
{
    public Guid LibraryLoanId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid LibraryBookCopyId { get; set; }

    public Guid StudentId { get; set; }

    public DateOnly BorrowedDate { get; set; }

    public DateOnly DueDate { get; set; }

    public DateOnly? ReturnedDate { get; set; }

    public decimal FineAmount { get; set; }

    public string? Remarks { get; set; }

    public bool IsReturned { get; set; }


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public LibraryBookCopy LibraryBookCopy { get; set; }
        = default!;

    public Student Student { get; set; } = default!;
}
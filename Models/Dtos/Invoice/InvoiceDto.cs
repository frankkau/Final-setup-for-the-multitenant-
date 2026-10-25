namespace CouncilRevenueCollection.Models.Dtos.Invoice;

public class InvoiceDto
{
    public Guid InvoiceId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public Guid FeeStructureId { get; set; }

    public string FeeName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal Balance { get; set; }

    public DateOnly InvoiceDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public bool IsPaid { get; set; }

    public bool IsCancelled { get; set; }

    public string? Remarks { get; set; }
}
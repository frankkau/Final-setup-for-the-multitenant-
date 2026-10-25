namespace CouncilRevenueCollection.Models.Dtos.Invoice;

public class InvoiceListDto
{
    public Guid InvoiceId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public string FeeName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal Balance { get; set; }

    public DateOnly InvoiceDate { get; set; }

    public bool IsPaid { get; set; }
}
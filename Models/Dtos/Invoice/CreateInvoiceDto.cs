namespace CouncilRevenueCollection.Models.Dtos.Invoice;

public class CreateInvoiceDto
{
    public Guid StudentId { get; set; }

    public Guid FeeStructureId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly InvoiceDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public string? Remarks { get; set; }
}
namespace CouncilRevenueCollection.Models.Dtos.Invoice;

public class UpdateInvoiceDto
{
    public DateOnly? DueDate { get; set; }

    public string? Remarks { get; set; }
}
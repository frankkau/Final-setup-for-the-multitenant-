namespace CouncilRevenueCollection.Models.Dtos.FeePayment;

public class CreateFeePaymentDto
{
    public Guid StudentId { get; set; }

    public Guid FeeStructureId { get; set; }

    public decimal AmountPaid { get; set; }

    public DateOnly PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string? ReferenceNumber { get; set; }

    public string? Remarks { get; set; }
}
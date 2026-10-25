namespace CouncilRevenueCollection.Models.Dtos.FeePayment;

public class UpdateFeePaymentDto
{
    public decimal AmountPaid { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;
    public DateOnly PaymentDate { get; set; }

    public string PaymentReference { get; set; } = string.Empty;

    public string? Remarks { get; set; }
}
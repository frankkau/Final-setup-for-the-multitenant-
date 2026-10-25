namespace CouncilRevenueCollection.Models.Dtos.FeePayment;

public class FeePaymentListDto
{
    public Guid FeePaymentId { get; set; }

    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string FeeName { get; set; } = string.Empty;

    public decimal AmountPaid { get; set; }

    public DateOnly PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string? ReferenceNumber { get; set; }
}
namespace CouncilRevenueCollection.Models.Dtos.FeePayment;

public class FeePaymentDto
{
    public Guid FeePaymentId { get; set; }

    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public Guid FeeStructureId { get; set; }

    public string FeeName { get; set; } = string.Empty;

    public decimal AmountPaid { get; set; }

    public DateOnly PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string? ReferenceNumber { get; set; }

    public string? Remarks { get; set; }
}
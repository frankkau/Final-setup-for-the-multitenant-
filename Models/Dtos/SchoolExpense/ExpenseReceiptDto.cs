namespace CouncilRevenueCollection.Models.Dtos.SchoolExpense;

public class ExpenseReceiptDto
{
    public Guid ExpenseReceiptId { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateOnly ReceiptDate { get; set; }

    public string? ReceiptNumber { get; set; }

    public decimal? ReceiptAmount { get; set; }

    public DateTime UploadedAtUtc { get; set; }
}
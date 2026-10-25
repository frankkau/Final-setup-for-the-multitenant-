using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class ExpenseReceipt : IMustHaveTenant
{
    public Guid ExpenseReceiptId { get; set; }

    public string TenantId { get; set; } = default!;

    public Guid SchoolExpenseId { get; set; }


    // Original filename supplied by the user.
    // Used for display only.
    public string OriginalFileName { get; set; } = string.Empty;


    // Server-generated filename.
    // Never trust the original filename for storage.
    public string StoredFileName { get; set; } = string.Empty;


    // MIME type, e.g. application/pdf, image/jpeg.
    public string ContentType { get; set; } = string.Empty;


    // File size in bytes.
    public long FileSize { get; set; }


    // SHA-256 hash used to detect duplicate files.
    public string FileHash { get; set; } = string.Empty;


    // Date printed on the receipt.
    public DateOnly ReceiptDate { get; set; }


    // Receipt/invoice number printed on the document.
    public string? ReceiptNumber { get; set; }


    // Amount appearing on the receipt.
    public decimal? ReceiptAmount { get; set; }


    // Physical/object-storage location.
    public string? StoragePath { get; set; }


    // When the receipt was uploaded to the system.
    public DateTime UploadedAtUtc { get; set; }


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public SchoolExpense SchoolExpense { get; set; } = default!;
}
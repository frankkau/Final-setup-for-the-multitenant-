using Microsoft.AspNetCore.Http;

namespace CouncilRevenueCollection.Models.Dtos.ExpenseReceipt;

public class CreateExpenseReceiptDto
{
    public Guid SchoolExpenseId { get; set; }

    public DateOnly ReceiptDate { get; set; }

    public string? ReceiptNumber { get; set; }

    public decimal? ReceiptAmount { get; set; }

    public IFormFile Receipt { get; set; } = default!;
}
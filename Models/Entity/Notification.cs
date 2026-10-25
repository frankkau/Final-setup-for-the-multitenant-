using CouncilRevenueCollection.Common;
using CouncilRevenueCollection.Domain.Entities;

namespace CouncilRevenueCollection.Models.Entity;

public class Notification : IMustHaveTenant
{
    public Guid NotificationId { get; set; }

    public string TenantId { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    // Example: "Student", "FeePayment", "ExamResult"
    public string? NotificationType { get; set; }

    // Optional ID of the record the notification relates to.
    public Guid? ReferenceId { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? ReadAtUtc { get; set; }


    // Navigation properties

    public Tenant Tenant { get; set; } = default!;

    public User User { get; set; } = default!;
}
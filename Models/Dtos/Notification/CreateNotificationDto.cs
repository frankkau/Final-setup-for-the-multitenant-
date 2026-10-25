namespace CouncilRevenueCollection.Models.Dtos.Notification;

public class CreateNotificationDto
{
    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? NotificationType { get; set; }

    public Guid? ReferenceId { get; set; }
}
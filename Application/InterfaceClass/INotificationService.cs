using CouncilRevenueCollection.Models.Dtos.Notification;

namespace CouncilRevenueCollection.Application.InterfaceClass;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetMyNotificationsAsync(
        string userId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<NotificationDto>> GetUnreadAsync(
        string userId,
        CancellationToken cancellationToken);

    Task<NotificationDto?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken);

    Task<NotificationDto> CreateAsync(
        CreateNotificationDto dto,
        CancellationToken cancellationToken);

    Task MarkAsReadAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken);

    Task MarkAllAsReadAsync(
        string userId,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken);
}
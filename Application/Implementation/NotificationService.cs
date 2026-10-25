using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.Notification;
using CouncilRevenueCollection.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CouncilRevenueCollection.Application.Implementation;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        ApplicationDbContext context,
        ITenantService tenantService,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _tenantService = tenantService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<NotificationDto>>
        GetMyNotificationsAsync(
            string userId,
            CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new NotificationDto
            {
                NotificationId = x.NotificationId,

                Title = x.Title,

                Message = x.Message,

                NotificationType =
                    x.NotificationType,

                ReferenceId =
                    x.ReferenceId,

                IsRead =
                    x.IsRead,

                CreatedAtUtc =
                    x.CreatedAtUtc,

                ReadAtUtc =
                    x.ReadAtUtc
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<IReadOnlyList<NotificationDto>>
        GetUnreadAsync(
            string userId,
            CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Notifications
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                !x.IsRead)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new NotificationDto
            {
                NotificationId = x.NotificationId,

                Title = x.Title,

                Message = x.Message,

                NotificationType =
                    x.NotificationType,

                ReferenceId =
                    x.ReferenceId,

                IsRead =
                    x.IsRead,

                CreatedAtUtc =
                    x.CreatedAtUtc,

                ReadAtUtc =
                    x.ReadAtUtc
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<NotificationDto?>
        GetByIdAsync(
            Guid id,
            string userId,
            CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Notifications
            .AsNoTracking()
            .Where(x =>
                x.NotificationId == id &&
                x.UserId == userId)
            .Select(x => new NotificationDto
            {
                NotificationId = x.NotificationId,

                Title = x.Title,

                Message = x.Message,

                NotificationType =
                    x.NotificationType,

                ReferenceId =
                    x.ReferenceId,

                IsRead =
                    x.IsRead,

                CreatedAtUtc =
                    x.CreatedAtUtc,

                ReadAtUtc =
                    x.ReadAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<NotificationDto> CreateAsync(
        CreateNotificationDto dto,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();


        if (string.IsNullOrWhiteSpace(dto.UserId))
            throw new ArgumentException(
                "User ID is required.");


        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException(
                "Notification title is required.");


        if (string.IsNullOrWhiteSpace(dto.Message))
            throw new ArgumentException(
                "Notification message is required.");


        var userExists = await _context.Users
            .AnyAsync(
                x => x.Id == dto.UserId,
                cancellationToken);


        if (!userExists)
            throw new KeyNotFoundException(
                "User not found.");


        var notification = new Notification
        {
            NotificationId = Guid.NewGuid(),

            TenantId = tenantId,

            UserId = dto.UserId,

            Title = dto.Title.Trim(),

            Message = dto.Message.Trim(),

            NotificationType =
                dto.NotificationType?.Trim(),

            ReferenceId = dto.ReferenceId,

            IsRead = false,

            CreatedAtUtc = DateTime.UtcNow
        };


        await _context.Notifications.AddAsync(
            notification,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Notification {Id} created for user {UserId}",
            notification.NotificationId,
            notification.UserId);


        return new NotificationDto
        {
            NotificationId =
                notification.NotificationId,

            Title =
                notification.Title,

            Message =
                notification.Message,

            NotificationType =
                notification.NotificationType,

            ReferenceId =
                notification.ReferenceId,

            IsRead =
                notification.IsRead,

            CreatedAtUtc =
                notification.CreatedAtUtc,

            ReadAtUtc =
                notification.ReadAtUtc
        };
    }


    public async Task MarkAsReadAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var notification = await _context.Notifications
            .FirstOrDefaultAsync(
                x =>
                    x.NotificationId == id &&
                    x.UserId == userId,
                cancellationToken);


        if (notification is null)
            throw new KeyNotFoundException(
                "Notification not found.");


        if (notification.IsRead)
            return;


        notification.IsRead = true;

        notification.ReadAtUtc =
            DateTime.UtcNow;


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Notification {Id} marked as read",
            id);
    }


    public async Task MarkAllAsReadAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var notifications = await _context.Notifications
            .Where(x =>
                x.UserId == userId &&
                !x.IsRead)
            .ToListAsync(cancellationToken);


        if (notifications.Count == 0)
            return;


        var readAt = DateTime.UtcNow;


        foreach (var notification in notifications)
        {
            notification.IsRead = true;

            notification.ReadAtUtc = readAt;
        }


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Marked {Count} notifications as read for user {UserId}",
            notifications.Count,
            userId);
    }


    public async Task DeleteAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken)
    {
        GetTenantId();


        var notification = await _context.Notifications
            .FirstOrDefaultAsync(
                x =>
                    x.NotificationId == id &&
                    x.UserId == userId,
                cancellationToken);


        if (notification is null)
            throw new KeyNotFoundException(
                "Notification not found.");


        _context.Notifications.Remove(
            notification);


        await _context.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Notification {Id} deleted",
            id);
    }


    public async Task<bool> ExistsAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken)
    {
        GetTenantId();

        return await _context.Notifications
            .AsNoTracking()
            .AnyAsync(
                x =>
                    x.NotificationId == id &&
                    x.UserId == userId,
                cancellationToken);
    }


    private string GetTenantId()
    {
        var tenantId =
            _tenantService.GetCurrentTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException(
                "No tenant has been resolved.");

        return tenantId;
    }
}
using System.Security.Claims;
using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationController(
        INotificationService service)
    {
        _service = service;
    }


    [HttpGet]
    public async Task<IActionResult> GetMyNotifications(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var notifications =
            await _service.GetMyNotificationsAsync(
                userId,
                cancellationToken);

        return Ok(notifications);
    }


    [HttpGet("unread")]
    public async Task<IActionResult> GetUnread(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var notifications =
            await _service.GetUnreadAsync(
                userId,
                cancellationToken);

        return Ok(notifications);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var notification =
            await _service.GetByIdAsync(
                id,
                userId,
                cancellationToken);

        if (notification is null)
            return NotFound();

        return Ok(notification);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateNotificationDto dto,
        CancellationToken cancellationToken)
    {
        var notification =
            await _service.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = notification.NotificationId
            },
            notification);
    }


    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        await _service.MarkAsReadAsync(
            id,
            userId,
            cancellationToken);

        return NoContent();
    }


    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        await _service.MarkAllAsReadAsync(
            userId,
            cancellationToken);

        return NoContent();
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        await _service.DeleteAsync(
            id,
            userId,
            cancellationToken);

        return NoContent();
    }


    private string GetUserId()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException(
                "User identity was not found.");

        return userId;
    }
}
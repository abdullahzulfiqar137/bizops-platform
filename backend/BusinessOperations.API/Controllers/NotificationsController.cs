using BusinessOperations.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationService _notifications;
    public NotificationsController(INotificationService notifications) => _notifications = notifications;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken token) => Ok(await _notifications.GetAsync(token));

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> Read(Guid id, CancellationToken token)
    {
        var response = await _notifications.MarkReadAsync(id, token);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> ReadAll(CancellationToken token) => Ok(await _notifications.MarkAllReadAsync(token));
}

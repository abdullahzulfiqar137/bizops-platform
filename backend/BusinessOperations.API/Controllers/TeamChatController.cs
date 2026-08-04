using System.Security.Claims;
using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Chat;
using BusinessOperations.API.Hubs;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Controllers;

public sealed record ChatChannelDto(string Id, string Name, string Topic, int UnreadCount);
public sealed record DirectUserDto(string Id, string Name, string Role, string Status, string AvatarColor, int UnreadCount);
public sealed record ChatMessageDto(string Id, string TargetId, string SenderId, string SenderName, string SenderRole, string AvatarColor, DateTime TimestampUtc, string Content, bool IsSelf);
public sealed record SendMessageRequest(string TargetId, string Content);

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TeamChatController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IHubContext<NotificationHub> _hub;

    public TeamChatController(ApplicationDbContext context, ICurrentUserService currentUser, IHubContext<NotificationHub> hub)
    {
        _context = context;
        _currentUser = currentUser;
        _hub = hub;
    }

    [HttpGet("direct-users")]
    public async Task<IActionResult> GetDirectUsers(CancellationToken cancellationToken)
    {
        var employees = await _context.Employees.AsNoTracking()
            .Where(employee => employee.Status != "Terminated")
            .OrderBy(employee => employee.FirstName).ThenBy(employee => employee.LastName)
            .Select(employee => new DirectUserDto(
                employee.Id.ToString(), employee.FirstName + " " + employee.LastName,
                employee.Designation, employee.Status == "Active" ? "online" : "away",
                employee.AvatarColor, 0))
            .ToListAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<DirectUserDto>>.SuccessResponse(employees));
    }

    [HttpGet("messages/{targetId}")]
    public async Task<IActionResult> GetMessages(string targetId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(targetId, out var employeeId))
            return BadRequest(ApiResponse<IEnumerable<ChatMessageDto>>.FailureResponse("Invalid employee chat target."));

        var currentUserId = _currentUser.UserId;
        var messages = await _context.TeamChatMessages.AsNoTracking()
            .Where(message => message.TargetEmployeeId == employeeId)
            .OrderBy(message => message.TimestampUtc)
            .Select(message => new ChatMessageDto(
                message.Id.ToString(), message.TargetEmployeeId.ToString(),
                message.SenderUserId.HasValue ? message.SenderUserId.Value.ToString() : "system",
                message.SenderName, message.SenderRole, message.AvatarColor,
                message.TimestampUtc, message.Content, message.SenderUserId == currentUserId))
            .ToListAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<ChatMessageDto>>.SuccessResponse(messages));
    }

    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.TargetId, out var employeeId) ||
            !await _context.Employees.AnyAsync(employee => employee.Id == employeeId, cancellationToken))
            return BadRequest(ApiResponse<ChatMessageDto>.FailureResponse("Employee chat target was not found."));

        var content = request.Content.Trim();
        if (content.Length is < 1 or > 4000)
            return BadRequest(ApiResponse<ChatMessageDto>.FailureResponse("Message must contain between 1 and 4,000 characters."));

        var entity = new TeamChatMessage
        {
            TargetEmployeeId = employeeId,
            SenderUserId = _currentUser.UserId,
            SenderName = _currentUser.Username ?? _currentUser.Email ?? "Team member",
            SenderRole = User.FindFirstValue(ClaimTypes.Role) ?? "Staff",
            Content = content
        };
        _context.TeamChatMessages.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        var message = new ChatMessageDto(
            entity.Id.ToString(), employeeId.ToString(), entity.SenderUserId?.ToString() ?? "system",
            entity.SenderName, entity.SenderRole, entity.AvatarColor,
            entity.TimestampUtc, entity.Content, true);
        var broadcastMessage = message with { IsSelf = false };
        await _hub.Clients.Group("AllUsers").SendAsync("TeamChatMessage", broadcastMessage, cancellationToken);
        return Ok(ApiResponse<ChatMessageDto>.SuccessResponse(message, "Message sent successfully."));
    }
}

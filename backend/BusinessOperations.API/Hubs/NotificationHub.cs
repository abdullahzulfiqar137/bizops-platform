using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BusinessOperations.API.Hubs;

[Authorize]
public sealed class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.Identity?.Name ?? "Anonymous";
        await Groups.AddToGroupAsync(Context.ConnectionId, "AllUsers");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AllUsers");
        await base.OnDisconnectedAsync(exception);
    }
}

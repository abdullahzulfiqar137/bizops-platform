namespace BusinessOperations.API.DTOs.Notifications;

public sealed class NotificationResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Route { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public sealed class NotificationCenterResponse
{
    public int UnreadCount { get; set; }
    public IReadOnlyList<NotificationResponse> Notifications { get; set; } = [];
}

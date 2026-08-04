using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Chat;

public sealed class TeamChatMessage : BaseEntity
{
    public Guid TargetEmployeeId { get; set; }
    public Guid? SenderUserId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderRole { get; set; } = string.Empty;
    public string AvatarColor { get; set; } = "#2563eb";
    public string Content { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}

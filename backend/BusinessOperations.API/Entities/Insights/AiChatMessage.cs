using System.ComponentModel.DataAnnotations;
using BusinessOperations.API.Entities.Common;

namespace BusinessOperations.API.Entities.Insights;

public class AiChatMessage : BaseEntity
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    public string SessionId { get; set; } = string.Empty;

    [Required]
    public string UserPrompt { get; set; } = string.Empty;

    [Required]
    public string AiResponse { get; set; } = string.Empty;

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}

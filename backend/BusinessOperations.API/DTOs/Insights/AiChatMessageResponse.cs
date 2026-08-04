namespace BusinessOperations.API.DTOs.Insights;

public sealed class AiChatMessageResponse
{
    public Guid Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string UserPrompt { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string Sender { get; set; } = "BizOps AI Assistant";
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}

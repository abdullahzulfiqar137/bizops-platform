namespace BusinessOperations.API.DTOs.Insights;

public sealed class AiChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public string? SystemPrompt { get; set; }
    public bool? ThinkingEnabled { get; set; }
    public List<AiConversationMessage> History { get; set; } = [];
}

public sealed class AiConversationMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

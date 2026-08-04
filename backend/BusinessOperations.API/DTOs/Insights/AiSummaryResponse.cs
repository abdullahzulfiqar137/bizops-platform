namespace BusinessOperations.API.DTOs.Insights;

public sealed class AiSummaryResponse
{
    public string ExecutiveSummary { get; set; } = string.Empty;
    public List<string> KeyHighlights { get; set; } = new();
    public List<string> OperationalRecommendations { get; set; } = new();
    public List<string> RiskAlerts { get; set; } = new();
    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
}

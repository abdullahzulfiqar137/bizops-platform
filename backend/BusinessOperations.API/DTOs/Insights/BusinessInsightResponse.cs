using BusinessOperations.API.Enums;

namespace BusinessOperations.API.DTOs.Insights;

public sealed class BusinessInsightResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public InsightCategory Category { get; set; }
    public string CategoryDescription => Category.ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InsightSeverity Severity { get; set; }
    public string SeverityDescription => Severity.ToString();
    public string ActionRecommendation { get; set; } = string.Empty;
    public int ImpactScore { get; set; } // 1 to 10
    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
}

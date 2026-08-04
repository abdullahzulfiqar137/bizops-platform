export interface AiSummaryResponse {
  executiveSummary: string;
  keyHighlights: string[];
  operationalRecommendations: string[];
  riskAlerts: string[];
  generatedAtUtc: string;
}

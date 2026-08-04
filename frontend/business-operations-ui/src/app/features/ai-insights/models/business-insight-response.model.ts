import { InsightCategory } from '../../../core/enums/insight-category.enum';
import { InsightSeverity } from '../../../core/enums/insight-severity.enum';

export interface BusinessInsightResponse {
  id: string;
  category: InsightCategory;
  categoryDescription: string;
  title: string;
  description: string;
  severity: InsightSeverity;
  severityDescription: string;
  actionRecommendation: string;
  impactScore: number;
  generatedAtUtc: string;
}

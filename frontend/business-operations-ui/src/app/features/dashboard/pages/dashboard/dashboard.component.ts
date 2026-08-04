import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { ReportApiService } from '../../../../core/services/report-api.service';
import { AiInsightApiService } from '../../../../core/services/ai-insight-api.service';
import { TranslationService } from '../../../../core/services/translation.service';
import { DashboardSummaryResponse } from '../../../reports/models/dashboard-summary-response.model';
import { AiSummaryResponse } from '../../../ai-insights/models/ai-summary-response.model';
import { BusinessInsightResponse } from '../../../ai-insights/models/business-insight-response.model';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { BusinessCurrencyPipe } from '../../../../core/pipes/business-currency.pipe';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, BusinessCurrencyPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  reportApi = inject(ReportApiService);
  aiApi = inject(AiInsightApiService);
  ts = inject(TranslationService);
  route = inject(ActivatedRoute);
  summary: DashboardSummaryResponse | null = null;
  aiDigest: AiSummaryResponse | null = null;
  insights: BusinessInsightResponse[] = [];

  param: any;
  ngOnInit(): void {
    this.param = this.route.snapshot.queryParamMap.get('param');
    this.loadData();
  }

  loadData(): void {
    this.reportApi.getDashboardSummary().subscribe((res: ApiResponse<DashboardSummaryResponse>) => {
      if (res.success) this.summary = res.data;
    });

    this.aiApi.getDigest().subscribe((res: ApiResponse<AiSummaryResponse>) => {
      if (res.success) this.aiDigest = res.data;
    });

    this.aiApi.getInsights().subscribe((res: ApiResponse<BusinessInsightResponse[]>) => {
      if (res.success && res.data) this.insights = res.data;
    });
  }

  getSeverityBadgeClass(severity: number): string {
    if (severity === 3) return 'badge-danger';
    if (severity === 2) return 'badge-warning';
    return 'badge-info';
  }
}

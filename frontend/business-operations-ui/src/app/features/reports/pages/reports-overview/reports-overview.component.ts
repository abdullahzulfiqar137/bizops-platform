import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportApiService } from '../../../../core/services/report-api.service';
import { ProfitAndLossReportResponse } from '../../models/profit-and-loss-report-response.model';
import { SalesSummaryReportResponse } from '../../models/sales-summary-report-response.model';
import { ExpenseSummaryReportResponse } from '../../models/expense-summary-report-response.model';

@Component({
  selector: 'app-reports-overview',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports-overview.component.html',
  styleUrl: './reports-overview.component.css'
})
export class ReportsOverviewComponent implements OnInit {
  private reportApi = inject(ReportApiService);

  activeTab: 'pnl' | 'sales' | 'expenses' = 'pnl';

  pnl: ProfitAndLossReportResponse | null = null;
  salesSummary: SalesSummaryReportResponse | null = null;
  expenseSummary: ExpenseSummaryReportResponse | null = null;

  ngOnInit(): void {
    this.reportApi.getProfitAndLoss().subscribe(res => {
      if (res.success) this.pnl = res.data;
    });

    this.reportApi.getSalesSummary().subscribe(res => {
      if (res.success) this.salesSummary = res.data;
    });

    this.reportApi.getExpenseSummary().subscribe(res => {
      if (res.success) this.expenseSummary = res.data;
    });
  }
}

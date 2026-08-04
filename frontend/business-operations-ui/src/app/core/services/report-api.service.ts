import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { DashboardSummaryResponse } from '../../features/reports/models/dashboard-summary-response.model';
import { SalesSummaryReportResponse } from '../../features/reports/models/sales-summary-report-response.model';
import { PurchaseSummaryReportResponse } from '../../features/reports/models/purchase-summary-report-response.model';
import { ExpenseSummaryReportResponse } from '../../features/reports/models/expense-summary-report-response.model';
import { ProfitAndLossReportResponse } from '../../features/reports/models/profit-and-loss-report-response.model';
import { ProductResponse } from '../../features/products/models/product-response.model';

@Injectable({
  providedIn: 'root'
})
export class ReportApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/reports`;

  constructor(private http: HttpClient) {}

  getDashboardSummary(): Observable<ApiResponse<DashboardSummaryResponse>> {
    return this.http.get<ApiResponse<DashboardSummaryResponse>>(`${this.baseUrl}/dashboard`);
  }

  getSalesSummary(startDate?: string, endDate?: string): Observable<ApiResponse<SalesSummaryReportResponse>> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);

    return this.http.get<ApiResponse<SalesSummaryReportResponse>>(`${this.baseUrl}/sales-summary`, { params });
  }

  getPurchaseSummary(startDate?: string, endDate?: string): Observable<ApiResponse<PurchaseSummaryReportResponse>> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);

    return this.http.get<ApiResponse<PurchaseSummaryReportResponse>>(`${this.baseUrl}/purchase-summary`, { params });
  }

  getExpenseSummary(startDate?: string, endDate?: string): Observable<ApiResponse<ExpenseSummaryReportResponse>> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);

    return this.http.get<ApiResponse<ExpenseSummaryReportResponse>>(`${this.baseUrl}/expense-summary`, { params });
  }

  getProfitAndLoss(startDate?: string, endDate?: string): Observable<ApiResponse<ProfitAndLossReportResponse>> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);

    return this.http.get<ApiResponse<ProfitAndLossReportResponse>>(`${this.baseUrl}/profit-and-loss`, { params });
  }

  getLowStockReport(): Observable<ApiResponse<ProductResponse[]>> {
    return this.http.get<ApiResponse<ProductResponse[]>>(`${this.baseUrl}/low-stock`);
  }
}

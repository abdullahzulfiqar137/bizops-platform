export interface MonthlySalesSummary {
  year: number;
  month: number;
  monthName: string;
  invoiceCount: number;
  totalRevenue: number;
}

export interface SalesSummaryReportResponse {
  totalInvoices: number;
  grossSales: number;
  totalTaxAmount: number;
  totalDiscountAmount: number;
  netSalesRevenue: number;
  totalPaidAmount: number;
  totalDueAmount: number;
  averageOrderValue: number;
  monthlyBreakdown: MonthlySalesSummary[];
}

export interface PurchaseSummaryReportResponse {
  totalPurchaseOrders: number;
  grossPurchases: number;
  totalTaxAmount: number;
  totalDiscountAmount: number;
  netPurchaseSpend: number;
  totalPaidAmount: number;
  totalDueAmount: number;
}

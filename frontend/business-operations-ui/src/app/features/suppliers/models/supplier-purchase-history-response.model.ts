export interface SupplierPurchaseHistoryResponse {
  purchaseId: string;
  purchaseOrderNumber: string;
  totalAmount: number;
  paidAmount: number;
  remainingBalance: number;
  paymentStatus: string;
  purchaseDateUtc: string;
}

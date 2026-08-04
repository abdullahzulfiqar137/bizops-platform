import { PurchaseStatus } from '../../../core/enums/purchase-status.enum';
import { PaymentStatus } from '../../../core/enums/payment-status.enum';
import { PurchaseItemResponse } from './purchase-item-response.model';

export interface PurchaseResponse {
  id: string;
  purchaseOrderNumber: string;
  supplierId: string;
  supplierName: string;
  supplierCompanyName: string;
  supplierCode: string;
  purchaseDateUtc: string;
  subTotal: number;
  taxRate: number;
  taxAmount: number;
  discountAmount: number;
  totalAmount: number;
  paidAmount: number;
  dueAmount: number;
  status: PurchaseStatus;
  statusDescription: string;
  paymentStatus: PaymentStatus;
  paymentStatusDescription: string;
  notes: string | null;
  createdBy: string;
  createdAtUtc: string;
  items: PurchaseItemResponse[];
}

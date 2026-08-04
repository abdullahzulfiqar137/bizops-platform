import { SaleStatus } from '../../../core/enums/sale-status.enum';
import { PaymentStatus } from '../../../core/enums/payment-status.enum';
import { SaleItemResponse } from './sale-item-response.model';

export interface SaleResponse {
  id: string;
  invoiceNumber: string;
  customerId: string;
  customerName: string;
  customerCode: string;
  saleDateUtc: string;
  subTotal: number;
  taxRate: number;
  taxAmount: number;
  discountAmount: number;
  totalAmount: number;
  paidAmount: number;
  dueAmount: number;
  status: SaleStatus;
  statusDescription: string;
  paymentStatus: PaymentStatus;
  paymentStatusDescription: string;
  notes: string | null;
  createdBy: string;
  createdAtUtc: string;
  items: SaleItemResponse[];
}

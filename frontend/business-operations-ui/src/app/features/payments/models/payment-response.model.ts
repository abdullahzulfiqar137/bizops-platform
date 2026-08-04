import { PaymentType } from '../../../core/enums/payment-type.enum';
import { PaymentMethod } from '../../../core/enums/payment-method.enum';

export interface PaymentResponse {
  id: string;
  paymentNumber: string;
  paymentType: PaymentType;
  paymentTypeDescription: string;
  customerId: string | null;
  customerName: string | null;
  supplierId: string | null;
  supplierName: string | null;
  saleId: string | null;
  invoiceNumber: string | null;
  purchaseId: string | null;
  purchaseOrderNumber: string | null;
  amount: number;
  paymentDateUtc: string;
  paymentMethod: PaymentMethod;
  paymentMethodDescription: string;
  referenceNumber: string | null;
  notes: string | null;
  createdBy: string;
  createdAtUtc: string;
}

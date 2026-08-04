import { PaymentType } from '../../../core/enums/payment-type.enum';
import { PaymentMethod } from '../../../core/enums/payment-method.enum';

export interface CreatePaymentRequest {
  paymentType: PaymentType;
  customerId?: string | null;
  supplierId?: string | null;
  saleId?: string | null;
  purchaseId?: string | null;
  amount: number;
  paymentMethod: PaymentMethod;
  referenceNumber?: string | null;
  notes?: string | null;
}

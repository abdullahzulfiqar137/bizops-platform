import { PaymentMethod } from '../../../core/enums/payment-method.enum';

export interface CreateExpenseRequest {
  expenseCategoryId: string;
  title: string;
  amount: number;
  paymentMethod: PaymentMethod;
  referenceNumber?: string | null;
  notes?: string | null;
}

import { PaymentMethod } from '../../../core/enums/payment-method.enum';

export interface ExpenseResponse {
  id: string;
  expenseNumber: string;
  expenseCategoryId: string;
  expenseCategoryName: string;
  title: string;
  amount: number;
  expenseDateUtc: string;
  paymentMethod: PaymentMethod;
  paymentMethodDescription: string;
  referenceNumber: string | null;
  notes: string | null;
  createdBy: string;
  createdAtUtc: string;
}

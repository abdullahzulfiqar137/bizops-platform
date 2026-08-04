import { CreateSaleItemRequest } from './create-sale-item-request.model';

export interface CreateSaleRequest {
  customerId: string;
  taxRate?: number;
  discountAmount?: number;
  paidAmount?: number;
  notes?: string | null;
  items: CreateSaleItemRequest[];
}

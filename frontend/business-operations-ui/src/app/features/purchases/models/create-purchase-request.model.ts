import { CreatePurchaseItemRequest } from './create-purchase-item-request.model';

export interface CreatePurchaseRequest {
  supplierId: string;
  taxRate?: number;
  discountAmount?: number;
  paidAmount?: number;
  notes?: string | null;
  items: CreatePurchaseItemRequest[];
}

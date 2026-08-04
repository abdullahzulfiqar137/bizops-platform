import { InventoryAdjustmentType } from '../../../core/enums/inventory-adjustment-type.enum';

export interface InventoryAdjustmentResponse {
  id: string;
  adjustmentNumber: string;
  productId: string;
  productName: string;
  productSKU: string;
  adjustmentType: InventoryAdjustmentType;
  adjustmentTypeDescription: string;
  quantity: number;
  reason: string;
  notes: string | null;
  createdBy: string;
  createdAtUtc: string;
}

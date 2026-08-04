import { InventoryAdjustmentType } from '../../../core/enums/inventory-adjustment-type.enum';

export interface CreateInventoryAdjustmentRequest {
  productId: string;
  adjustmentType: InventoryAdjustmentType;
  quantity: number;
  reason: string;
  notes?: string | null;
}

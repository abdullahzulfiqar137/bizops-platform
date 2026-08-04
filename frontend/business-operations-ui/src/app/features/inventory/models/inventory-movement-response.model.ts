import { InventoryMovementType } from '../../../core/enums/inventory-movement-type.enum';

export interface InventoryMovementResponse {
  id: string;
  movementNumber: string;
  productId: string;
  productName: string;
  productSKU: string;
  movementType: InventoryMovementType;
  movementTypeDescription: string;
  quantity: number;
  previousStock: number;
  newStock: number;
  referenceType: string;
  referenceId: string;
  notes: string | null;
  createdAtUtc: string;
}

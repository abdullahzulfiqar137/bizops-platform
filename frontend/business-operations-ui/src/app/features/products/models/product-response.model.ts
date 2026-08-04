export interface ProductResponse {
  id: string;
  sku: string;
  barcode: string | null;
  name: string;
  description: string | null;
  categoryId: string;
  categoryName: string;
  unitId: string;
  unitName: string;
  unitAbbreviation: string;
  costPrice: number;
  salePrice: number;
  stockQuantity: number;
  minimumStockLevel: number;
  isLowStock: boolean;
  isActive: boolean;
  createdAtUtc: string;
}

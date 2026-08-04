export interface CreateProductRequest {
  name: string;
  barcode?: string | null;
  description?: string | null;
  categoryId: string;
  unitId: string;
  costPrice: number;
  salePrice: number;
  initialStockQuantity?: number;
  minimumStockLevel?: number;
}

export interface UpdateProductRequest {
  name: string;
  barcode?: string | null;
  description?: string | null;
  categoryId: string;
  unitId: string;
  costPrice: number;
  salePrice: number;
  minimumStockLevel?: number;
  isActive: boolean;
}

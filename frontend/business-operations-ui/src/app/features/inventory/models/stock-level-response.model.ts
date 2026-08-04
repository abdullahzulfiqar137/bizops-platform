export interface StockLevelResponse {
  productId: string;
  sku: string;
  productName: string;
  categoryName: string;
  unitAbbreviation: string;
  currentStock: number;
  minimumStockLevel: number;
  isLowStock: boolean;
}

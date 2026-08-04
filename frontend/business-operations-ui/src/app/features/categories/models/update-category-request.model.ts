export interface UpdateCategoryRequest {
  name: string;
  categoryType?: string | null;
  parentCategory?: string | null;
  description?: string | null;
  isActive: boolean;
}

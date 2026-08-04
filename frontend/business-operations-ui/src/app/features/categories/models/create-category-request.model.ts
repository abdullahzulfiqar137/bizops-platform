export interface CreateCategoryRequest {
  name: string;
  categoryType?: string | null;
  parentCategory?: string | null;
  description?: string | null;
}

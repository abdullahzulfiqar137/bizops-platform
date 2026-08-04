export interface CategoryResponse {
  id: string;
  name: string;
  categoryType?: string | null;
  parentCategory?: string | null;
  description: string | null;
  isActive: boolean;
  createdAtUtc: string;
}

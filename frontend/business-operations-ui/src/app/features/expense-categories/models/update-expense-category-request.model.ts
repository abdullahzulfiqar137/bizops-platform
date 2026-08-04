export interface UpdateExpenseCategoryRequest {
  name: string;
  description?: string | null;
  isActive: boolean;
}

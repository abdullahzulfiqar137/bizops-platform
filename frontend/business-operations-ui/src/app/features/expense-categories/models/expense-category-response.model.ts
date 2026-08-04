export interface ExpenseCategoryResponse {
  id: string;
  name: string;
  description: string | null;
  isActive: boolean;
  createdAtUtc: string;
}

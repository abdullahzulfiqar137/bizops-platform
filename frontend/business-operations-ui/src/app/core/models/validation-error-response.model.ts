export interface ValidationErrorResponse {
  success: boolean;
  message: string;
  errors: Record<string, string[]>;
}

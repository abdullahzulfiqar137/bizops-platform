export interface RegisterRequest {
  companyName: string;
  firstName: string;
  lastName: string;
  email: string;
  username: string;
  password: string;
  confirmPassword: string;
  phoneNumber?: string | null;
}

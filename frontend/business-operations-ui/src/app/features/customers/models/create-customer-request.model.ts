export interface CreateCustomerRequest {
  name: string;
  email: string;
  phoneNumber: string;
  address?: string | null;
  city?: string | null;
  state?: string | null;
  postalCode?: string | null;
  country?: string | null;
  openingBalance?: number;
}

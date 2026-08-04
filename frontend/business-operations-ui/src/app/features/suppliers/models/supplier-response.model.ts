export interface SupplierResponse {
  id: string;
  supplierCode: string;
  name: string;
  companyName: string;
  email: string;
  phoneNumber: string;
  address: string | null;
  city: string | null;
  state: string | null;
  postalCode: string | null;
  country: string | null;
  currentBalance: number;
  isActive: boolean;
  createdAtUtc: string;
}

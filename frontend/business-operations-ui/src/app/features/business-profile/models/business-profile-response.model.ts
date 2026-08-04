export interface BusinessProfileResponse {
  id: string;
  companyName: string;
  legalName: string;
  taxNumber: string | null;
  email: string;
  phoneNumber: string;
  address: string;
  city: string;
  state: string;
  postalCode: string;
  country: string;
  currencyCode: string;
  currencySymbol: string;
  taxRate: number;
  invoicePrefix: string;
  nextInvoiceNumber: number;
  logoUrl: string | null;
  updatedAtUtc: string;
}

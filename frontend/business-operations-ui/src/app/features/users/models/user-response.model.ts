export interface UserResponse {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  username: string;
  phoneNumber: string | null;
  isActive: boolean;
  roles: string[];
  createdAtUtc: string;
  tenantId?: string;
  tenantName?: string;
}

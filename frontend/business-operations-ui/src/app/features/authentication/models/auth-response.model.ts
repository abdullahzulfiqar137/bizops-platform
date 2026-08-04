import { UserResponse } from '../../users/models/user-response.model';

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
  user: UserResponse;
}

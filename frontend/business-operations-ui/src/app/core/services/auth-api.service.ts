import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { LoginRequest } from '../../features/authentication/models/login-request.model';
import { RegisterRequest } from '../../features/authentication/models/register-request.model';
import { RefreshTokenRequest } from '../../features/authentication/models/refresh-token-request.model';
import { RevokeTokenRequest } from '../../features/authentication/models/revoke-token-request.model';
import { AuthResponse } from '../../features/authentication/models/auth-response.model';
import { UserResponse } from '../../features/users/models/user-response.model';
import { TokenStorageService } from './token-storage.service';
import { AuthStateService } from '../state/auth-state.service';

@Injectable({
  providedIn: 'root'
})
export class AuthApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/auth`;

  constructor(
    private http: HttpClient,
    private tokenStorage: TokenStorageService,
    private authState: AuthStateService
  ) {}

  login(request: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}/login`, request, { withCredentials: true }).pipe(
      tap(res => {
        if (res.success && res.data) {
          this.tokenStorage.saveAccessToken(res.data.accessToken);
          this.authState.setUser(res.data.user);
        }
      })
    );
  }

  register(request: RegisterRequest): Observable<ApiResponse<UserResponse>> {
    return this.http.post<ApiResponse<UserResponse>>(`${this.baseUrl}/register`, request);
  }

  refreshToken(request: RefreshTokenRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}/refresh-token`, request, { withCredentials: true }).pipe(
      tap(res => {
        if (res.success && res.data) {
          this.tokenStorage.saveAccessToken(res.data.accessToken);
          this.authState.setUser(res.data.user);
        }
      })
    );
  }

  revokeToken(request: RevokeTokenRequest): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/revoke-token`, request, { withCredentials: true });
  }

  getCurrentUser(): Observable<ApiResponse<UserResponse>> {
    return this.http.get<ApiResponse<UserResponse>>(`${this.baseUrl}/me`).pipe(
      tap(res => {
        if (res.success && res.data) {
          this.authState.setUser(res.data);
        }
      })
    );
  }

  updateCurrentUser(request: { firstName: string; lastName: string; email: string; phoneNumber: string | null }): Observable<ApiResponse<UserResponse>> {
    return this.http.put<ApiResponse<UserResponse>>(`${this.baseUrl}/me`, request).pipe(
      tap(res => {
        if (res.success && res.data) this.authState.setUser(res.data);
      })
    );
  }

  forgotPassword(request: { email: string }): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/forgot-password`, request);
  }

  resetPassword(request: { email: string; token: string; newPassword: string; confirmPassword: string }): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/reset-password`, request);
  }

  getTwoFactorSetup(): Observable<ApiResponse<{ isEnabled: boolean; sharedKey: string; authenticatorUri: string }>> {
    return this.http.get<ApiResponse<{ isEnabled: boolean; sharedKey: string; authenticatorUri: string }>>(`${this.baseUrl}/security/two-factor`);
  }

  setTwoFactor(enabled: boolean, code: string): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/security/two-factor/${enabled ? 'enable' : 'disable'}`, { code });
  }

  getSessions(): Observable<ApiResponse<Array<{ id: string; ipAddress: string; device: string; createdAtUtc: string; expiresAtUtc: string }>>> {
    return this.http.get<ApiResponse<Array<{ id: string; ipAddress: string; device: string; createdAtUtc: string; expiresAtUtc: string }>>>(`${this.baseUrl}/security/sessions`);
  }

  revokeSession(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/security/sessions/${id}`);
  }

  revokeAllSessions(): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/security/sessions`);
  }

  logout(): void {
    this.revokeToken({ refreshToken: '' }).subscribe();
    this.tokenStorage.clearTokens();
    this.authState.setUser(null);
  }
}

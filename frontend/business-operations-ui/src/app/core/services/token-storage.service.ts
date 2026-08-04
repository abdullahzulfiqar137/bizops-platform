import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenStorageService {
  private readonly ACCESS_TOKEN_KEY = 'bo_access_token';
  private readonly REFRESH_TOKEN_KEY = 'bo_refresh_token';
  private readonly USER_KEY = 'bo_user_data';

  getAccessToken(): string | null {
    return sessionStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  saveAccessToken(token: string): void {
    sessionStorage.setItem(this.ACCESS_TOKEN_KEY, token);
  }

  getRefreshToken(): string | null {
    return null;
  }

  saveRefreshToken(token: string): void {
    // Refresh tokens are held only in a backend-issued HttpOnly cookie.
  }

  getUserData<T>(): T | null {
    const data = sessionStorage.getItem(this.USER_KEY);
    return data ? JSON.parse(data) : null;
  }

  saveUserData<T>(user: T): void {
    sessionStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  clearTokens(): void {
    sessionStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    sessionStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.USER_KEY);
  }
}

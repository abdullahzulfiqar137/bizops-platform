import { Injectable, signal, computed } from '@angular/core';
import { UserResponse } from '../../features/users/models/user-response.model';
import { TokenStorageService } from '../services/token-storage.service';

@Injectable({
  providedIn: 'root'
})
export class AuthStateService {
  readonly currentUser = signal<UserResponse | null>(null);

  readonly isAuthenticated = computed(() => !!this.currentUser());
  readonly userRoles = computed(() => this.currentUser()?.roles ?? []);

  constructor(private tokenStorage: TokenStorageService) {
    const savedUser = this.tokenStorage.getUserData<UserResponse>();
    if (savedUser && this.tokenStorage.getAccessToken()) {
      this.currentUser.set(savedUser);
    } else {
      this.tokenStorage.clearTokens();
    }
  }

  setUser(user: UserResponse | null): void {
    this.currentUser.set(user);
    if (user) {
      this.tokenStorage.saveUserData(user);
    } else {
      this.tokenStorage.clearTokens();
    }
  }

  hasRole(role: string): boolean {
    return this.userRoles().includes(role);
  }

  hasAnyRole(roles: string[]): boolean {
    const currentRoles = this.userRoles();
    return roles.some(r => currentRoles.includes(r));
  }
}

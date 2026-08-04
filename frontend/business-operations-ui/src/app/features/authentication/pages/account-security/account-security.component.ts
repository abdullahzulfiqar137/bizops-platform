import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthApiService } from '../../../../core/services/auth-api.service';

interface SecuritySession {
  id: string;
  ipAddress: string;
  device: string;
  createdAtUtc: string;
  expiresAtUtc: string;
}

@Component({
  selector: 'app-account-security',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './account-security.component.html',
  styleUrl: './account-security.component.css'
})
export class AccountSecurityComponent implements OnInit {
  private api = inject(AuthApiService);

  twoFactor: { isEnabled: boolean; sharedKey: string; authenticatorUri: string } | null = null;
  sessions: SecuritySession[] = [];
  code = '';
  revealKey = false;
  copied = false;
  error = '';

  get maskedKey(): string {
    if (!this.twoFactor?.sharedKey) return '';
    return `${this.twoFactor.sharedKey.slice(0, 4)} ${'•'.repeat(16)} ${this.twoFactor.sharedKey.slice(-4)}`;
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getTwoFactorSetup().subscribe(r => this.twoFactor = r.data);
    this.api.getSessions().subscribe(r => this.sessions = r.data ?? []);
  }

  toggle2fa(): void {
    this.error = '';
    this.api.setTwoFactor(!this.twoFactor?.isEnabled, this.code).subscribe({
      next: () => {
        this.code = '';
        this.revealKey = false;
        this.load();
      },
      error: e => this.error = e.error?.message || 'The verification code was not accepted.'
    });
  }

  copyKey(): void {
    if (!this.twoFactor?.sharedKey) return;
    navigator.clipboard.writeText(this.twoFactor.sharedKey).then(() => {
      this.copied = true;
      setTimeout(() => this.copied = false, 1800);
    });
  }

  deviceName(value: string): string {
    if (!value) return 'Unknown device';
    const browser = value.includes('Edg/') ? 'Microsoft Edge' :
      value.includes('Chrome/') ? 'Google Chrome' :
      value.includes('Firefox/') ? 'Mozilla Firefox' :
      value.includes('Safari/') ? 'Safari' : 'Web browser';
    const os = value.includes('Windows') ? 'Windows' :
      value.includes('Mac OS') ? 'macOS' :
      value.includes('Android') ? 'Android' :
      value.includes('iPhone') || value.includes('iPad') ? 'iOS' : 'Unknown OS';
    return `${browser} on ${os}`;
  }

  revoke(id: string): void {
    this.api.revokeSession(id).subscribe(() => this.load());
  }

  revokeAll(): void {
    if (confirm('Sign out every active device? You may need to sign in again.')) {
      this.api.revokeAllSessions().subscribe(() => this.load());
    }
  }
}

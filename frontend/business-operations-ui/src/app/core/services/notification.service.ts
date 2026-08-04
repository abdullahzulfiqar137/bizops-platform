import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  success(message: string): void {
    console.log('[SUCCESS]', message);
  }

  error(message: string): void {
    console.error('[ERROR]', message);
  }

  info(message: string): void {
    console.info('[INFO]', message);
  }

  warning(message: string): void {
    console.warn('[WARN]', message);
  }

  showSuccess(message: string): void {
    this.success(message);
  }

  showError(message: string): void {
    this.error(message);
  }
}

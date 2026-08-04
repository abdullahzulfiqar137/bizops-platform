import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BusinessCurrencyService {
  currentCurrency = signal<string>(localStorage.getItem('bizops_currency_symbol') || '$');
  currencyCode = signal<string>(localStorage.getItem('bizops_currency_code') || 'USD');

  setCurrency(code: string, symbol?: string): void {
    const normalized = (code || 'USD').trim().toUpperCase();
    this.currencyCode.set(normalized);
    this.currentCurrency.set(symbol || normalized);
    localStorage.setItem('bizops_currency_code', normalized);
    localStorage.setItem('bizops_currency_symbol', symbol || normalized);
  }

  format(value: number | string | null | undefined): string {
    const num = value === null || value === undefined || isNaN(Number(value)) ? 0 : Number(value);
    try {
      return new Intl.NumberFormat(navigator.language || 'en-US', {
        style: 'currency',
        currency: this.currencyCode()
      }).format(num);
    } catch {
      return `${this.currentCurrency()}${num.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
    }
  }
}

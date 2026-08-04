import { Injectable } from '@angular/core';

export interface ExchangeRate {
  code: string;
  name: string;
  symbol: string;
  rateToUsd: number; // 1 USD = rateToUsd of this currency
}

@Injectable({
  providedIn: 'root'
})
export class CurrencyConverterService {
  private readonly rates: ExchangeRate[] = [
    { code: 'USD', name: 'US Dollar', symbol: '$', rateToUsd: 1.0 },
    { code: 'EUR', name: 'Euro', symbol: '€', rateToUsd: 0.92 },
    { code: 'GBP', name: 'British Pound', symbol: '£', rateToUsd: 0.79 },
    { code: 'AED', name: 'UAE Dirham', symbol: 'AED ', rateToUsd: 3.67 },
    { code: 'SAR', name: 'Saudi Riyal', symbol: 'SAR ', rateToUsd: 3.75 },
    { code: 'PKR', name: 'Pakistani Rupee', symbol: 'Rs ', rateToUsd: 278.50 },
    { code: 'CAD', name: 'Canadian Dollar', symbol: 'CA$', rateToUsd: 1.36 },
    { code: 'AUD', name: 'Australian Dollar', symbol: 'A$', rateToUsd: 1.52 },
    { code: 'JPY', name: 'Japanese Yen', symbol: '¥', rateToUsd: 154.20 },
    { code: 'CNY', name: 'Chinese Yuan', symbol: 'CN¥', rateToUsd: 7.23 }
  ];

  getRates(): ExchangeRate[] {
    return this.rates;
  }

  convert(amount: number, fromCode: string, toCode: string): number {
    const fromRate = this.rates.find(r => r.code === fromCode)?.rateToUsd || 1.0;
    const toRate = this.rates.find(r => r.code === toCode)?.rateToUsd || 1.0;

    // Convert from source currency to USD, then to target currency
    const amountInUsd = amount / fromRate;
    return amountInUsd * toRate;
  }
}

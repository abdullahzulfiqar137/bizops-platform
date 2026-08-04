import { Pipe, PipeTransform, inject } from '@angular/core';
import { BusinessCurrencyService } from '../services/business-currency.service';

@Pipe({
  name: 'businessCurrency',
  standalone: true
})
export class BusinessCurrencyPipe implements PipeTransform {
  private currencyService = inject(BusinessCurrencyService);

  transform(value: number | string | null | undefined, currencySymbol?: string): string {
    if (!currencySymbol) return this.currencyService.format(value);
    if (value === null || value === undefined || isNaN(Number(value))) {
      return `${currencySymbol || this.currencyService.currentCurrency()}0.00`;
    }
    const num = Number(value);
    const symbol = currencySymbol || this.currencyService.currentCurrency();
    return `${symbol}${num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
  }
}

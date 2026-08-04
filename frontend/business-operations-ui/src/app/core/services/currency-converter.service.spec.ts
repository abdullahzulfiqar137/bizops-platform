import { CurrencyConverterService } from './currency-converter.service';

describe('CurrencyConverterService', () => {
  let service: CurrencyConverterService;

  beforeEach(() => {
    service = new CurrencyConverterService();
  });

  it('keeps USD unchanged', () => {
    expect(service.convert(125.5, 'USD', 'USD')).toBe(125.5);
  });

  it('converts through the USD base rate', () => {
    expect(service.convert(100, 'EUR', 'GBP')).toBeCloseTo(85.87, 2);
  });

  it('exposes each supported currency code only once', () => {
    const codes = service.getRates().map(rate => rate.code);
    expect(new Set(codes).size).toBe(codes.length);
  });
});

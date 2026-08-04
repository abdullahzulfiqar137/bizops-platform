export interface CurrencyOption {
  code: string;
  name: string;
  symbol: string;
}

const ISO_4217_FALLBACK = `AED AFN ALL AMD ANG AOA ARS AUD AWG AZN BAM BBD BDT BGN BHD BIF BMD BND BOB BOV BRL BSD BTN BWP BYN BZD CAD CDF CHE CHF CHW CLF CLP CNY COP COU CRC CUC CUP CVE CZK DJF DKK DOP DZD EGP ERN ETB EUR FJD FKP GBP GEL GHS GIP GMD GNF GTQ GYD HKD HNL HRK HTG HUF IDR ILS INR IQD IRR ISK JMD JOD JPY KES KGS KHR KMF KPW KRW KWD KYD KZT LAK LBP LKR LRD LSL LYD MAD MDL MGA MKD MMK MNT MOP MRU MUR MVR MWK MXN MXV MYR MZN NAD NGN NIO NOK NPR NZD OMR PAB PEN PGK PHP PKR PLN PYG QAR RON RSD RUB RWF SAR SBD SCR SDG SEK SGD SHP SLE SLL SOS SRD SSP STN SVC SYP SZL THB TJS TMT TND TOP TRY TTD TWD TZS UAH UGX USD USN UYI UYU UYW UZS VED VES VND VUV WST XAF XAG XAU XBA XBB XBC XBD XCD XCG XDR XOF XPD XPF XPT XSU XTS XUA XXX YER ZAR ZMW ZWG`;

type IntlWithSupportedValues = typeof Intl & {
  supportedValuesOf?: (key: 'currency') => string[];
};

export function getCurrencyOptions(): CurrencyOption[] {
  const runtimeCodes = (Intl as IntlWithSupportedValues).supportedValuesOf?.('currency');
  const codes = runtimeCodes?.length ? runtimeCodes : ISO_4217_FALLBACK.split(' ');
  const displayNames = new Intl.DisplayNames(['en'], { type: 'currency' });

  return [...new Set(codes)].sort().map(code => {
    const parts = new Intl.NumberFormat('en', {
      style: 'currency',
      currency: code,
      currencyDisplay: 'narrowSymbol'
    }).formatToParts(0);

    return {
      code,
      name: displayNames.of(code) || code,
      symbol: parts.find(part => part.type === 'currency')?.value || code
    };
  });
}

import { Component, OnInit, inject, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder, Validators } from '@angular/forms';
import { BusinessProfileApiService } from '../../../../core/services/business-profile-api.service';
import { BusinessProfileResponse } from '../../models/business-profile-response.model';
import { BusinessCurrencyService } from '../../../../core/services/business-currency.service';
import { CurrencyOption, getCurrencyOptions } from '../../../../core/constants/currencies.constant';
import { COUNTRY_DATA, CountryLocation } from '../../../../core/constants/countries.constant';

@Component({
  selector: 'app-business-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './business-profile.component.html',
  styleUrl: './business-profile.component.css'
})
export class BusinessProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private profileApi = inject(BusinessProfileApiService);
  private currency = inject(BusinessCurrencyService);

  loading = false;
  successMessage = '';
  errorMessage = '';
  activeTab: 'all' | 'identity' | 'location' | 'financial' = 'all';

  readonly currencies: CurrencyOption[] = getCurrencyOptions();
  readonly countries: CountryLocation[] = COUNTRY_DATA;

  selectedCountry: CountryLocation | null = null;
  availableCities: string[] = [];
  availableStates: string[] = [];
  phoneDialCode = '+1';
  postalCodeHint = 'e.g. 94107 or 10001';
  todayDate = new Date().toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });

  // Custom Dropdown state
  countryDropdownOpen = false;
  phoneDropdownOpen = false;
  countrySearchQuery = '';

  profileForm = this.fb.group({
    companyName: ['', Validators.required],
    legalName: ['', Validators.required],
    taxNumber: [''],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', Validators.required],
    address: ['', Validators.required],
    city: ['', Validators.required],
    state: ['', Validators.required],
    postalCode: ['', Validators.required],
    country: ['United States', Validators.required],
    currencyCode: ['USD', Validators.required],
    currencySymbol: ['$', Validators.required],
    taxRate: [0, [Validators.required, Validators.min(0)]],
    invoicePrefix: ['INV', Validators.required],
    nextInvoiceNumber: [1001, [Validators.required, Validators.min(1)]],
    logoUrl: ['']
  });

  ngOnInit(): void {
    // Listen to currency changes
    this.profileForm.controls.currencyCode.valueChanges.subscribe(code => {
      const selected = this.currencies.find(currency => currency.code === code);
      if (selected) this.profileForm.controls.currencySymbol.setValue(selected.symbol, { emitEvent: false });
    });

    // Listen to country changes
    this.profileForm.controls.country.valueChanges.subscribe(countryName => {
      if (countryName) {
        this.onCountryChange(countryName);
      }
    });

    this.loadProfile();
  }

  toggleCountryDropdown(event: MouseEvent): void {
    event.stopPropagation();
    this.countryDropdownOpen = !this.countryDropdownOpen;
    this.phoneDropdownOpen = false;
  }

  togglePhoneDropdown(event: MouseEvent): void {
    event.stopPropagation();
    this.phoneDropdownOpen = !this.phoneDropdownOpen;
    this.countryDropdownOpen = false;
  }

  getFilteredCountries(): CountryLocation[] {
    if (!this.countrySearchQuery.trim()) {
      return this.countries;
    }
    const q = this.countrySearchQuery.toLowerCase();
    return this.countries.filter(c => c.name.toLowerCase().includes(q) || c.dialCode.includes(q));
  }

  selectCountryItem(country: CountryLocation): void {
    this.profileForm.controls.country.setValue(country.name);
    this.onCountryChange(country.name);
    this.countryDropdownOpen = false;
    this.countrySearchQuery = '';
  }

  selectPhoneDialCode(country: CountryLocation): void {
    this.phoneDialCode = country.dialCode;
    this.phoneDropdownOpen = false;
  }

  getSelectedDialCountry(): CountryLocation | undefined {
    return this.countries.find(c => c.dialCode === this.phoneDialCode) || this.selectedCountry || undefined;
  }

  @HostListener('document:click')
  closeCustomDropdowns(): void {
    this.countryDropdownOpen = false;
    this.phoneDropdownOpen = false;
  }

  onLogoUpload(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        this.profileForm.patchValue({ logoUrl: reader.result as string });
      };
      reader.readAsDataURL(file);
    }
  }

  onCountryChange(countryName: string): void {
    const found = this.countries.find(c => c.name.toLowerCase() === countryName.toLowerCase());
    if (found) {
      this.selectedCountry = found;
      this.availableCities = found.cities;
      this.availableStates = found.states;
      this.phoneDialCode = found.dialCode;
      this.postalCodeHint = found.postalFormatHint;
    } else {
      this.selectedCountry = null;
      this.availableCities = [];
      this.availableStates = [];
    }
  }

  getInitials(name: string): string {
    if (!name) return 'BO';
    const parts = name.trim().split(' ');
    if (parts.length >= 2) {
      return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
    }
    return name.substring(0, 2).toUpperCase();
  }

  getCalculatedTaxAmount(): string {
    const rate = this.profileForm.value.taxRate || 0;
    return (1250 * rate / 100).toFixed(2);
  }

  getCalculatedGrandTotal(): string {
    const tax = 1250 * (this.profileForm.value.taxRate || 0) / 100;
    return (1250 + tax).toFixed(2);
  }

  loadProfile(): void {
    this.profileApi.getProfile().subscribe(res => {
      if (res.success && res.data) {
        this.profileForm.patchValue(res.data);
        if (res.data.country) {
          this.onCountryChange(res.data.country);
        }
        this.currency.setCurrency(res.data.currencyCode, res.data.currencySymbol);
      }
    });
  }

  onSubmit(): void {
    if (this.profileForm.invalid) return;

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    const val = this.profileForm.value;
    this.profileApi.updateProfile({
      companyName: val.companyName!,
      legalName: val.legalName!,
      taxNumber: val.taxNumber || null,
      email: val.email!,
      phoneNumber: val.phoneNumber!,
      address: val.address!,
      city: val.city!,
      state: val.state!,
      postalCode: val.postalCode!,
      country: val.country!,
      currencyCode: val.currencyCode!,
      currencySymbol: val.currencySymbol!,
      taxRate: val.taxRate!,
      invoicePrefix: val.invoicePrefix!,
      nextInvoiceNumber: val.nextInvoiceNumber!,
      logoUrl: val.logoUrl || null
    }).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.success) {
          this.currency.setCurrency(val.currencyCode!, val.currencySymbol!);
          this.successMessage = 'Business profile settings updated successfully!';
        } else {
          this.errorMessage = res.message || 'Update failed.';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Failed to update business profile.';
      }
    });
  }
}

import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { finalize } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api-config';

@Component({
  selector: 'app-import-center',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './import-center.component.html',
  styleUrl: './import-center.component.css'
})
export class ImportCenterComponent {
  private http = inject(HttpClient);
  base = `${API_CONFIG.baseUrl}/imports`;
  busy = '';
  files: Record<string, File | undefined> = {};
  results: Record<string, string> = {};
  errors: Record<string, boolean> = {};

  types = [
    { key: 'customers', label: 'Customers', code: 'CU', copy: 'Customer contacts, addresses, and opening balances.' },
    { key: 'suppliers', label: 'Suppliers', code: 'SU', copy: 'Vendor details and opening balances.' },
    { key: 'products', label: 'Products', code: 'PR', copy: 'Catalog, pricing, units, and opening stock.' }
  ];

  select(k: string, e: Event): void {
    const f = (e.target as HTMLInputElement).files?.[0];
    this.results[k] = '';
    this.errors[k] = false;

    if (f && (!f.name.toLowerCase().endsWith('.csv') || f.size > 5 * 1024 * 1024)) {
      this.errors[k] = true;
      this.results[k] = 'Select a valid CSV file no larger than 5 MB.';
      this.files[k] = undefined;
      return;
    }
    this.files[k] = f;
  }

  upload(k: string): void {
    const f = this.files[k];
    if (!f) return;

    const body = new FormData();
    body.append('file', f);
    this.busy = k;
    this.results[k] = '';

    this.http.post<any>(`${this.base}/${k}`, body)
      .pipe(finalize(() => this.busy = ''))
      .subscribe({
        next: r => {
          this.errors[k] = false;
          this.results[k] = r.message || 'Import completed.';
        },
        error: e => {
          this.errors[k] = true;
          this.results[k] = e.error?.message || 'Import failed.';
        }
      });
  }
}

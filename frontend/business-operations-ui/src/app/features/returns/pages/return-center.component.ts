import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG } from '../../../core/config/api-config';

@Component({
  selector: 'app-return-center',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './return-center.component.html',
  styleUrl: './return-center.component.css'
})
export class ReturnCenterComponent {
  private http = inject(HttpClient);
  private api = API_CONFIG.baseUrl;
  
  type: 'sales' | 'purchases' = 'sales';
  recordId = '';
  record: any = null;
  quantities: Record<string, number> = {};
  reason = '';
  notes = '';
  message = '';

  setType(t: 'sales' | 'purchases'): void {
    this.type = t;
    this.record = null;
    this.message = '';
  }

  load(): void {
    if (!this.recordId) return;
    this.http.get<any>(`${this.api}/${this.type}/${this.recordId}`).subscribe({
      next: r => {
        this.record = r.data;
        this.quantities = {};
      },
      error: e => this.message = e.error?.message || 'Record not found.'
    });
  }

  submit(): void {
    const items = (this.record?.items ?? [])
      .filter((x: any) => +this.quantities[x.id] > 0)
      .map((x: any) => this.type === 'sales'
        ? { saleItemId: x.id, quantity: +this.quantities[x.id], restock: true }
        : { purchaseItemId: x.id, quantity: +this.quantities[x.id] }
      );

    if (!this.reason || !items.length) {
      this.message = 'Enter a reason and at least one return quantity.';
      return;
    }

    this.http.post<any>(`${this.api}/${this.type}/${this.record.id}/returns`, {
      reason: this.reason,
      notes: this.notes || null,
      items
    }).subscribe({
      next: r => this.message = r.message || 'Return processed.',
      error: e => this.message = e.error?.message || 'Return failed.'
    });
  }
}

import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { InventoryApiService } from '../../../../core/services/inventory-api.service';
import { ProductApiService } from '../../../../core/services/product-api.service';
import { InventoryAdjustmentResponse } from '../../models/inventory-adjustment-response.model';
import { InventoryMovementResponse } from '../../models/inventory-movement-response.model';
import { ProductResponse } from '../../../products/models/product-response.model';
import { InventoryAdjustmentType } from '../../../../core/enums/inventory-adjustment-type.enum';

@Component({
  selector: 'app-inventory-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './inventory-list.component.html',
  styleUrl: './inventory-list.component.css'
})
export class InventoryListComponent implements OnInit {
  private inventoryApi = inject(InventoryApiService);
  private productApi = inject(ProductApiService);
  private fb = inject(FormBuilder);

  activeTab: 'adjustments' | 'movements' = 'adjustments';

  adjustments: InventoryAdjustmentResponse[] = [];
  movements: InventoryMovementResponse[] = [];
  products: ProductResponse[] = [];

  showModal = false;

  adjForm = this.fb.group({
    productId: ['', Validators.required],
    adjustmentType: [InventoryAdjustmentType.Addition, Validators.required],
    quantity: [1, [Validators.required, Validators.min(0.01)]],
    reason: ['', Validators.required],
    notes: ['']
  });

  reasonPresets = [
    { text: 'Damaged Stock / Broken Package', icon: '📦' },
    { text: 'Expired Product Removal', icon: '⏳' },
    { text: 'Stock Audit Discrepancy Correction', icon: '📋' },
    { text: 'Internal Business & Demo Use', icon: '🏪' },
    { text: 'Returned Supplier Replacement', icon: '🔄' }
  ];

  selectReasonPreset(text: string): void {
    this.adjForm.patchValue({ reason: text });
  }

  ngOnInit(): void {
    this.loadAdjustments();
    this.loadMovements();
    this.loadProducts();
  }

  setTab(tab: 'adjustments' | 'movements'): void {
    this.activeTab = tab;
  }

  loadAdjustments(): void {
    this.inventoryApi.getAdjustments().subscribe(res => {
      if (res.success) this.adjustments = res.data;
    });
  }

  loadMovements(): void {
    this.inventoryApi.getMovements().subscribe(res => {
      if (res.success) this.movements = res.data;
    });
  }

  loadProducts(): void {
    this.productApi.getProducts(undefined, undefined, true, undefined, undefined, false, 1, 100).subscribe(res => {
      if (res.success) this.products = res.data;
    });
  }

  openAdjustmentModal(): void {
    this.adjForm.reset({ adjustmentType: InventoryAdjustmentType.Addition, quantity: 1 });
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
  }

  saveAdjustment(): void {
    if (this.adjForm.invalid) return;

    const val = this.adjForm.value;
    this.inventoryApi.createAdjustment({
      productId: val.productId!,
      adjustmentType: val.adjustmentType!,
      quantity: val.quantity!,
      reason: val.reason!,
      notes: val.notes || null
    }).subscribe(res => {
      if (res.success) {
        this.closeModal();
        this.loadAdjustments();
        this.loadMovements();
      }
    });
  }
}

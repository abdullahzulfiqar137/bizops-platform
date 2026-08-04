import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormArray, Validators } from '@angular/forms';
import { PurchaseApiService } from '../../../../core/services/purchase-api.service';
import { SupplierApiService } from '../../../../core/services/supplier-api.service';
import { ProductApiService } from '../../../../core/services/product-api.service';
import { PurchaseResponse } from '../../models/purchase-response.model';
import { SupplierResponse } from '../../../suppliers/models/supplier-response.model';
import { ProductResponse } from '../../../products/models/product-response.model';

@Component({
  selector: 'app-purchases-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './purchases-list.component.html',
  styleUrl: './purchases-list.component.css'
})
export class PurchasesListComponent implements OnInit {
  private purchaseApi = inject(PurchaseApiService);
  private supplierApi = inject(SupplierApiService);
  private productApi = inject(ProductApiService);
  private fb = inject(FormBuilder);

  purchases: PurchaseResponse[] = [];
  suppliers: SupplierResponse[] = [];
  products: ProductResponse[] = [];

  searchQuery = '';
  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  showCreateModal = false;
  selectedPO: PurchaseResponse | null = null;

  calculatedSubtotal = 0;
  calculatedTax = 0;
  calculatedTotal = 0;

  poForm = this.fb.group({
    supplierId: ['', Validators.required],
    taxRate: [0],
    discountAmount: [0],
    paidAmount: [0],
    items: this.fb.array([])
  });

  taxPresets = [
    { label: '0% Exempt', value: 0 },
    { label: '5% Standard VAT', value: 5 },
    { label: '10% Sales Tax', value: 10 },
    { label: '17% GST', value: 17 }
  ];

  setTaxPreset(rate: number): void {
    this.poForm.patchValue({ taxRate: rate });
    this.calculateTotals();
  }

  setDiscountPreset(discount: number): void {
    this.poForm.patchValue({ discountAmount: discount });
    this.calculateTotals();
  }

  setPaymentPreset(type: 'full' | 'half' | 'none'): void {
    if (type === 'full') {
      this.poForm.patchValue({ paidAmount: this.calculatedTotal });
    } else if (type === 'half') {
      this.poForm.patchValue({ paidAmount: Math.round((this.calculatedTotal / 2) * 100) / 100 });
    } else {
      this.poForm.patchValue({ paidAmount: 0 });
    }
  }

  setSupplierPreset(id: string): void {
    this.poForm.patchValue({ supplierId: id });
  }

  setProductPreset(index: number, productId: string): void {
    const itemGroup = this.itemsArray.at(index);
    if (!itemGroup) return;
    itemGroup.patchValue({ productId });
    this.onProductSelect(index);
  }

  setQtyPreset(index: number, qty: number): void {
    const itemGroup = this.itemsArray.at(index);
    if (!itemGroup) return;
    itemGroup.patchValue({ quantity: qty });
    this.calculateTotals();
  }

  get itemsArray(): FormArray {
    return this.poForm.get('items') as FormArray;
  }

  ngOnInit(): void {
    this.loadPurchases();
    this.loadSuppliers();
    this.loadProducts();
  }

  loadPurchases(): void {
    this.purchaseApi.getPurchases(this.searchQuery, undefined, undefined, undefined, undefined, undefined, undefined, false, this.currentPage, this.pageSize)
      .subscribe(res => {
        if (res.success) {
          this.purchases = res.data;
          this.currentPage = res.metadata.currentPage;
          this.totalPages = res.metadata.totalPages;
          this.totalCount = res.metadata.totalCount;
        }
      });
  }

  loadSuppliers(): void {
    this.supplierApi.getSuppliers(undefined, undefined, true, undefined, false, 1, 100).subscribe(res => {
      if (res.success) this.suppliers = res.data;
    });
  }

  loadProducts(): void {
    this.productApi.getProducts(undefined, undefined, true, undefined, undefined, false, 1, 100).subscribe(res => {
      if (res.success) this.products = res.data;
    });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadPurchases();
    }
  }

  openCreateModal(): void {
    this.poForm.reset({ taxRate: 0, discountAmount: 0, paidAmount: 0 });
    this.itemsArray.clear();
    this.addItem();
    this.showCreateModal = true;
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  addItem(): void {
    const itemGroup = this.fb.group({
      productId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitCost: [0, [Validators.required, Validators.min(0)]]
    });
    this.itemsArray.push(itemGroup);
  }

  removeItem(index: number): void {
    this.itemsArray.removeAt(index);
    this.calculateTotals();
  }

  onProductSelect(index: number): void {
    const item = this.itemsArray.at(index);
    const prodId = item.value.productId;
    const prod = this.products.find(p => p.id === prodId);
    if (prod) {
      item.patchValue({ unitCost: prod.costPrice });
    }
    this.calculateTotals();
  }

  calculateTotals(): void {
    let sub = 0;
    for (let i = 0; i < this.itemsArray.length; i++) {
      const val = this.itemsArray.at(i).value;
      sub += (val.quantity || 0) * (val.unitCost || 0);
    }
    this.calculatedSubtotal = sub;

    const taxRate = this.poForm.value.taxRate || 0;
    const discount = this.poForm.value.discountAmount || 0;

    this.calculatedTax = (sub * taxRate) / 100;
    this.calculatedTotal = sub + this.calculatedTax - discount;
  }

  savePurchase(): void {
    if (this.poForm.invalid) return;

    const val = this.poForm.value;
    this.purchaseApi.createPurchase({
      supplierId: val.supplierId!,
      taxRate: val.taxRate || 0,
      discountAmount: val.discountAmount || 0,
      paidAmount: val.paidAmount || 0,
      items: val.items as any
    }).subscribe(res => {
      if (res.success) {
        this.closeCreateModal();
        this.loadPurchases();
      }
    });
  }

  viewDetails(po: PurchaseResponse): void {
    this.selectedPO = po;
  }

  closeDetailsModal(): void {
    this.selectedPO = null;
  }

  cancelPurchase(id: string): void {
    if (confirm('Are you sure you want to cancel this purchase order? Product stock and supplier balance will be reverted.')) {
      this.purchaseApi.cancelPurchase(id).subscribe(res => {
        if (res.success) {
          this.loadPurchases();
        }
      });
    }
  }
}

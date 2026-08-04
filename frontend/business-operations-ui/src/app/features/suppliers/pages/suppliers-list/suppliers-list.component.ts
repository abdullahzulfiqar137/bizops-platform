import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { SupplierApiService } from '../../../../core/services/supplier-api.service';
import { SupplierResponse } from '../../models/supplier-response.model';
import { SupplierPurchaseHistoryResponse } from '../../models/supplier-purchase-history-response.model';

@Component({
  selector: 'app-suppliers-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './suppliers-list.component.html',
  styleUrl: './suppliers-list.component.css'
})
export class SuppliersListComponent implements OnInit {
  private supplierApi = inject(SupplierApiService);
  private fb = inject(FormBuilder);

  suppliers: SupplierResponse[] = [];
  searchQuery = '';

  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  showFormModal = false;
  editingSupplier: SupplierResponse | null = null;

  selectedSupplierForHistory: SupplierResponse | null = null;
  purchaseHistory: SupplierPurchaseHistoryResponse[] = [];

  supplierForm = this.fb.group({
    name: ['', Validators.required],
    companyName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', Validators.required],
    address: [''],
    city: [''],
    state: [''],
    postalCode: [''],
    country: [''],
    openingBalance: [0]
  });

  cityPresets = [
    'Karachi', 'Lahore', 'Islamabad', 'Faisalabad', 'Gujranwala',
    'Sialkot', 'Peshawar', 'China (Import)', 'UAE (Import)', 'Germany (Import)'
  ];

  companyPresets = [
    'Global Electronics Trading Co.',
    'Apex Wholesalers & Distributors',
    'Prime Food Products Pvt Ltd',
    'Universal Hardware Supplies',
    'National Textile Mills'
  ];

  selectCityPreset(city: string): void {
    this.supplierForm.patchValue({ city });
  }

  selectCompanyPreset(companyName: string): void {
    this.supplierForm.patchValue({ companyName, name: companyName + ' Agent' });
  }

  ngOnInit(): void {
    this.loadSuppliers();
  }

  loadSuppliers(): void {
    this.supplierApi.getSuppliers(this.searchQuery, undefined, undefined, undefined, false, this.currentPage, this.pageSize)
      .subscribe(res => {
        if (res.success) {
          this.suppliers = res.data;
          this.currentPage = res.metadata.currentPage;
          this.totalPages = res.metadata.totalPages;
          this.totalCount = res.metadata.totalCount;
        }
      });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadSuppliers();
    }
  }

  openCreateModal(): void {
    this.editingSupplier = null;
    this.supplierForm.reset({ openingBalance: 0 });
    this.showFormModal = true;
  }

  openEditModal(supplier: SupplierResponse): void {
    this.editingSupplier = supplier;
    this.supplierForm.patchValue({
      name: supplier.name,
      companyName: supplier.companyName,
      email: supplier.email,
      phoneNumber: supplier.phoneNumber,
      address: supplier.address || '',
      city: supplier.city || '',
      state: supplier.state || '',
      postalCode: supplier.postalCode || '',
      country: supplier.country || ''
    });
    this.showFormModal = true;
  }

  closeFormModal(): void {
    this.showFormModal = false;
    this.editingSupplier = null;
  }

  saveSupplier(): void {
    if (this.supplierForm.invalid) return;

    const val = this.supplierForm.value;
    if (this.editingSupplier) {
      this.supplierApi.updateSupplier(this.editingSupplier.id, {
        name: val.name!,
        companyName: val.companyName!,
        email: val.email!,
        phoneNumber: val.phoneNumber!,
        address: val.address || null,
        city: val.city || null,
        state: val.state || null,
        postalCode: val.postalCode || null,
        country: val.country || null,
        isActive: true
      }).subscribe(res => {
        if (res.success) {
          this.closeFormModal();
          this.loadSuppliers();
        }
      });
    } else {
      this.supplierApi.createSupplier({
        name: val.name!,
        companyName: val.companyName!,
        email: val.email!,
        phoneNumber: val.phoneNumber!,
        address: val.address || null,
        city: val.city || null,
        state: val.state || null,
        postalCode: val.postalCode || null,
        country: val.country || null,
        openingBalance: val.openingBalance || 0
      }).subscribe(res => {
        if (res.success) {
          this.closeFormModal();
          this.loadSuppliers();
        }
      });
    }
  }

  deleteSupplier(id: string): void {
    if (confirm('Are you sure you want to delete this supplier?')) {
      this.supplierApi.deleteSupplier(id).subscribe(res => {
        if (res.success) {
          this.loadSuppliers();
        }
      });
    }
  }

  viewPurchases(supplier: SupplierResponse): void {
    this.selectedSupplierForHistory = supplier;
    this.supplierApi.getSupplierPurchases(supplier.id).subscribe(res => {
      if (res.success && res.data) {
        this.purchaseHistory = res.data;
      }
    });
  }

  closeHistoryModal(): void {
    this.selectedSupplierForHistory = null;
    this.purchaseHistory = [];
  }
}

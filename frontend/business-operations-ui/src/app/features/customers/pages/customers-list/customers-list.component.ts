import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CustomerApiService } from '../../../../core/services/customer-api.service';
import { CustomerResponse } from '../../models/customer-response.model';
import { CustomerTransactionHistoryResponse } from '../../models/customer-transaction-history-response.model';

@Component({
  selector: 'app-customers-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './customers-list.component.html',
  styleUrl: './customers-list.component.css'
})
export class CustomersListComponent implements OnInit {
  private customerApi = inject(CustomerApiService);
  private fb = inject(FormBuilder);

  customers: CustomerResponse[] = [];
  searchQuery = '';

  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  showFormModal = false;
  editingCustomer: CustomerResponse | null = null;

  selectedCustomerForHistory: CustomerResponse | null = null;
  transactions: CustomerTransactionHistoryResponse[] = [];

  customerForm = this.fb.group({
    name: ['', Validators.required],
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
    'Karachi', 'Lahore', 'Islamabad', 'Rawalpindi', 'Faisalabad',
    'Multan', 'Peshawar', 'Quetta', 'Sialkot', 'Dubai', 'Riyadh'
  ];

  selectCityPreset(city: string): void {
    this.customerForm.patchValue({ city });
  }

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.customerApi.getCustomers(this.searchQuery, undefined, undefined, undefined, false, this.currentPage, this.pageSize)
      .subscribe(res => {
        if (res.success) {
          this.customers = res.data;
          this.currentPage = res.metadata.currentPage;
          this.totalPages = res.metadata.totalPages;
          this.totalCount = res.metadata.totalCount;
        }
      });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadCustomers();
    }
  }

  openCreateModal(): void {
    this.editingCustomer = null;
    this.customerForm.reset({ openingBalance: 0 });
    this.showFormModal = true;
  }

  openEditModal(customer: CustomerResponse): void {
    this.editingCustomer = customer;
    this.customerForm.patchValue({
      name: customer.name,
      email: customer.email,
      phoneNumber: customer.phoneNumber,
      address: customer.address || '',
      city: customer.city || '',
      state: customer.state || '',
      postalCode: customer.postalCode || '',
      country: customer.country || ''
    });
    this.showFormModal = true;
  }

  closeFormModal(): void {
    this.showFormModal = false;
    this.editingCustomer = null;
  }

  saveCustomer(): void {
    if (this.customerForm.invalid) return;

    const val = this.customerForm.value;
    if (this.editingCustomer) {
      this.customerApi.updateCustomer(this.editingCustomer.id, {
        name: val.name!,
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
          this.loadCustomers();
        }
      });
    } else {
      this.customerApi.createCustomer({
        name: val.name!,
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
          this.loadCustomers();
        }
      });
    }
  }

  deleteCustomer(id: string): void {
    if (confirm('Are you sure you want to delete this customer?')) {
      this.customerApi.deleteCustomer(id).subscribe(res => {
        if (res.success) {
          this.loadCustomers();
        }
      });
    }
  }

  viewTransactions(customer: CustomerResponse): void {
    this.selectedCustomerForHistory = customer;
    this.customerApi.getCustomerTransactions(customer.id).subscribe(res => {
      if (res.success && res.data) {
        this.transactions = res.data;
      }
    });
  }

  closeHistoryModal(): void {
    this.selectedCustomerForHistory = null;
    this.transactions = [];
  }
}

import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { PaymentApiService } from '../../../../core/services/payment-api.service';
import { CustomerApiService } from '../../../../core/services/customer-api.service';
import { SupplierApiService } from '../../../../core/services/supplier-api.service';
import { PaymentResponse } from '../../models/payment-response.model';
import { CustomerResponse } from '../../../customers/models/customer-response.model';
import { SupplierResponse } from '../../../suppliers/models/supplier-response.model';
import { PaymentType } from '../../../../core/enums/payment-type.enum';
import { PaymentMethod } from '../../../../core/enums/payment-method.enum';

@Component({
  selector: 'app-payments-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './payments-list.component.html',
  styleUrl: './payments-list.component.css'
})
export class PaymentsListComponent implements OnInit {
  private paymentApi = inject(PaymentApiService);
  private customerApi = inject(CustomerApiService);
  private supplierApi = inject(SupplierApiService);
  private fb = inject(FormBuilder);

  payments: PaymentResponse[] = [];
  customers: CustomerResponse[] = [];
  suppliers: SupplierResponse[] = [];

  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  showFormModal = false;

  paymentForm = this.fb.group({
    paymentType: [PaymentType.CustomerPayment, Validators.required],
    customerId: [''],
    supplierId: [''],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    paymentMethod: [PaymentMethod.Cash, Validators.required],
    referenceNumber: [''],
    notes: ['']
  });

  ngOnInit(): void {
    this.loadPayments();
    this.loadCustomers();
    this.loadSuppliers();
  }

  loadPayments(): void {
    this.paymentApi.getPayments(undefined, undefined, undefined, undefined, undefined, undefined, undefined, this.currentPage, this.pageSize)
      .subscribe(res => {
        if (res.success) {
          this.payments = res.data;
          this.currentPage = res.metadata.currentPage;
          this.totalPages = res.metadata.totalPages;
          this.totalCount = res.metadata.totalCount;
        }
      });
  }

  loadCustomers(): void {
    this.customerApi.getCustomers(undefined, undefined, true, undefined, false, 1, 100).subscribe(res => {
      if (res.success) this.customers = res.data;
    });
  }

  loadSuppliers(): void {
    this.supplierApi.getSuppliers(undefined, undefined, true, undefined, false, 1, 100).subscribe(res => {
      if (res.success) this.suppliers = res.data;
    });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadPayments();
    }
  }

  openCreateModal(): void {
    this.paymentForm.reset({ paymentType: PaymentType.CustomerPayment, paymentMethod: PaymentMethod.Cash, amount: 0 });
    this.showFormModal = true;
  }

  closeFormModal(): void {
    this.showFormModal = false;
  }

  onPaymentTypeChange(): void {
    this.paymentForm.patchValue({ customerId: '', supplierId: '' });
  }

  savePayment(): void {
    if (this.paymentForm.invalid) return;

    const val = this.paymentForm.value;
    this.paymentApi.processPayment({
      paymentType: val.paymentType!,
      customerId: val.customerId || null,
      supplierId: val.supplierId || null,
      amount: val.amount!,
      paymentMethod: val.paymentMethod!,
      referenceNumber: val.referenceNumber || null,
      notes: val.notes || null
    }).subscribe(res => {
      if (res.success) {
        this.closeFormModal();
        this.loadPayments();
      }
    });
  }
}

import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ExpenseApiService } from '../../../../core/services/expense-api.service';
import { ExpenseCategoryApiService } from '../../../../core/services/expense-category-api.service';
import { ExpenseResponse } from '../../models/expense-response.model';
import { ExpenseCategoryResponse } from '../../../expense-categories/models/expense-category-response.model';
import { PaymentMethod } from '../../../../core/enums/payment-method.enum';

@Component({
  selector: 'app-expenses-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './expenses-list.component.html',
  styleUrl: './expenses-list.component.css'
})
export class ExpensesListComponent implements OnInit {
  private expenseApi = inject(ExpenseApiService);
  private categoryApi = inject(ExpenseCategoryApiService);
  private fb = inject(FormBuilder);

  expenses: ExpenseResponse[] = [];
  categories: ExpenseCategoryResponse[] = [];

  searchQuery = '';
  selectedCategory = '';

  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  showFormModal = false;
  editingExpense: ExpenseResponse | null = null;

  expenseForm = this.fb.group({
    title: ['', Validators.required],
    expenseCategoryId: ['', Validators.required],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    paymentMethod: [PaymentMethod.Cash, Validators.required],
    referenceNumber: [''],
    notes: ['']
  });

  expensePresetMap: Record<string, Array<{ name: string; amount: number; icon: string }>> = {
    'Utilities': [
      { name: 'Electricity Bill', amount: 15000, icon: '⚡' },
      { name: 'High-Speed Internet / Wifi', amount: 4500, icon: '🌐' },
      { name: 'Water & Maintenance Charges', amount: 3000, icon: '🚰' },
      { name: 'Office Tea, Coffee & Water', amount: 2500, icon: '☕' }
    ],
    'Rent': [
      { name: 'Office Monthly Rent', amount: 120000, icon: '🏢' },
      { name: 'Warehouse Rental Fee', amount: 85000, icon: '🏬' },
      { name: 'Shop Premises Rent', amount: 60000, icon: '🏪' }
    ],
    'Salaries': [
      { name: 'Staff Monthly Salary', amount: 45000, icon: '💵' },
      { name: 'Employee Overtime Allowance', amount: 8000, icon: '🕒' },
      { name: 'Performance Bonus', amount: 15000, icon: '🎁' }
    ],
    'Marketing': [
      { name: 'Facebook & Instagram Ads', amount: 20000, icon: '📲' },
      { name: 'Google Ads Campaign', amount: 35000, icon: '🔍' },
      { name: 'Printing Banners & Pamphlets', amount: 12000, icon: '🖨️' }
    ],
    'Travel': [
      { name: 'Fuel & Vehicle Gasoline', amount: 8000, icon: '⛽' },
      { name: 'Taxi / Cab Fare', amount: 2500, icon: '🚕' },
      { name: 'Business Trip Hotel Stay', amount: 25000, icon: '🏨' }
    ],
    'Default': [
      { name: 'Electricity Bill', amount: 15000, icon: '⚡' },
      { name: 'Office Monthly Rent', amount: 100000, icon: '🏢' },
      { name: 'High-Speed Internet', amount: 4500, icon: '🌐' },
      { name: 'Tea & Refreshments', amount: 2000, icon: '☕' },
      { name: 'Fuel & Gas', amount: 5000, icon: '⛽' }
    ]
  };

  get activeExpensePresets(): Array<{ name: string; amount: number; icon: string }> {
    const catId = this.expenseForm.get('expenseCategoryId')?.value;
    const catObj = this.categories.find(c => c.id === catId);
    const catName = (catObj?.name || '').toLowerCase();

    if (catName.includes('util') || catName.includes('bill')) return this.expensePresetMap['Utilities'];
    if (catName.includes('rent')) return this.expensePresetMap['Rent'];
    if (catName.includes('salar') || catName.includes('wage') || catName.includes('payroll')) return this.expensePresetMap['Salaries'];
    if (catName.includes('market') || catName.includes('ad')) return this.expensePresetMap['Marketing'];
    if (catName.includes('trave') || catName.includes('fuel')) return this.expensePresetMap['Travel'];

    return this.expensePresetMap['Default'];
  }

  selectExpensePreset(preset: { name: string; amount: number }): void {
    this.expenseForm.patchValue({
      title: preset.name,
      amount: preset.amount
    });
  }

  ngOnInit(): void {
    this.loadExpenses();
    this.loadCategories();
  }

  loadExpenses(): void {
    this.expenseApi.getExpenses(
      this.searchQuery,
      this.selectedCategory || undefined,
      undefined, undefined, undefined, false,
      this.currentPage, this.pageSize
    ).subscribe(res => {
      if (res.success) {
        this.expenses = res.data;
        this.currentPage = res.metadata.currentPage;
        this.totalPages = res.metadata.totalPages;
        this.totalCount = res.metadata.totalCount;
      }
    });
  }

  loadCategories(): void {
    this.categoryApi.getAllCategories().subscribe(res => {
      if (res.success && res.data) this.categories = res.data;
    });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadExpenses();
    }
  }

  openCreateModal(): void {
    this.editingExpense = null;
    this.expenseForm.reset({ paymentMethod: PaymentMethod.Cash, amount: 0 });
    this.showFormModal = true;
  }

  openEditModal(expense: ExpenseResponse): void {
    this.editingExpense = expense;
    this.expenseForm.patchValue({
      title: expense.title,
      expenseCategoryId: expense.expenseCategoryId,
      amount: expense.amount,
      paymentMethod: expense.paymentMethod,
      referenceNumber: expense.referenceNumber || '',
      notes: expense.notes || ''
    });
    this.showFormModal = true;
  }

  closeFormModal(): void {
    this.showFormModal = false;
    this.editingExpense = null;
  }

  saveExpense(): void {
    if (this.expenseForm.invalid) return;

    const val = this.expenseForm.value;
    if (this.editingExpense) {
      this.expenseApi.updateExpense(this.editingExpense.id, {
        title: val.title!,
        expenseCategoryId: val.expenseCategoryId!,
        amount: val.amount!,
        paymentMethod: val.paymentMethod!,
        referenceNumber: val.referenceNumber || null,
        notes: val.notes || null
      }).subscribe(res => {
        if (res.success) {
          this.closeFormModal();
          this.loadExpenses();
        }
      });
    } else {
      this.expenseApi.createExpense({
        title: val.title!,
        expenseCategoryId: val.expenseCategoryId!,
        amount: val.amount!,
        paymentMethod: val.paymentMethod!,
        referenceNumber: val.referenceNumber || null,
        notes: val.notes || null
      }).subscribe(res => {
        if (res.success) {
          this.closeFormModal();
          this.loadExpenses();
        }
      });
    }
  }

  deleteExpense(id: string): void {
    if (confirm('Are you sure you want to delete this expense record?')) {
      this.expenseApi.deleteExpense(id).subscribe(res => {
        if (res.success) {
          this.loadExpenses();
        }
      });
    }
  }
}

import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormArray, Validators } from '@angular/forms';
import { SaleApiService } from '../../../../core/services/sale-api.service';
import { CustomerApiService } from '../../../../core/services/customer-api.service';
import { ProductApiService } from '../../../../core/services/product-api.service';
import { ExportService } from '../../../../core/services/export.service';
import { TranslationService } from '../../../../core/services/translation.service';
import { SaleResponse } from '../../models/sale-response.model';
import { CustomerResponse } from '../../../customers/models/customer-response.model';
import { ProductResponse } from '../../../products/models/product-response.model';

@Component({
  selector: 'app-sales-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './sales-list.component.html',
  styleUrl: './sales-list.component.css'
})
export class SalesListComponent implements OnInit {
  private saleApi = inject(SaleApiService);
  private customerApi = inject(CustomerApiService);
  private productApi = inject(ProductApiService);
  private exportService = inject(ExportService);
  ts = inject(TranslationService);
  private fb = inject(FormBuilder);

  sales: SaleResponse[] = [];
  customers: CustomerResponse[] = [];
  products: ProductResponse[] = [];

  searchQuery = '';
  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  showCreateModal = false;
  selectedSale: SaleResponse | null = null;

  calculatedSubtotal = 0;
  calculatedTax = 0;
  calculatedTotal = 0;

  saleForm = this.fb.group({
    customerId: ['', Validators.required],
    taxRate: [0],
    discountAmount: [0],
    paidAmount: [0],
    items: this.fb.array([])
  });

  taxPresets = [
    { label: '0% Exempt', value: 0 },
    { label: '5% Standard VAT', value: 5 },
    { label: '10% Sales Tax', value: 10 },
    { label: '17% GST', value: 17 },
    { label: '18% Retail Tax', value: 18 }
  ];

  setTaxPreset(rate: number): void {
    this.saleForm.patchValue({ taxRate: rate });
    this.calculateTotals();
  }

  setDiscountPreset(discount: number): void {
    this.saleForm.patchValue({ discountAmount: discount });
    this.calculateTotals();
  }

  setPaymentPreset(type: 'full' | 'half' | 'none'): void {
    if (type === 'full') {
      this.saleForm.patchValue({ paidAmount: this.calculatedTotal });
    } else if (type === 'half') {
      this.saleForm.patchValue({ paidAmount: Math.round((this.calculatedTotal / 2) * 100) / 100 });
    } else {
      this.saleForm.patchValue({ paidAmount: 0 });
    }
  }

  setCustomerPreset(id: string): void {
    this.saleForm.patchValue({ customerId: id });
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
    return this.saleForm.get('items') as FormArray;
  }

  ngOnInit(): void {
    this.loadSales();
    this.loadCustomers();
    this.loadProducts();
  }

  loadSales(): void {
    this.saleApi.getSales(this.searchQuery, undefined, undefined, undefined, undefined, undefined, undefined, false, this.currentPage, this.pageSize)
      .subscribe(res => {
        if (res.success) {
          this.sales = res.data;
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

  loadProducts(): void {
    this.productApi.getProducts(undefined, undefined, true, undefined, undefined, false, 1, 100).subscribe(res => {
      if (res.success) this.products = res.data;
    });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadSales();
    }
  }

  openCreateModal(): void {
    this.saleForm.reset({ taxRate: 0, discountAmount: 0, paidAmount: 0 });
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
      unitPrice: [0, [Validators.required, Validators.min(0)]]
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
      item.patchValue({ unitPrice: prod.salePrice });
    }
    this.calculateTotals();
  }

  calculateTotals(): void {
    let sub = 0;
    for (let i = 0; i < this.itemsArray.length; i++) {
      const val = this.itemsArray.at(i).value;
      sub += (val.quantity || 0) * (val.unitPrice || 0);
    }
    this.calculatedSubtotal = sub;

    const taxRate = this.saleForm.value.taxRate || 0;
    const discount = this.saleForm.value.discountAmount || 0;

    this.calculatedTax = (sub * taxRate) / 100;
    this.calculatedTotal = sub + this.calculatedTax - discount;
  }

  saveSale(): void {
    if (this.saleForm.invalid) return;

    const val = this.saleForm.value;
    this.saleApi.createSale({
      customerId: val.customerId!,
      taxRate: val.taxRate || 0,
      discountAmount: val.discountAmount || 0,
      paidAmount: val.paidAmount || 0,
      items: val.items as any
    }).subscribe(res => {
      if (res.success) {
        this.closeCreateModal();
        this.loadSales();
      }
    });
  }

  viewDetails(sale: SaleResponse): void {
    this.selectedSale = sale;
  }

  closeDetailsModal(): void {
    this.selectedSale = null;
  }

  cancelSale(id: string): void {
    if (confirm('Are you sure you want to cancel this invoice? Product stock and customer balance will be reverted.')) {
      this.saleApi.cancelSale(id).subscribe(res => {
        if (res.success) {
          this.loadSales();
        }
      });
    }
  }

  exportSalesCsv(): void {
    if (!this.sales || !this.sales.length) return;
    const exportData = this.sales.map(s => ({
      InvoiceNumber: s.invoiceNumber,
      CustomerName: s.customerName,
      CustomerCode: s.customerCode,
      SaleDate: s.saleDateUtc,
      SubTotal: s.subTotal,
      TaxAmount: s.taxAmount,
      DiscountAmount: s.discountAmount,
      TotalAmount: s.totalAmount,
      PaidAmount: s.paidAmount,
      DueAmount: s.dueAmount,
      Status: s.statusDescription
    }));
    this.exportService.exportToCsv('Sales_Invoices_Report', exportData);
  }

  printInvoice(sale: SaleResponse): void {
    window.print();
  }

  printThermalReceipt(sale: SaleResponse): void {
    const printWin = window.open('', '_blank', 'width=400,height=600');
    if (!printWin) return;
    const itemsHtml = sale.items.map(i => `
      <tr>
        <td style="padding: 4px 0;">${i.productName} (x${i.quantity})</td>
        <td style="text-align: right; padding: 4px 0;">$${i.totalPrice.toFixed(2)}</td>
      </tr>
    `).join('');

    printWin.document.write(`
      <html>
        <head>
          <title>POS Receipt - ${sale.invoiceNumber}</title>
          <style>
            body { font-family: monospace; font-size: 12px; padding: 10px; width: 280px; margin: 0 auto; }
            .header { text-align: center; border-bottom: 1px dashed #000; padding-bottom: 8px; margin-bottom: 8px; }
            .title { font-size: 16px; font-weight: bold; }
            table { width: 100%; border-collapse: collapse; margin-bottom: 8px; }
            .totals { border-top: 1px dashed #000; padding-top: 8px; }
            .total-line { display: flex; justify-content: space-between; font-weight: bold; }
            .footer { text-align: center; margin-top: 16px; font-size: 10px; }
          </style>
        </head>
        <body>
          <div class="header">
            <div class="title">ACME ENTERPRISE</div>
            <div>POS COUNTER RECEIPT</div>
            <div>Invoice: ${sale.invoiceNumber}</div>
            <div>Customer: ${sale.customerName}</div>
          </div>
          <table>
            <tbody>${itemsHtml}</tbody>
          </table>
          <div class="totals">
            <div class="total-line"><span>SUBTOTAL:</span><span>$${sale.subTotal.toFixed(2)}</span></div>
            <div class="total-line"><span>TAX (${sale.taxRate}%):</span><span>+$${sale.taxAmount.toFixed(2)}</span></div>
            <div class="total-line"><span>DISCOUNT:</span><span>-$${sale.discountAmount.toFixed(2)}</span></div>
            <div class="total-line" style="font-size: 14px; margin-top: 4px;"><span>GRAND TOTAL:</span><span>$${sale.totalAmount.toFixed(2)}</span></div>
            <div class="total-line" style="color: green; margin-top: 4px;"><span>PAID:</span><span>$${sale.paidAmount.toFixed(2)}</span></div>
          </div>
          <div class="footer">
            <p>*** THANK YOU FOR YOUR BUSINESS ***</p>
          </div>
          <script>
            window.onload = function() { window.print(); window.close(); }
          </script>
        </body>
      </html>
    `);
    printWin.document.close();
  }
}

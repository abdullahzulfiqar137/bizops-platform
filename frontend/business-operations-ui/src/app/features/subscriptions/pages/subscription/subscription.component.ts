import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin, finalize } from 'rxjs';
import { BusinessCurrencyPipe } from '@core/pipes/business-currency.pipe';
import { NotificationService } from '@core/services/notification.service';
import {
  SubscriptionApiService,
  SubscriptionPlan,
  CurrentSubscriptionData,
  SubscriptionInvoice,
} from '../../../../core/services/subscription-api.service';

const DEFAULT_PLANS: SubscriptionPlan[] = [
  {
    id: '11111111-1111-1111-1111-111111111111',
    name: 'Starter Plan',
    code: 'STARTER',
    monthlyPrice: 29,
    annualPrice: 290,
    maxUsers: 3,
    maxSalesPerMonth: 250,
    enableAiAssistant: false,
    enableDoubleEntryAccounting: false,
    enablePdfGeneration: true,
    description: 'Essential point-of-sale and inventory tracking for small retail stores.',
  },
  {
    id: '22222222-2222-2222-2222-222222222222',
    name: 'Professional Plan',
    code: 'PRO',
    monthlyPrice: 79,
    annualPrice: 790,
    maxUsers: 15,
    maxSalesPerMonth: 2500,
    enableAiAssistant: true,
    enableDoubleEntryAccounting: true,
    enablePdfGeneration: true,
    description: 'Complete ERP operations with PDF invoices, SignalR alerts, and AI insights.',
  },
  {
    id: '33333333-3333-3333-3333-333333333333',
    name: 'Enterprise VIP Plan',
    code: 'ENTERPRISE',
    monthlyPrice: 199,
    annualPrice: 1990,
    maxUsers: 100,
    maxSalesPerMonth: 100000,
    enableAiAssistant: true,
    enableDoubleEntryAccounting: true,
    enablePdfGeneration: true,
    description: 'Unlimited high-volume business operations with dedicated AI assistant & automated backups.',
  },
];

const DEFAULT_CURRENT: CurrentSubscriptionData = {
  subscription: {
    id: 'sub-active-001',
    planId: '22222222-2222-2222-2222-222222222222',
    status: 2,
    startDateUtc: new Date().toISOString(),
    endDateUtc: new Date(Date.now() + 335 * 86400000).toISOString(),
    autoRenew: true,
    paymentMethod: 'Visa ending in 4242',
    plan: DEFAULT_PLANS[1],
  },
  usage: {
    activeUsers: 1,
    maxUsers: 15,
    monthlySales: 12,
    maxMonthlySales: 2500,
  },
};

const DEFAULT_INVOICES: SubscriptionInvoice[] = [
  {
    id: 'inv-001',
    invoiceNumber: 'SUB-INV-2026-001',
    amount: 790,
    currency: 'USD',
    status: 2,
    billingDateUtc: new Date(Date.now() - 30 * 86400000).toISOString(),
    paymentTransactionReference: 'Visa Card **** 4242',
    subscription: {
      plan: DEFAULT_PLANS[1],
    },
  },
];

export interface SavedPaymentMethod {
  id: string;
  type: 'card' | 'paypal' | 'bank';
  label: string;
  sublabel: string;
  isDefault: boolean;
  brandIcon: string;
}

@Component({
  selector: 'app-subscription',
  standalone: true,
  imports: [CommonModule, FormsModule, BusinessCurrencyPipe],
  templateUrl: './subscription.component.html',
  styleUrl: './subscription.component.css'
})
export class SubscriptionComponent implements OnInit {
  private api = inject(SubscriptionApiService);
  private notifications = inject(NotificationService);

  loading = false;
  upgrading = false;
  isAnnual = true;
  activeTab: 'overview' | 'plans' | 'payment-methods' | 'company-profile' | 'invoices' = 'overview';

  plans: SubscriptionPlan[] = [];
  currentSub: CurrentSubscriptionData | null = null;
  invoices: SubscriptionInvoice[] = [];

  // Saved Payment Methods list
  savedPaymentMethods: SavedPaymentMethod[] = [
    {
      id: 'pm-1',
      type: 'card',
      label: 'Visa ending in 4242',
      sublabel: 'Expires 12/2028 • Primary Default Card',
      isDefault: true,
      brandIcon: '💳',
    },
    {
      id: 'pm-2',
      type: 'paypal',
      label: 'PayPal Account (billing@bizops.com)',
      sublabel: 'Authorized Express Billing',
      isDefault: false,
      brandIcon: '🅿️',
    },
  ];

  // Company Billing & Tax Profile
  companyProfile = {
    legalName: 'BizOps Enterprise Global Inc.',
    taxId: 'US-VAT-992144510',
    billingEmail: 'accounting@bizops.com',
    currencyCode: 'USD',
    address: '100 Enterprise Way, Suite 500',
    cityState: 'New York, NY 10001',
    countryZip: 'United States • 10001',
  };

  // Checkout Modal State
  showCheckoutModal = false;
  showAddPmModal = false;
  selectedPlan: SubscriptionPlan | null = null;
  selectedPaymentMethod: 'card' | 'paypal' | 'wallet' | 'bank' = 'card';
  processingPayment = false;

  cardForm = {
    name: 'BizOps Operations Ltd',
    number: '4242 •••• •••• 4242',
    expiry: '12/28',
    cvc: '888',
  };

  newPmForm = {
    name: '',
    number: '',
    expiry: '',
    cvc: '',
  };

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    forkJoin({
      plans: this.api.getPlans(),
      current: this.api.getCurrentSubscription(),
      invoices: this.api.getInvoices(),
    })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (r) => {
          this.plans = r.plans?.data ?? [];
          if (r.current?.data) {
            this.currentSub = r.current.data;
          }
          this.invoices = r.invoices?.data ?? [];
        },
        error: () => {
          this.plans = [];
          this.currentSub = null;
          this.invoices = [];
          this.notifications.error('Subscription data could not be loaded. No local billing data was substituted.');
        },
      });
  }

  openCheckoutModal(plan: SubscriptionPlan): void {
    this.selectedPlan = plan;
    this.showCheckoutModal = true;
  }

  closeCheckoutModal(): void {
    this.showCheckoutModal = false;
    this.selectedPlan = null;
  }

  processPayment(): void {
    if (!this.selectedPlan) return;

    this.processingPayment = true;

    this.api
      .subscribePlan(this.selectedPlan.id, this.isAnnual)
      .pipe(finalize(() => (this.processingPayment = false)))
      .subscribe({
        next: (res) => {
          this.notifications.success(res.message || `Payment approved! Upgraded to ${this.selectedPlan?.name}`);
          this.closeCheckoutModal();
          this.loadData();
        },
        error: (error) => {
          this.notifications.error(error?.error?.message || 'Payment was not processed and the subscription was not changed.');
        },
      });
  }

  private addCompletedInvoice(plan: SubscriptionPlan): void {
    const invNumber = `SUB-INV-${new Date().getFullYear()}-${Math.floor(100 + Math.random() * 900)}`;
    const newInvoice: SubscriptionInvoice = {
      id: `inv-${Date.now()}`,
      invoiceNumber: invNumber,
      amount: this.getPlanPrice(plan),
      currency: this.companyProfile.currencyCode,
      status: 2,
      billingDateUtc: new Date().toISOString(),
      paymentTransactionReference: `${this.getPaymentMethodLabel()} (${new Date().toLocaleTimeString()})`,
      subscription: { plan },
    };

    this.invoices = [newInvoice, ...this.invoices];
    this.currentSub = {
      subscription: {
        id: `sub-${Date.now()}`,
        planId: plan.id,
        status: 2,
        startDateUtc: new Date().toISOString(),
        endDateUtc: new Date(Date.now() + 365 * 86400000).toISOString(),
        autoRenew: true,
        paymentMethod: this.getPaymentMethodLabel(),
        plan: plan,
      },
      usage: {
        activeUsers: 1,
        maxUsers: plan.maxUsers,
        monthlySales: 12,
        maxMonthlySales: plan.maxSalesPerMonth,
      },
    };
  }

  setDefaultPaymentMethod(id: string): void {
    this.savedPaymentMethods.forEach((pm) => (pm.isDefault = pm.id === id));
    this.notifications.success('Default payment method updated!');
  }

  removePaymentMethod(id: string): void {
    this.savedPaymentMethods = this.savedPaymentMethods.filter((pm) => pm.id !== id);
    this.notifications.info('Payment method removed.');
  }

  addNewPaymentMethod(): void {
    if (!this.newPmForm.number) {
      this.notifications.error('Please enter card number.');
      return;
    }
    const last4 = this.newPmForm.number.slice(-4) || '8888';
    this.savedPaymentMethods.push({
      id: `pm-${Date.now()}`,
      type: 'card',
      label: `Card ending in ${last4}`,
      sublabel: `Expires ${this.newPmForm.expiry || '12/28'}`,
      isDefault: false,
      brandIcon: '💳',
    });
    this.showAddPmModal = false;
    this.newPmForm = { name: '', number: '', expiry: '', cvc: '' };
    this.notifications.success('New payment card saved successfully!');
  }

  saveCompanyProfile(): void {
    this.notifications.success('Company billing profile & VAT tax information saved!');
  }

  downloadReceipt(inv: SubscriptionInvoice): void {
    const textContent = `
==================================================
           BIZOPS ENTERPRISE BILLING RECEIPT
==================================================
Receipt #: ${inv.invoiceNumber}
Date: ${new Date(inv.billingDateUtc).toLocaleDateString()}
Status: PAID ✅

Company: ${this.companyProfile.legalName}
Tax/VAT ID: ${this.companyProfile.taxId}
Billing Email: ${this.companyProfile.billingEmail}

Plan: ${inv.subscription?.plan?.name ?? 'Professional Plan'}
Amount Paid: $${inv.amount.toFixed(2)} USD
Payment Method: ${inv.paymentTransactionReference || 'Visa Card **** 4242'}
==================================================
Thank you for subscribing to BizOps Enterprise SaaS!
    `;

    const blob = new Blob([textContent], { type: 'text/plain;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `${inv.invoiceNumber}.txt`;
    link.click();
    URL.revokeObjectURL(url);
    this.notifications.success(`Downloaded receipt ${inv.invoiceNumber}`);
  }

  getPaymentMethodLabel(): string {
    switch (this.selectedPaymentMethod) {
      case 'card': return 'Credit/Debit Card **** 4242';
      case 'paypal': return 'PayPal Express Checkout';
      case 'wallet': return 'Apple / Google Pay';
      case 'bank': return 'Direct Wire Transfer (IBAN)';
    }
  }

  isCurrentPlan(planId: string): boolean {
    return this.currentSub?.subscription?.planId === planId;
  }

  getStatusText(status?: number): string {
    return ['', 'Trialing ⏳', 'Active ✅', 'Past Due ⚠️', 'Cancelled 🚫', 'Expired ❌'][status ?? 2] ?? 'Active ✅';
  }

  getPlanPrice(plan?: SubscriptionPlan | null): number {
    if (!plan) return 790;
    return this.isAnnual ? plan.annualPrice : plan.monthlyPrice;
  }

  getUserUsagePercent(): number {
    if (!this.currentSub?.usage) return 50;
    return Math.min(100, Math.round((this.currentSub.usage.activeUsers / this.currentSub.usage.maxUsers) * 100));
  }

  getSalesUsagePercent(): number {
    if (!this.currentSub?.usage) return 30;
    return Math.min(100, Math.round((this.currentSub.usage.monthlySales / this.currentSub.usage.maxMonthlySales) * 100));
  }
}

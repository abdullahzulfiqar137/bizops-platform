import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {
  LucideBell, LucideBot, LucideBriefcaseBusiness, LucideBuilding2,
  LucideChartNoAxesCombined, LucideCreditCard, LucideDatabaseBackup,
  LucideFileUp, LucideLandmark, LucideLayoutDashboard, LucideMessageSquare,
  LucidePackage, LucideReceiptText, LucideScrollText, LucideShoppingCart,
  LucideTags, LucideTruck, LucideUndo2, LucideUserCog, LucideUsersRound,
  LucideWalletCards, LucideWarehouse, LucideSearch, LucideSparkles,
  LucideArrowRight, LucideCheckCircle2, LucideBookOpen, LucideShieldCheck
} from '@lucide/angular';

interface ModuleItem {
  id: string;
  title: string;
  category: 'operations' | 'inventory' | 'financials' | 'team';
  icon: string;
  route: string;
  shortDesc: string;
  whatItDoes: string;
  whyUseIt: string;
  highlights: string[];
  color: string;
}

@Component({
  selector: 'app-business-overview',
  standalone: true,
  imports: [
    CommonModule, RouterLink, FormsModule,
    LucideBell, LucideBot, LucideBriefcaseBusiness, LucideBuilding2,
    LucideChartNoAxesCombined, LucideCreditCard, LucideDatabaseBackup,
    LucideFileUp, LucideLandmark, LucideLayoutDashboard, LucideMessageSquare,
    LucidePackage, LucideReceiptText, LucideScrollText, LucideShoppingCart,
    LucideTags, LucideTruck, LucideUndo2, LucideUserCog, LucideUsersRound,
    LucideWalletCards, LucideWarehouse, LucideSearch, LucideSparkles,
    LucideArrowRight, LucideCheckCircle2, LucideBookOpen, LucideShieldCheck
  ],
  templateUrl: './business-overview.component.html',
  styleUrl: './business-overview.component.css'
})
export class BusinessOverviewComponent {
  readonly searchQuery = signal('');
  readonly selectedCategory = signal<'all' | 'operations' | 'inventory' | 'financials' | 'team'>('all');

  readonly modules: ModuleItem[] = [
    {
      id: 'dashboard',
      title: 'Executive Dashboard',
      category: 'operations',
      icon: 'dashboard',
      route: '/dashboard',
      shortDesc: 'Real-time overview of business revenue, profit, cash flow & executive digests.',
      whatItDoes: 'Displays top KPI metric cards (Revenue, Net Profit, Purchases, Expenses) along with auto-generated AI Executive Summaries.',
      whyUseIt: 'Use daily to get an instant financial health check of your business operations at a single glance.',
      highlights: ['Revenue & Net Profit KPIs', 'AI Executive Digest', 'Cash Flow Widgets'],
      color: '#2563eb'
    },
    {
      id: 'ai-insights',
      title: 'AI Insights & Analytics',
      category: 'operations',
      icon: 'bot',
      route: '/ai-insights',
      shortDesc: 'Artificial intelligence recommendations for stock optimization and revenue growth.',
      whatItDoes: 'Analyzes historical sales, product demand patterns, and profit margins to issue automated operational recommendations.',
      whyUseIt: 'Use when planning stock orders, identifying low-margin products, and predicting future revenue trends.',
      highlights: ['Smart Demand Forecasting', 'Profit Margin Warnings', 'Anomaly Detection'],
      color: '#8b5cf6'
    },
    {
      id: 'sales',
      title: 'Sales & Invoices',
      category: 'operations',
      icon: 'sales',
      route: '/sales',
      shortDesc: 'Create sales orders, tax invoices, customer billing, and track payment statuses.',
      whatItDoes: 'Generates professional invoices, deducts product inventory automatically, and records customer receivables.',
      whyUseIt: 'Use whenever selling products or services to retail or wholesale customers.',
      highlights: ['PDF Invoice Generation', 'Tax Calculation', 'Paid/Unpaid Status Tracking'],
      color: '#10b981'
    },
    {
      id: 'purchases',
      title: 'Purchases & Vendor Bills',
      category: 'operations',
      icon: 'purchases',
      route: '/purchases',
      shortDesc: 'Manage supplier purchase orders, stock arrivals, and procurement expenditures.',
      whatItDoes: 'Records vendor bills, increases product stock levels upon order receiving, and registers supplier payables.',
      whyUseIt: 'Use whenever ordering new stock or raw materials from your trade suppliers.',
      highlights: ['Stock Auto-Restock', 'Supplier Cost Logs', 'Order Receiving Status'],
      color: '#f59e0b'
    },
    {
      id: 'payments',
      title: 'Payments Center',
      category: 'operations',
      icon: 'payments',
      route: '/payments',
      shortDesc: 'Record cash, card, and bank transfers for sales invoices and supplier bills.',
      whatItDoes: 'Logs payment transactions linked to sales invoices and purchase bills with full payment method detail.',
      whyUseIt: 'Use to record customer payment receipts or issue payments to vendors and update outstanding balances.',
      highlights: ['Multi-Payment Methods', 'Partial Settlement', 'Receipt Number Tracking'],
      color: '#06b6d4'
    },
    {
      id: 'expenses',
      title: 'Expenses Management',
      category: 'operations',
      icon: 'expenses',
      route: '/expenses',
      shortDesc: 'Track daily operating expenses including office rent, utilities, and wages.',
      whatItDoes: 'Categorizes non-inventory overhead expenses and deducts them from net profit calculations.',
      whyUseIt: 'Use whenever paying operating bills (electricity, salary, rent, marketing) to keep financial reports accurate.',
      highlights: ['Category Breakdown', 'Receipt Notes', 'Profit Deduction'],
      color: '#f43f5e'
    },
    {
      id: 'returns',
      title: 'Returns Center',
      category: 'operations',
      icon: 'returns',
      route: '/returns',
      shortDesc: 'Process sales returns from customers and purchase returns to suppliers.',
      whatItDoes: 'Handles returned items, restocks undamaged inventory back to warehouses, and calculates customer refunds or credit.',
      whyUseIt: 'Use when a customer brings back goods or when defective items are shipped back to a vendor.',
      highlights: ['Refunds & Credits', 'Restock Options', 'Return Reason Auditing'],
      color: '#ea580c'
    },
    {
      id: 'products',
      title: 'Products Master Catalog',
      category: 'inventory',
      icon: 'products',
      route: '/products',
      shortDesc: 'Master registry of items, SKUs, barcode tracking, pricing, and unit measures.',
      whatItDoes: 'Stores all product details, cost prices, selling prices, tax rates, reorder thresholds, and active status.',
      whyUseIt: 'Use to introduce new items, update prices, or adjust minimum reorder points.',
      highlights: ['SKU & Barcode Support', 'Cost vs Price Margin', 'Reorder Point Alerts'],
      color: '#7c3aed'
    },
    {
      id: 'inventory',
      title: 'Stock Adjustments',
      category: 'inventory',
      icon: 'inventory',
      route: '/inventory',
      shortDesc: 'Perform stock audits, record shrinkage, damage, and inventory counts.',
      whatItDoes: 'Manually adjusts quantity on hand for items due to physical audits, damage, loss, or promotional giveaways.',
      whyUseIt: 'Use during periodic warehouse physical inventory audits or when stock damaged items must be written off.',
      highlights: ['Audit Adjustments', 'Damage Write-offs', 'Reason Tracking'],
      color: '#14b8a6'
    },
    {
      id: 'categories',
      title: 'Product Categories',
      category: 'inventory',
      icon: 'categories',
      route: '/categories',
      shortDesc: 'Group items into structured product categories for catalog organization.',
      whatItDoes: 'Allows grouping of inventory items into logical divisions for easy searching and sales reporting.',
      whyUseIt: 'Use when organizing catalog taxonomy to analyze which product lines generate top revenue.',
      highlights: ['Taxonomy Grouping', 'Filtered Reports', 'Clean Catalog Tree'],
      color: '#d97706'
    },
    {
      id: 'customers',
      title: 'Customers Directory (CRM)',
      category: 'operations',
      icon: 'customers',
      route: '/customers',
      shortDesc: 'Customer directory, contact details, total purchase history, and credit balances.',
      whatItDoes: 'Maintains customer profiles, contact info, total lifetime spend, and outstanding invoice balances.',
      whyUseIt: 'Use to manage client relationships, verify credit limits, and view individual customer sales history.',
      highlights: ['Outstanding Receivables', 'Purchase History', 'Contact Management'],
      color: '#0284c7'
    },
    {
      id: 'suppliers',
      title: 'Suppliers Directory',
      category: 'operations',
      icon: 'suppliers',
      route: '/suppliers',
      shortDesc: 'Vendor profiles, payables tracking, lead times, and purchase histories.',
      whatItDoes: 'Stores supplier contact details, payment terms, and total outstanding payables.',
      whyUseIt: 'Use when managing wholesale vendor lists, placing purchase orders, or tracking payable liabilities.',
      highlights: ['Payables Tracking', 'Vendor Contact Cards', 'Order Histories'],
      color: '#16a34a'
    },
    {
      id: 'reports',
      title: 'Financial Reports',
      category: 'financials',
      icon: 'reports',
      route: '/reports',
      shortDesc: 'Comprehensive Profit & Loss, sales summaries, and tax breakdown reports.',
      whatItDoes: 'Aggregates sales revenue, cost of goods sold (COGS), operating expenses, and tax liabilities over any date range.',
      whyUseIt: 'Use for executive decision-making, tax filing, investor reporting, and financial auditing.',
      highlights: ['Profit & Loss (P&L)', 'Sales Breakdown', 'Tax Liability Summaries'],
      color: '#4f46e5'
    },
    {
      id: 'accounting',
      title: 'Accounting Center',
      category: 'financials',
      icon: 'accounting',
      route: '/accounting',
      shortDesc: 'Double-entry general ledger, journal entries, and account balances.',
      whatItDoes: 'Automatically posts debit/credit ledger entries for sales, purchases, payments, and expenses.',
      whyUseIt: 'Use by finance teams and accountants to verify trial balances and chart of accounts integrity.',
      highlights: ['General Ledger', 'Journal Entries', 'Chart of Accounts'],
      color: '#ca8a04'
    },
    {
      id: 'audit-logs',
      title: 'Audit Logs & Security',
      category: 'financials',
      icon: 'audit',
      route: '/audit-logs',
      shortDesc: 'Immutable record of user actions, data edits, and system events.',
      whatItDoes: 'Logs every create, update, delete, and login operation with timestamp, user ID, and modified values.',
      whyUseIt: 'Use for security oversight, compliance audits, and investigating unauthorized data changes.',
      highlights: ['User Activity Logs', 'Timestamp Verification', 'Action Traceability'],
      color: '#64748b'
    },
    {
      id: 'imports',
      title: 'Data Import Center',
      category: 'inventory',
      icon: 'imports',
      route: '/imports',
      shortDesc: 'Bulk import products, customers, and inventory data via CSV/Excel.',
      whatItDoes: 'Parses uploaded spreadsheets, maps columns to system schemas, and creates records in bulk.',
      whyUseIt: 'Use when migrating data from legacy systems or onboarding large product catalogs quickly.',
      highlights: ['CSV / Excel Importer', 'Column Mapping', 'Validation Checking'],
      color: '#0d9488'
    },
    {
      id: 'notifications',
      title: 'Notifications Center',
      category: 'team',
      icon: 'notifications',
      route: '/notifications',
      shortDesc: 'System alerts, low stock warnings, and overdue payment notifications.',
      whatItDoes: 'Displays urgent operational alerts generated by system triggers or automated checks.',
      whyUseIt: 'Use daily to ensure low stock items are reordered and overdue invoices are followed up.',
      highlights: ['Low Stock Alerts', 'Overdue Invoice Alerts', 'Read/Unread Tracking'],
      color: '#e11d48'
    },
    {
      id: 'team-chat',
      title: 'Team Live Chat',
      category: 'team',
      icon: 'team-chat',
      route: '/team-chat',
      shortDesc: 'Real-time messaging for internal staff and department communication.',
      whatItDoes: 'Enables instant messaging between logged-in users, team channels, and order discussions.',
      whyUseIt: 'Use to communicate quickly with colleagues, discuss stock queries, or coordinate sales.',
      highlights: ['Real-time SignalR Chat', 'Channel Messaging', 'Direct Messages'],
      color: '#2563eb'
    },
    {
      id: 'employees',
      title: 'Employees (HRM)',
      category: 'team',
      icon: 'employees',
      route: '/employees',
      shortDesc: 'Staff roster, designations, department allocation, and contact records.',
      whatItDoes: 'Stores employee profiles, employment status, job titles, department assignments, and contact details.',
      whyUseIt: 'Use for internal HR record-keeping, department structuring, and staff roster management.',
      highlights: ['Department Allocation', 'Job Titles', 'Staff Directory'],
      color: '#9333ea'
    },
    {
      id: 'users',
      title: 'Users & Access Control',
      category: 'team',
      icon: 'users',
      route: '/users',
      shortDesc: 'Manage user logins, password resets, and role-based permissions (RBAC).',
      whatItDoes: 'Controls who can log into the system and what actions (Admin, Manager, Accountant, Cashier) they can perform.',
      whyUseIt: 'Use when onboarding new employees, resetting passwords, or restricting sensitive financial views.',
      highlights: ['Role Assignment (RBAC)', 'Active / Deactive Status', 'Security Permissions'],
      color: '#0284c7'
    },
    {
      id: 'business-profile',
      title: 'Business Profile',
      category: 'team',
      icon: 'profile',
      route: '/business-profile',
      shortDesc: 'Company organization details, tax numbers, currency, and address.',
      whatItDoes: 'Stores company legal name, tax registration number, logo, default currency, and header details for printed invoices.',
      whyUseIt: 'Use when initializing your business or updating contact/tax info printed on customer receipts.',
      highlights: ['Company Branding', 'Invoice Header Settings', 'Tax Registration'],
      color: '#7c3aed'
    },
    {
      id: 'subscriptions',
      title: 'Subscription & Billing',
      category: 'team',
      icon: 'subscriptions',
      route: '/subscriptions',
      shortDesc: 'Manage platform plan tier, feature limits, and subscription renewals.',
      whatItDoes: 'Displays current subscription plan, renewal dates, license quotas, and upgrade options.',
      whyUseIt: 'Use to check active plan capabilities or upgrade your subscription for higher operational limits.',
      highlights: ['Plan Tier Status', 'Usage Limits', 'Billing Details'],
      color: '#059669'
    },
    {
      id: 'backups',
      title: 'Database Backups',
      category: 'team',
      icon: 'backups',
      route: '/backups',
      shortDesc: 'Create SQL database backups, restore points, and disaster recovery.',
      whatItDoes: 'Executes SQL Server backup routines, lists historical backup files, and supports safe data restoration.',
      whyUseIt: 'Use regularly to backup business data and safeguard against hardware failures or accidental data loss.',
      highlights: ['One-Click SQL Backups', 'Downloadable .bak Files', 'Disaster Recovery'],
      color: '#d97706'
    }
  ];

  readonly filteredModules = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    const cat = this.selectedCategory();

    return this.modules.filter(m => {
      const matchesCat = cat === 'all' || m.category === cat;
      const matchesQuery = !query || 
        m.title.toLowerCase().includes(query) ||
        m.shortDesc.toLowerCase().includes(query) ||
        m.whatItDoes.toLowerCase().includes(query) ||
        m.whyUseIt.toLowerCase().includes(query) ||
        m.highlights.some(h => h.toLowerCase().includes(query));

      return matchesCat && matchesQuery;
    });
  });

  getCategoryLabel(category: string): string {
    switch (category) {
      case 'operations': return 'Core Operations';
      case 'inventory': return 'Inventory & Catalog';
      case 'financials': return 'Financials & Analytics';
      case 'team': return 'Team & Admin';
      default: return category;
    }
  }
}

import { Component, Output, EventEmitter, HostListener, ElementRef, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthStateService } from '../../core/state/auth-state.service';
import { AuthApiService } from '../../core/services/auth-api.service';
import { TranslationService, LanguageOption } from '../../core/services/translation.service';

interface SearchDestination {
  label: string;
  description: string;
  route: string;
  code: string;
}

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.css'
})
export class TopbarComponent {
  @Output() toggleSidebar = new EventEmitter<void>();

  private authState = inject(AuthStateService);
  private authApi = inject(AuthApiService);
  private router = inject(Router);
  private host = inject(ElementRef<HTMLElement>);
  ts = inject(TranslationService);

  user = this.authState.currentUser;
  @ViewChild('searchInput') searchInput?: ElementRef<HTMLInputElement>;

  showMenu = false;
  quickCreateOpen = false;
  searchOpen = false;
  langMenuOpen = false;
  isDarkMode = false;
  searchQuery = '';

  languages = [
    { code: 'EN', name: 'English (US)', flag: '🇺🇸' },
    { code: 'UR', name: 'Urdu (اردو)', flag: '🇵🇰' },
    { code: 'AR', name: 'Arabic (العربية)', flag: '🇦🇪' },
    { code: 'ES', name: 'Spanish (Español)', flag: '🇪🇸' }
  ];
  selectedLang = this.languages[0];

  readonly searchResults: SearchDestination[] = [
    { label: 'Sales & Invoices', description: 'Create invoice & manage sales', route: '/sales', code: 'SL' },
    { label: 'Purchases', description: 'Record supplier purchase orders', route: '/purchases', code: 'PO' },
    { label: 'Payments', description: 'Customer & vendor payment vouchers', route: '/payments', code: 'PY' },
    { label: 'Expenses', description: 'Record operating expenses', route: '/expenses', code: 'EX' },
    { label: 'Products', description: 'Manage inventory item catalog', route: '/products', code: 'PR' },
    { label: 'Stock Adjustments', description: 'Adjust warehouse stock count', route: '/inventory', code: 'IN' },
    { label: 'Categories', description: 'Product category taxonomy', route: '/categories', code: 'CT' },
    { label: 'Customers', description: 'Customer CRM directory', route: '/customers', code: 'CU' },
    { label: 'Suppliers', description: 'Supplier vendor directory', route: '/suppliers', code: 'SU' },
    { label: 'Team Live Chat', description: 'Real-time team messaging', route: '/team-chat', code: 'TC' },
    { label: 'Employees (HRM)', description: 'Human Resource Management', route: '/employees', code: 'HR' },
    { label: 'Financial Reports', description: 'Profit loss & financial reports', route: '/reports', code: 'RP' },
    { label: 'Accounting Center', description: 'Double-entry general ledger', route: '/accounting', code: 'AC' },
    { label: 'Users & Access', description: 'System user management', route: '/users', code: 'US' },
    { label: 'Subscription & Billing', description: 'SaaS plan & invoice billing', route: '/subscriptions', code: 'SB' },
    { label: 'Database Backups', description: 'Security backups & restore', route: '/backups', code: 'DB' },
  ];

  get filteredSearchResults(): SearchDestination[] {
    if (!this.searchQuery.trim()) {
      return this.searchResults;
    }
    const query = this.searchQuery.toLowerCase().trim();
    return this.searchResults.filter(
      (r) => r.label.toLowerCase().includes(query) ||
        r.description.toLowerCase().includes(query) ||
        r.code.toLowerCase().includes(query) ||
        r.route.toLowerCase().includes(query)
    );
  }

  onToggleSidebar(): void {
    this.toggleSidebar.emit();
  }

  toggleQuickCreate(): void {
    this.quickCreateOpen = !this.quickCreateOpen;
    this.showMenu = false;
    this.searchOpen = false;
    this.langMenuOpen = false;
  }

  toggleLangMenu(): void {
    this.langMenuOpen = !this.langMenuOpen;
    this.quickCreateOpen = false;
    this.showMenu = false;
    this.searchOpen = false;
  }

  selectLanguage(lang: LanguageOption): void {
    this.ts.setLanguage(lang);
    this.langMenuOpen = false;
  }

  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;
    if (this.isDarkMode) {
      document.body.classList.add('dark-theme');
    } else {
      document.body.classList.remove('dark-theme');
    }
  }

  toggleUserMenu(): void {
    this.showMenu = !this.showMenu;
    this.quickCreateOpen = false;
    this.searchOpen = false;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.host.nativeElement.contains(event.target as Node)) {
      this.closeMenus();
    }
  }

  @HostListener('document:keydown', ['$event'])
  onKeydown(event: KeyboardEvent): void {
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') {
      event.preventDefault();
      this.searchInput?.nativeElement.focus();
      this.searchOpen = true;
    }
  }

  @HostListener('document:keydown.escape')
  closeMenus(): void {
    this.showMenu = false;
    this.quickCreateOpen = false;
    this.searchOpen = false;
  }

  onSearch(): void {
    if (!this.searchQuery.trim()) return;
    const query = this.searchQuery.toLowerCase();
    const match = this.searchResults.find(
      (r) => r.label.toLowerCase().includes(query) || r.route.includes(query)
    );
    if (match) {
      this.openSearchResult(match.route);
    }
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.searchOpen = false;
  }

  openSearchResult(route: string): void {
    this.router.navigate([route]);
    this.clearSearch();
  }

  getInitials(name: string): string {
    if (!name) return 'U';
    const parts = name.trim().split(' ');
    if (parts.length >= 2) {
      return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
    }
    return name.substring(0, 2).toUpperCase();
  }

  getRoleBadge(): string {
    const roles = this.authState.userRoles();
    return roles.length > 0 ? roles[0] : 'User';
  }

  getRoleDescription(): string {
    switch (this.getRoleBadge().toLowerCase()) {
      case 'admin':
        return 'Full workspace access, user administration, billing, backups, and business settings.';
      case 'manager':
        return 'Manage daily operations, reports, staff workflows, inventory, sales, and purchases.';
      case 'accountant':
        return 'Access accounting, payments, expenses, financial reports, and audit records.';
      case 'sales':
        return 'Manage customers, sales invoices, payments, and related operational records.';
      default:
        return 'Standard access to the workspace features assigned to your account.';
    }
  }

  logout(): void {
    this.closeMenus();
    this.authApi.logout();
    this.router.navigate(['/auth/login']);
  }
}

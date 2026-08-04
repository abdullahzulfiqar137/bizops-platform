import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { permissionGuard } from './core/guards/permission.guard';
import { Roles } from './core/constants/roles.constant';

export const routes: Routes = [
  {
    path: 'forbidden',
    loadComponent: () => import('./pages/forbidden/forbidden.component').then(m => m.ForbiddenComponent)
  },
  {
    path: 'auth/login',
    loadComponent: () => import('./features/authentication/pages/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'auth/register',
    loadComponent: () => import('./features/authentication/pages/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'auth/forgot-password',
    loadComponent: () => import('./features/authentication/pages/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent)
  },
  {
    path: 'auth/reset-password',
    loadComponent: () => import('./features/authentication/pages/reset-password/reset-password.component').then(m => m.ResetPasswordComponent)
  },
  {
    path: '',
    loadComponent: () => import('./layout/main-layout/main-layout.component').then(m => m.MainLayoutComponent),
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'business-overview',
        loadComponent: () => import('./features/business-overview/pages/business-overview/business-overview.component').then(m => m.BusinessOverviewComponent)
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/pages/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'business-profile',
        loadComponent: () => import('./features/business-profile/pages/business-profile/business-profile.component').then(m => m.BusinessProfileComponent),
        canActivate: [permissionGuard],
        data: { roles: [Roles.Admin, Roles.Manager] }
      },
      {
        path: 'users',
        loadComponent: () => import('./features/users/pages/users-list/users-list.component').then(m => m.UsersListComponent),
        canActivate: [permissionGuard],
        data: { roles: [Roles.Admin, Roles.Manager] }
      },
      {
        path: 'account-security',
        loadComponent: () => import('./features/authentication/pages/account-security/account-security.component').then(m => m.AccountSecurityComponent)
      },
      {
        path: 'profile',
        loadComponent: () => import('./features/profile/pages/my-profile.component').then(m => m.MyProfileComponent)
      },
      {
        path: 'roles',
        loadComponent: () => import('./features/roles/pages/roles-list/roles-list.component').then(m => m.RolesListComponent),
        canActivate: [permissionGuard],
        data: { roles: [Roles.Admin] }
      },
      {
        path: 'customers',
        loadComponent: () => import('./features/customers/pages/customers-list/customers-list.component').then(m => m.CustomersListComponent)
      },
      {
        path: 'suppliers',
        loadComponent: () => import('./features/suppliers/pages/suppliers-list/suppliers-list.component').then(m => m.SuppliersListComponent)
      },
      {
        path: 'employees',
        loadComponent: () => import('./features/employees/pages/employees-list/employees-list.component').then(m => m.EmployeesListComponent)
      },
      {
        path: 'team-chat',
        loadComponent: () => import('./features/team-chat/pages/team-chat/team-chat.component').then(m => m.TeamChatComponent)
      },
      {
        path: 'categories',
        loadComponent: () => import('./features/categories/pages/categories-list/categories-list.component').then(m => m.CategoriesListComponent)
      },
      {
        path: 'products',
        loadComponent: () => import('./features/products/pages/products-list/products-list.component').then(m => m.ProductsListComponent)
      },
      {
        path: 'inventory',
        loadComponent: () => import('./features/inventory/pages/inventory-list/inventory-list.component').then(m => m.InventoryListComponent)
      },
      {
        path: 'sales',
        loadComponent: () => import('./features/sales/pages/sales-list/sales-list.component').then(m => m.SalesListComponent)
      },
      {
        path: 'purchases',
        loadComponent: () => import('./features/purchases/pages/purchases-list/purchases-list.component').then(m => m.PurchasesListComponent)
      },
      {
        path: 'expenses',
        loadComponent: () => import('./features/expenses/pages/expenses-list/expenses-list.component').then(m => m.ExpensesListComponent)
      },
      {
        path: 'payments',
        loadComponent: () => import('./features/payments/pages/payments-list/payments-list.component').then(m => m.PaymentsListComponent)
      },
      {
        path: 'returns',
        loadComponent: () => import('./features/returns/pages/return-center.component').then(m => m.ReturnCenterComponent)
      },
      {
        path: 'reports',
        loadComponent: () => import('./features/reports/pages/reports-overview/reports-overview.component').then(m => m.ReportsOverviewComponent)
      },
      {
        path: 'accounting',
        loadComponent: () => import('./features/accounting/pages/accounting-center/accounting-center.component').then(m => m.AccountingCenterComponent),
        canActivate: [permissionGuard],
        data: { roles: [Roles.Admin, Roles.Manager] }
      },
      {
        path: 'audit-logs',
        loadComponent: () => import('./features/audit-logs/pages/audit-logs-list/audit-logs-list.component').then(m => m.AuditLogsListComponent),
        canActivate: [permissionGuard],
        data: { roles: [Roles.Admin, Roles.Manager] }
      },
      {
        path: 'imports',
        loadComponent: () => import('./features/imports/pages/import-center.component').then(m => m.ImportCenterComponent),
        canActivate: [permissionGuard],
        data: { roles: [Roles.Admin, Roles.Manager] }
      },
      {
        path: 'notifications',
        loadComponent: () => import('./features/notifications/pages/notification-center.component').then(m => m.NotificationCenterComponent)
      },
      {
        path: 'subscriptions',
        loadComponent: () => import('./features/subscriptions/pages/subscription/subscription.component').then(m => m.SubscriptionComponent)
      },
      {
        path: 'backups',
        loadComponent: () => import('./features/database-backups/pages/database-backups/database-backups.component').then(m => m.DatabaseBackupsComponent),
        canActivate: [permissionGuard],
        data: { roles: [Roles.Admin] }
      },
      {
        path: 'ai-insights',
        loadComponent: () => import('./features/ai-insights/pages/ai-insights-overview/ai-insights-overview.component').then(m => m.AiInsightsOverviewComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];

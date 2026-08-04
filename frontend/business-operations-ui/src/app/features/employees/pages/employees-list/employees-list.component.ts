import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BusinessCurrencyPipe } from '@core/pipes/business-currency.pipe';
import { NotificationService } from '@core/services/notification.service';
import { EmployeeApiRecord, EmployeeApiService } from '../../../../core/services/employee-api.service';

export interface Employee {
  id: string;
  employeeCode: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  department: string;
  designation: string;
  hireDate: string;
  monthlySalary: number;
  status: string;
  avatarColor: string;
}

@Component({
  selector: 'app-employees-list',
  standalone: true,
  imports: [CommonModule, FormsModule, BusinessCurrencyPipe],
  templateUrl: './employees-list.component.html',
  styleUrl: './employees-list.component.css'
})
export class EmployeesListComponent implements OnInit {
  private notifications = inject(NotificationService);
  private employeeApi = inject(EmployeeApiService);

  employees: Employee[] = [];
  searchQuery = '';
  departmentFilter = 'ALL';
  statusFilter = 'ALL';
  customDepartment = '';

  readonly departmentOptions = [
    'Executive Management',
    'Operations',
    'Business Administration',
    'Sales',
    'Marketing',
    'Customer Success',
    'Customer Support',
    'Finance & Accounting',
    'Human Resources',
    'Information Technology',
    'Engineering',
    'Product Management',
    'Design & Creative',
    'Procurement',
    'Supply Chain',
    'Logistics',
    'Warehouse & Inventory',
    'Manufacturing & Production',
    'Quality Assurance',
    'Research & Development',
    'Legal',
    'Risk & Compliance',
    'Security',
    'Facilities & Administration'
  ];

  showAddModal = false;
  selectedEmployee: Employee | null = null;

  newEmp: Partial<Employee> = {
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    department: 'Operations',
    designation: '',
    monthlySalary: 5000,
    status: 'Active',
  };

  designationPresetsMap: Record<string, string[]> = {
    'Operations': ['Operations Lead', 'Warehouse Supervisor', 'Logistics Coordinator', 'Supply Chain Analyst'],
    'Sales': ['Sales Executive', 'Account Manager', 'Sales Operations Specialist', 'Business Development Rep'],
    'Marketing': ['Digital Marketing Specialist', 'Content Creator', 'SEO & Ads Manager', 'Brand Strategist'],
    'Finance & Accounting': ['Senior Accountant', 'Financial Analyst', 'Payroll Specialist', 'Internal Auditor'],
    'Human Resources': ['HR Generalist', 'Recruitment Specialist', 'HR Business Partner', 'Talent Manager'],
    'Information Technology': ['Full-Stack Developer', 'DevOps Engineer', 'System Administrator', 'IT Support Specialist'],
    'Default': ['Manager', 'Supervisor', 'Senior Officer', 'Executive', 'Assistant']
  };

  get activeDesignationPresets(): string[] {
    const dept = this.newEmp.department || '';
    return this.designationPresetsMap[dept] || this.designationPresetsMap['Default'];
  }

  selectDesignationPreset(desig: string): void {
    this.newEmp.designation = desig;
  }

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.employeeApi.getEmployees().subscribe({
      next: response => this.employees = (response.data ?? []).map(record => this.mapEmployee(record)),
      error: () => this.notifications.showError('Employee roster could not be loaded from the server.')
    });
  }

  getActiveCount(): number {
    return this.employees.filter((e) => e.status === 'Active').length;
  }

  getOnLeaveCount(): number {
    return this.employees.filter((e) => e.status === 'On Leave').length;
  }

  getTotalPayroll(): number {
    return this.employees.reduce((acc, curr) => acc + (curr.monthlySalary || 0), 0);
  }

  getFilteredEmployees(): Employee[] {
    return this.employees.filter((emp) => {
      const matchesSearch =
        !this.searchQuery ||
        emp.employeeCode.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        `${emp.firstName} ${emp.lastName}`.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        emp.email.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        emp.designation.toLowerCase().includes(this.searchQuery.toLowerCase());

      const matchesDept = this.departmentFilter === 'ALL' || emp.department === this.departmentFilter;
      const matchesStatus = this.statusFilter === 'ALL' || emp.status === this.statusFilter;

      return matchesSearch && matchesDept && matchesStatus;
    });
  }

  resetFilters(): void {
    this.searchQuery = '';
    this.departmentFilter = 'ALL';
    this.statusFilter = 'ALL';
  }

  openAddModal(): void {
    this.newEmp = {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      department: 'Operations',
      designation: '',
      monthlySalary: 5000,
      status: 'Active',
    };
    this.customDepartment = '';
    this.showAddModal = true;
  }

  saveNewEmployee(): void {
    if (!this.newEmp.firstName || !this.newEmp.lastName || !this.newEmp.email) {
      this.notifications.showError('Please fill in Employee First Name, Last Name, and Email.');
      return;
    }

    const department = this.newEmp.department === 'CUSTOM'
      ? this.customDepartment.trim()
      : this.newEmp.department?.trim();
    if (!department) {
      this.notifications.showError('Please select or enter an employee department.');
      return;
    }

    this.employeeApi.createEmployee({
      firstName: this.newEmp.firstName,
      lastName: this.newEmp.lastName,
      email: this.newEmp.email,
      phone: this.newEmp.phone || '',
      department,
      designation: this.newEmp.designation || 'Staff Member',
      monthlySalary: this.newEmp.monthlySalary || 0,
      status: this.newEmp.status || 'Active'
    }).subscribe({
      next: response => {
        if (!response.data) return;
        const created = this.mapEmployee(response.data);
        this.employees.unshift(created);
        this.showAddModal = false;
        this.notifications.showSuccess(response.message || `Employee ${created.firstName} ${created.lastName} registered successfully.`);
      },
      error: error => this.notifications.showError(error.error?.message || 'Employee could not be registered.')
    });
  }

  viewEmployee(emp: Employee): void {
    this.selectedEmployee = emp;
  }

  deleteEmployee(id: string): void {
    if (confirm('Are you sure you want to remove this employee record?')) {
      this.employeeApi.deleteEmployee(id).subscribe({
        next: response => {
          this.employees = this.employees.filter((e) => e.id !== id);
          this.notifications.showSuccess(response.message || 'Employee record removed.');
        },
        error: error => this.notifications.showError(error.error?.message || 'Employee record could not be removed.')
      });
    }
  }

  private mapEmployee(record: EmployeeApiRecord): Employee {
    return {
      id: record.id,
      employeeCode: record.employeeCode,
      firstName: record.firstName,
      lastName: record.lastName,
      email: record.email,
      phone: record.phone,
      department: record.department as Employee['department'],
      designation: record.designation,
      hireDate: record.hireDateUtc,
      monthlySalary: record.monthlySalary,
      status: record.status as Employee['status'],
      avatarColor: record.avatarColor
    };
  }

  getDeptClass(dept: string): string {
    switch (dept) {
      case 'Operations': return 'dept-ops';
      case 'Sales': return 'dept-sales';
      case 'Finance': return 'dept-fin';
      case 'IT & Engineering': return 'dept-it';
      case 'HR': return 'dept-hr';
      case 'Customer Support': return 'dept-supp';
      default: return 'dept-ops';
    }
  }
}

import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserApiService } from '../../../../core/services/user-api.service';
import { RoleApiService } from '../../../../core/services/role-api.service';
import { UserResponse } from '../../models/user-response.model';
import { RoleResponse } from '../../../roles/models/role-response.model';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.css'
})
export class UsersListComponent implements OnInit {
  private userApi = inject(UserApiService);
  private roleApi = inject(RoleApiService);

  users: UserResponse[] = [];
  availableRoles: RoleResponse[] = [];
  searchQuery = '';

  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  selectedUser: UserResponse | null = null;
  selectedRoles: string[] = [];

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();
  }

  loadUsers(): void {
    this.userApi.getUsers(this.searchQuery, this.currentPage, this.pageSize).subscribe(res => {
      if (res.success) {
        this.users = res.data;
        this.currentPage = res.metadata.currentPage;
        this.totalPages = res.metadata.totalPages;
        this.totalCount = res.metadata.totalCount;
      }
    });
  }

  loadRoles(): void {
    this.roleApi.getAllRoles().subscribe(res => {
      if (res.success && res.data) {
        this.availableRoles = res.data;
      }
    });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadUsers();
    }
  }

  toggleStatus(user: UserResponse): void {
    this.userApi.updateUserStatus(user.id, { isActive: !user.isActive }).subscribe(res => {
      if (res.success) {
        this.loadUsers();
      }
    });
  }

  openRoleModal(user: UserResponse): void {
    this.selectedUser = user;
    this.selectedRoles = [...user.roles];
  }

  closeModal(): void {
    this.selectedUser = null;
    this.selectedRoles = [];
  }

  toggleRoleSelection(roleName: string): void {
    const idx = this.selectedRoles.indexOf(roleName);
    if (idx > -1) {
      this.selectedRoles.splice(idx, 1);
    } else {
      this.selectedRoles.push(roleName);
    }
  }

  saveRoles(): void {
    if (!this.selectedUser) return;

    this.roleApi.assignRoles({
      userId: this.selectedUser.id,
      roles: this.selectedRoles
    }).subscribe(res => {
      if (res.success) {
        this.closeModal();
        this.loadUsers();
      }
    });
  }
}

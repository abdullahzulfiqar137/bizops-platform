import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { RoleApiService } from '../../../../core/services/role-api.service';
import { RoleResponse } from '../../models/role-response.model';
import { UserApiService } from '../../../../core/services/user-api.service';
import { UserResponse } from '../../../users/models/user-response.model';
import { NotificationService } from '../../../../core/services/notification.service';

@Component({
  selector: 'app-roles-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './roles-list.component.html',
  styleUrl: './roles-list.component.css'
})
export class RolesListComponent implements OnInit {
  private rolesApi = inject(RoleApiService);
  private usersApi = inject(UserApiService);
  private notices = inject(NotificationService);

  roles: RoleResponse[] = [];
  users: UserResponse[] = [];
  selections = new Map<string, Set<string>>();
  saving = '';

  ngOnInit(): void {
    forkJoin({
      roles: this.rolesApi.getAllRoles(),
      users: this.usersApi.getUsers(undefined, 1, 100)
    }).subscribe(r => {
      this.roles = r.roles.data ?? [];
      this.users = r.users.data ?? [];
      this.users.forEach(u => this.selections.set(u.id, new Set(u.roles)));
    });
  }

  hasRole(u: UserResponse, r: string): boolean {
    return this.selections.get(u.id)?.has(r) ?? false;
  }

  toggleRole(u: UserResponse, r: string, on: boolean): void {
    const set = this.selections.get(u.id) ?? new Set<string>();
    on ? set.add(r) : set.delete(r);
    this.selections.set(u.id, set);
  }

  save(u: UserResponse): void {
    this.saving = u.id;
    this.rolesApi.assignRoles({
      userId: u.id,
      roles: [...(this.selections.get(u.id) ?? [])]
    }).subscribe({
      next: r => {
        this.saving = '';
        this.notices.success(r.message);
      },
      error: e => {
        this.saving = '';
        this.notices.error(e.error?.message || 'Roles could not be saved.');
      }
    });
  }
}

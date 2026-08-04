import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { RoleResponse } from '../../features/roles/models/role-response.model';
import { AssignRoleRequest } from '../../features/roles/models/assign-role-request.model';
import { UserResponse } from '../../features/users/models/user-response.model';

@Injectable({
  providedIn: 'root'
})
export class RoleApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/roles`;

  constructor(private http: HttpClient) {}

  getAllRoles(): Observable<ApiResponse<RoleResponse[]>> {
    return this.http.get<ApiResponse<RoleResponse[]>>(this.baseUrl);
  }

  assignRoles(request: AssignRoleRequest): Observable<ApiResponse<UserResponse>> {
    return this.http.post<ApiResponse<UserResponse>>(`${this.baseUrl}/assign`, request);
  }
}

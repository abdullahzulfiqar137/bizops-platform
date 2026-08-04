import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { PagedResponse } from '../models/paged-response.model';
import { UserResponse } from '../../features/users/models/user-response.model';
import { UpdateUserStatusRequest } from '../../features/users/models/update-user-status-request.model';

@Injectable({
  providedIn: 'root'
})
export class UserApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/users`;

  constructor(private http: HttpClient) {}

  getUsers(search?: string, pageNumber: number = 1, pageSize: number = 10): Observable<PagedResponse<UserResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<PagedResponse<UserResponse>>(this.baseUrl, { params });
  }

  getUserById(id: string): Observable<ApiResponse<UserResponse>> {
    return this.http.get<ApiResponse<UserResponse>>(`${this.baseUrl}/${id}`);
  }

  updateUserStatus(id: string, request: UpdateUserStatusRequest): Observable<ApiResponse<UserResponse>> {
    return this.http.put<ApiResponse<UserResponse>>(`${this.baseUrl}/${id}/status`, request);
  }
}

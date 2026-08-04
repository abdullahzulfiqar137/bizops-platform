import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';

export interface EmployeeApiRecord {
  id: string;
  employeeCode: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  department: string;
  designation: string;
  hireDateUtc: string;
  monthlySalary: number;
  status: string;
  avatarColor: string;
}

export interface CreateEmployeePayload {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  department: string;
  designation: string;
  monthlySalary: number;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class EmployeeApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.baseUrl}/Employees`;

  getEmployees(department?: string, status?: string): Observable<ApiResponse<EmployeeApiRecord[]>> {
    let params = new HttpParams();
    if (department && department !== 'ALL') params = params.set('department', department);
    if (status && status !== 'ALL') params = params.set('status', status);
    return this.http.get<ApiResponse<EmployeeApiRecord[]>>(this.baseUrl, { params });
  }

  createEmployee(payload: CreateEmployeePayload): Observable<ApiResponse<EmployeeApiRecord>> {
    return this.http.post<ApiResponse<EmployeeApiRecord>>(this.baseUrl, payload);
  }

  deleteEmployee(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}

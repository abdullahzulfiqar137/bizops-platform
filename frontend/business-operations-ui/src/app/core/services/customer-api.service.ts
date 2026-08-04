import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { PagedResponse } from '../models/paged-response.model';
import { CustomerResponse } from '../../features/customers/models/customer-response.model';
import { CreateCustomerRequest } from '../../features/customers/models/create-customer-request.model';
import { UpdateCustomerRequest } from '../../features/customers/models/update-customer-request.model';
import { CustomerTransactionHistoryResponse } from '../../features/customers/models/customer-transaction-history-response.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/customers`;

  constructor(private http: HttpClient) {}

  getCustomers(
    search?: string,
    city?: string,
    isActive?: boolean,
    sortBy?: string,
    isDescending: boolean = false,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<PagedResponse<CustomerResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize)
      .set('isDescending', isDescending);

    if (search) params = params.set('search', search);
    if (city) params = params.set('city', city);
    if (isActive !== undefined && isActive !== null) params = params.set('isActive', isActive);
    if (sortBy) params = params.set('sortBy', sortBy);

    return this.http.get<PagedResponse<CustomerResponse>>(this.baseUrl, { params });
  }

  getCustomerById(id: string): Observable<ApiResponse<CustomerResponse>> {
    return this.http.get<ApiResponse<CustomerResponse>>(`${this.baseUrl}/${id}`);
  }

  createCustomer(request: CreateCustomerRequest): Observable<ApiResponse<CustomerResponse>> {
    return this.http.post<ApiResponse<CustomerResponse>>(this.baseUrl, request);
  }

  updateCustomer(id: string, request: UpdateCustomerRequest): Observable<ApiResponse<CustomerResponse>> {
    return this.http.put<ApiResponse<CustomerResponse>>(`${this.baseUrl}/${id}`, request);
  }

  deleteCustomer(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }

  getCustomerBalance(id: string): Observable<ApiResponse<number>> {
    return this.http.get<ApiResponse<number>>(`${this.baseUrl}/${id}/balance`);
  }

  getCustomerTransactions(id: string): Observable<ApiResponse<CustomerTransactionHistoryResponse[]>> {
    return this.http.get<ApiResponse<CustomerTransactionHistoryResponse[]>>(`${this.baseUrl}/${id}/transactions`);
  }
}

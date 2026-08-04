import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { PagedResponse } from '../models/paged-response.model';
import { SupplierResponse } from '../../features/suppliers/models/supplier-response.model';
import { CreateSupplierRequest } from '../../features/suppliers/models/create-supplier-request.model';
import { UpdateSupplierRequest } from '../../features/suppliers/models/update-supplier-request.model';
import { SupplierPurchaseHistoryResponse } from '../../features/suppliers/models/supplier-purchase-history-response.model';

@Injectable({
  providedIn: 'root'
})
export class SupplierApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/suppliers`;

  constructor(private http: HttpClient) {}

  getSuppliers(
    search?: string,
    city?: string,
    isActive?: boolean,
    hasBalance?: boolean,
    isDeleted?: boolean,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<PagedResponse<SupplierResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (search) params = params.set('search', search);
    if (city) params = params.set('city', city);
    if (isActive !== undefined) params = params.set('isActive', isActive);
    if (hasBalance !== undefined) params = params.set('hasBalance', hasBalance);
    if (isDeleted !== undefined) params = params.set('isDeleted', isDeleted);

    return this.http.get<PagedResponse<SupplierResponse>>(this.baseUrl, { params });
  }

  getSupplierById(id: string): Observable<ApiResponse<SupplierResponse>> {
    return this.http.get<ApiResponse<SupplierResponse>>(`${this.baseUrl}/${id}`);
  }

  createSupplier(request: CreateSupplierRequest): Observable<ApiResponse<SupplierResponse>> {
    return this.http.post<ApiResponse<SupplierResponse>>(this.baseUrl, request);
  }

  updateSupplier(id: string, request: UpdateSupplierRequest): Observable<ApiResponse<SupplierResponse>> {
    return this.http.put<ApiResponse<SupplierResponse>>(`${this.baseUrl}/${id}`, request);
  }

  deleteSupplier(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }

  getSupplierPurchases(id: string): Observable<ApiResponse<SupplierPurchaseHistoryResponse[]>> {
    return this.http.get<ApiResponse<SupplierPurchaseHistoryResponse[]>>(`${this.baseUrl}/${id}/purchases`);
  }
}

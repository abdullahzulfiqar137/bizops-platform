import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { PagedResponse } from '../models/paged-response.model';
import { PurchaseStatus } from '../enums/purchase-status.enum';
import { PaymentStatus } from '../enums/payment-status.enum';
import { CreatePurchaseRequest } from '../../features/purchases/models/create-purchase-request.model';
import { PurchaseResponse } from '../../features/purchases/models/purchase-response.model';

@Injectable({
  providedIn: 'root'
})
export class PurchaseApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/purchases`;

  constructor(private http: HttpClient) {}

  createPurchase(request: CreatePurchaseRequest): Observable<ApiResponse<PurchaseResponse>> {
    return this.http.post<ApiResponse<PurchaseResponse>>(this.baseUrl, request);
  }

  getPurchases(
    search?: string,
    supplierId?: string,
    status?: PurchaseStatus,
    paymentStatus?: PaymentStatus,
    startDate?: string,
    endDate?: string,
    sortBy?: string,
    isDescending: boolean = false,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<PagedResponse<PurchaseResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize)
      .set('isDescending', isDescending);

    if (search) params = params.set('search', search);
    if (supplierId) params = params.set('supplierId', supplierId);
    if (status) params = params.set('status', status);
    if (paymentStatus) params = params.set('paymentStatus', paymentStatus);
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    if (sortBy) params = params.set('sortBy', sortBy);

    return this.http.get<PagedResponse<PurchaseResponse>>(this.baseUrl, { params });
  }

  getPurchaseById(id: string): Observable<ApiResponse<PurchaseResponse>> {
    return this.http.get<ApiResponse<PurchaseResponse>>(`${this.baseUrl}/${id}`);
  }

  cancelPurchase(id: string): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/${id}/cancel`, {});
  }
}

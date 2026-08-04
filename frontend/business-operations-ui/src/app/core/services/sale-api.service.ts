import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { PagedResponse } from '../models/paged-response.model';
import { SaleStatus } from '../enums/sale-status.enum';
import { PaymentStatus } from '../enums/payment-status.enum';
import { CreateSaleRequest } from '../../features/sales/models/create-sale-request.model';
import { SaleResponse } from '../../features/sales/models/sale-response.model';

@Injectable({
  providedIn: 'root'
})
export class SaleApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/sales`;

  constructor(private http: HttpClient) {}

  createSale(request: CreateSaleRequest): Observable<ApiResponse<SaleResponse>> {
    return this.http.post<ApiResponse<SaleResponse>>(this.baseUrl, request);
  }

  getSales(
    search?: string,
    customerId?: string,
    status?: SaleStatus,
    paymentStatus?: PaymentStatus,
    startDate?: string,
    endDate?: string,
    sortBy?: string,
    isDescending: boolean = false,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<PagedResponse<SaleResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize)
      .set('isDescending', isDescending);

    if (search) params = params.set('search', search);
    if (customerId) params = params.set('customerId', customerId);
    if (status) params = params.set('status', status);
    if (paymentStatus) params = params.set('paymentStatus', paymentStatus);
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    if (sortBy) params = params.set('sortBy', sortBy);

    return this.http.get<PagedResponse<SaleResponse>>(this.baseUrl, { params });
  }

  getSaleById(id: string): Observable<ApiResponse<SaleResponse>> {
    return this.http.get<ApiResponse<SaleResponse>>(`${this.baseUrl}/${id}`);
  }

  cancelSale(id: string): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/${id}/cancel`, {});
  }
}

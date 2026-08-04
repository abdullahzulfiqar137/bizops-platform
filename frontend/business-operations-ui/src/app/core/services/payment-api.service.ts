import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { PagedResponse } from '../models/paged-response.model';
import { PaymentType } from '../enums/payment-type.enum';
import { CreatePaymentRequest } from '../../features/payments/models/create-payment-request.model';
import { PaymentResponse } from '../../features/payments/models/payment-response.model';

@Injectable({
  providedIn: 'root'
})
export class PaymentApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/payments`;

  constructor(private http: HttpClient) {}

  processPayment(request: CreatePaymentRequest): Observable<ApiResponse<PaymentResponse>> {
    return this.http.post<ApiResponse<PaymentResponse>>(this.baseUrl, request);
  }

  getPayments(
    paymentType?: PaymentType,
    customerId?: string,
    supplierId?: string,
    saleId?: string,
    purchaseId?: string,
    startDate?: string,
    endDate?: string,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<PagedResponse<PaymentResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (paymentType) params = params.set('paymentType', paymentType);
    if (customerId) params = params.set('customerId', customerId);
    if (supplierId) params = params.set('supplierId', supplierId);
    if (saleId) params = params.set('saleId', saleId);
    if (purchaseId) params = params.set('purchaseId', purchaseId);
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);

    return this.http.get<PagedResponse<PaymentResponse>>(this.baseUrl, { params });
  }

  getPaymentById(id: string): Observable<ApiResponse<PaymentResponse>> {
    return this.http.get<ApiResponse<PaymentResponse>>(`${this.baseUrl}/${id}`);
  }
}

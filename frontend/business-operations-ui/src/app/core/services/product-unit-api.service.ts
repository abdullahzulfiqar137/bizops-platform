import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { ProductUnitResponse } from '../../features/product-units/models/product-unit-response.model';
import { CreateProductUnitRequest } from '../../features/product-units/models/create-product-unit-request.model';
import { UpdateProductUnitRequest } from '../../features/product-units/models/update-product-unit-request.model';

@Injectable({
  providedIn: 'root'
})
export class ProductUnitApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/product-units`;

  constructor(private http: HttpClient) {}

  getProductUnits(): Observable<ApiResponse<ProductUnitResponse[]>> {
    return this.http.get<ApiResponse<ProductUnitResponse[]>>(this.baseUrl);
  }

  getUnitById(id: string): Observable<ApiResponse<ProductUnitResponse>> {
    return this.http.get<ApiResponse<ProductUnitResponse>>(`${this.baseUrl}/${id}`);
  }

  createUnit(request: CreateProductUnitRequest): Observable<ApiResponse<ProductUnitResponse>> {
    return this.http.post<ApiResponse<ProductUnitResponse>>(this.baseUrl, request);
  }

  updateUnit(id: string, request: UpdateProductUnitRequest): Observable<ApiResponse<ProductUnitResponse>> {
    return this.http.put<ApiResponse<ProductUnitResponse>>(`${this.baseUrl}/${id}`, request);
  }

  deleteUnit(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { PagedResponse } from '../models/paged-response.model';
import { ProductResponse } from '../../features/products/models/product-response.model';
import { CreateProductRequest } from '../../features/products/models/create-product-request.model';
import { UpdateProductRequest } from '../../features/products/models/update-product-request.model';
import { ProductUnitResponse } from '../../features/product-units/models/product-unit-response.model';

@Injectable({
  providedIn: 'root'
})
export class ProductApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/products`;

  constructor(private http: HttpClient) {}

  getProducts(
    search?: string,
    categoryId?: string,
    isActive?: boolean,
    isLowStock?: boolean,
    sortBy?: string,
    isDescending: boolean = false,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<PagedResponse<ProductResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize)
      .set('isDescending', isDescending);

    if (search) params = params.set('search', search);
    if (categoryId) params = params.set('categoryId', categoryId);
    if (isActive !== undefined && isActive !== null) params = params.set('isActive', isActive);
    if (isLowStock !== undefined && isLowStock !== null) params = params.set('isLowStock', isLowStock);
    if (sortBy) params = params.set('sortBy', sortBy);

    return this.http.get<PagedResponse<ProductResponse>>(this.baseUrl, { params });
  }

  getProductUnits(): Observable<ApiResponse<ProductUnitResponse[]>> {
    return this.http.get<ApiResponse<ProductUnitResponse[]>>(`${this.baseUrl}/units`);
  }

  getProductById(id: string): Observable<ApiResponse<ProductResponse>> {
    return this.http.get<ApiResponse<ProductResponse>>(`${this.baseUrl}/${id}`);
  }

  createProduct(request: CreateProductRequest): Observable<ApiResponse<ProductResponse>> {
    return this.http.post<ApiResponse<ProductResponse>>(this.baseUrl, request);
  }

  updateProduct(id: string, request: UpdateProductRequest): Observable<ApiResponse<ProductResponse>> {
    return this.http.put<ApiResponse<ProductResponse>>(`${this.baseUrl}/${id}`, request);
  }

  deleteProduct(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}

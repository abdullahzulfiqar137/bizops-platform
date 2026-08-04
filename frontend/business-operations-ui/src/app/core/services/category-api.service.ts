import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { CategoryResponse } from '../../features/categories/models/category-response.model';
import { CreateCategoryRequest } from '../../features/categories/models/create-category-request.model';
import { UpdateCategoryRequest } from '../../features/categories/models/update-category-request.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/categories`;

  constructor(private http: HttpClient) {}

  getCategories(): Observable<ApiResponse<CategoryResponse[]>> {
    return this.http.get<ApiResponse<CategoryResponse[]>>(this.baseUrl);
  }

  getCategoryById(id: string): Observable<ApiResponse<CategoryResponse>> {
    return this.http.get<ApiResponse<CategoryResponse>>(`${this.baseUrl}/${id}`);
  }

  createCategory(request: CreateCategoryRequest): Observable<ApiResponse<CategoryResponse>> {
    return this.http.post<ApiResponse<CategoryResponse>>(this.baseUrl, request);
  }

  updateCategory(id: string, request: UpdateCategoryRequest): Observable<ApiResponse<CategoryResponse>> {
    return this.http.put<ApiResponse<CategoryResponse>>(`${this.baseUrl}/${id}`, request);
  }

  deleteCategory(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}

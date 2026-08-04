import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { ExpenseCategoryResponse } from '../../features/expense-categories/models/expense-category-response.model';
import { CreateExpenseCategoryRequest } from '../../features/expense-categories/models/create-expense-category-request.model';
import { UpdateExpenseCategoryRequest } from '../../features/expense-categories/models/update-expense-category-request.model';

@Injectable({
  providedIn: 'root'
})
export class ExpenseCategoryApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/expense-categories`;

  constructor(private http: HttpClient) {}

  getAllCategories(): Observable<ApiResponse<ExpenseCategoryResponse[]>> {
    return this.http.get<ApiResponse<ExpenseCategoryResponse[]>>(this.baseUrl);
  }

  getCategoryById(id: string): Observable<ApiResponse<ExpenseCategoryResponse>> {
    return this.http.get<ApiResponse<ExpenseCategoryResponse>>(`${this.baseUrl}/${id}`);
  }

  createCategory(request: CreateExpenseCategoryRequest): Observable<ApiResponse<ExpenseCategoryResponse>> {
    return this.http.post<ApiResponse<ExpenseCategoryResponse>>(this.baseUrl, request);
  }

  updateCategory(id: string, request: UpdateExpenseCategoryRequest): Observable<ApiResponse<ExpenseCategoryResponse>> {
    return this.http.put<ApiResponse<ExpenseCategoryResponse>>(`${this.baseUrl}/${id}`, request);
  }

  deleteCategory(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}

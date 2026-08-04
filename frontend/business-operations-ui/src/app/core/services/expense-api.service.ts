import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { PagedResponse } from '../models/paged-response.model';
import { CreateExpenseRequest } from '../../features/expenses/models/create-expense-request.model';
import { UpdateExpenseRequest } from '../../features/expenses/models/update-expense-request.model';
import { ExpenseResponse } from '../../features/expenses/models/expense-response.model';

@Injectable({
  providedIn: 'root'
})
export class ExpenseApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/expenses`;

  constructor(private http: HttpClient) {}

  createExpense(request: CreateExpenseRequest): Observable<ApiResponse<ExpenseResponse>> {
    return this.http.post<ApiResponse<ExpenseResponse>>(this.baseUrl, request);
  }

  getExpenses(
    search?: string,
    expenseCategoryId?: string,
    startDate?: string,
    endDate?: string,
    sortBy?: string,
    isDescending: boolean = false,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<PagedResponse<ExpenseResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize)
      .set('isDescending', isDescending);

    if (search) params = params.set('search', search);
    if (expenseCategoryId) params = params.set('expenseCategoryId', expenseCategoryId);
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    if (sortBy) params = params.set('sortBy', sortBy);

    return this.http.get<PagedResponse<ExpenseResponse>>(this.baseUrl, { params });
  }

  getExpenseById(id: string): Observable<ApiResponse<ExpenseResponse>> {
    return this.http.get<ApiResponse<ExpenseResponse>>(`${this.baseUrl}/${id}`);
  }

  updateExpense(id: string, request: UpdateExpenseRequest): Observable<ApiResponse<ExpenseResponse>> {
    return this.http.put<ApiResponse<ExpenseResponse>>(`${this.baseUrl}/${id}`, request);
  }

  deleteExpense(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}

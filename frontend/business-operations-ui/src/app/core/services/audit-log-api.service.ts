import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { PagedResponse } from '../models/paged-response.model';
import { AuditLogResponse } from '../../features/audit-logs/models/audit-log-response.model';

@Injectable({
  providedIn: 'root'
})
export class AuditLogApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/audit-logs`;

  constructor(private http: HttpClient) {}

  getAuditLogs(
    entityName?: string,
    entityId?: string,
    userId?: string,
    action?: string,
    startDate?: string,
    endDate?: string,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<PagedResponse<AuditLogResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (entityName) params = params.set('entityName', entityName);
    if (entityId) params = params.set('entityId', entityId);
    if (userId) params = params.set('userId', userId);
    if (action) params = params.set('action', action);
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);

    return this.http.get<PagedResponse<AuditLogResponse>>(this.baseUrl, { params });
  }

  getAuditLogById(id: string): Observable<ApiResponse<AuditLogResponse>> {
    return this.http.get<ApiResponse<AuditLogResponse>>(`${this.baseUrl}/${id}`);
  }
}

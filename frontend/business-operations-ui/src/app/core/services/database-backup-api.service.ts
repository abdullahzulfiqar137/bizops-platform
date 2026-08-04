import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';

export interface DatabaseBackup {
  fileName: string;
  sizeBytes: number;
  createdAtUtc: string;
}

@Injectable({
  providedIn: 'root'
})
export class DatabaseBackupApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.baseUrl}/database-backups`;

  getBackups(): Observable<ApiResponse<DatabaseBackup[]>> {
    return this.http.get<ApiResponse<DatabaseBackup[]>>(this.baseUrl);
  }

  triggerBackup(): Observable<ApiResponse<DatabaseBackup>> {
    return this.http.post<ApiResponse<DatabaseBackup>>(this.baseUrl, {});
  }

  downloadBackup(fileName: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${encodeURIComponent(fileName)}/download`, {
      responseType: 'blob'
    });
  }

  restoreBackup(fileName: string): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/restore`, {
      fileName,
      confirmation: 'RESTORE'
    });
  }
}

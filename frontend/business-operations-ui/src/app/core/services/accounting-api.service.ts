import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import {
  AccountResponse,
  BalanceSheetResponse,
  JournalResponse,
  LedgerLineResponse,
  TrialBalanceResponse
} from '../../features/accounting/models/accounting.model';

@Injectable({
  providedIn: 'root'
})
export class AccountingApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.baseUrl}/accounting`;

  accounts(): Observable<ApiResponse<AccountResponse[]>> {
    return this.http.get<ApiResponse<AccountResponse[]>>(`${this.baseUrl}/accounts`);
  }

  journals(): Observable<ApiResponse<JournalResponse[]>> {
    return this.http.get<ApiResponse<JournalResponse[]>>(`${this.baseUrl}/journals`);
  }

  trialBalance(): Observable<ApiResponse<TrialBalanceResponse>> {
    return this.http.get<ApiResponse<TrialBalanceResponse>>(`${this.baseUrl}/trial-balance`);
  }

  balanceSheet(): Observable<ApiResponse<BalanceSheetResponse>> {
    return this.http.get<ApiResponse<BalanceSheetResponse>>(`${this.baseUrl}/balance-sheet`);
  }

  synchronize(): Observable<ApiResponse<unknown>> {
    return this.http.post<ApiResponse<unknown>>(`${this.baseUrl}/synchronize`, {});
  }

  createJournal(payload: unknown): Observable<ApiResponse<JournalResponse>> {
    return this.http.post<ApiResponse<JournalResponse>>(`${this.baseUrl}/journals`, payload);
  }

  ledger(id: string): Observable<ApiResponse<LedgerLineResponse[]>> {
    return this.http.get<ApiResponse<LedgerLineResponse[]>>(`${this.baseUrl}/ledger/${id}`);
  }
}

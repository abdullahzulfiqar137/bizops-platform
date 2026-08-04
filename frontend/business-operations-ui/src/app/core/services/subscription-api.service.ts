import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';

export interface SubscriptionPlan {
  id: string;
  name: string;
  code: string;
  monthlyPrice: number;
  annualPrice: number;
  maxUsers: number;
  maxSalesPerMonth: number;
  enableAiAssistant: boolean;
  enableDoubleEntryAccounting: boolean;
  enablePdfGeneration: boolean;
  description: string;
}

export interface CurrentSubscriptionData {
  subscription: {
    id: string;
    planId: string;
    status: number;
    startDateUtc: string;
    endDateUtc: string;
    trialEndsAtUtc?: string;
    autoRenew: boolean;
    paymentMethod: string;
    plan: SubscriptionPlan;
  };
  usage: {
    activeUsers: number;
    maxUsers: number;
    monthlySales: number;
    maxMonthlySales: number;
  };
}

export interface SubscriptionInvoice {
  id: string;
  invoiceNumber: string;
  amount: number;
  currency: string;
  status: number;
  billingDateUtc: string;
  paymentTransactionReference: string;
  subscription?: {
    plan?: SubscriptionPlan;
  };
}

@Injectable({
  providedIn: 'root',
})
export class SubscriptionApiService {
  private http = inject(HttpClient);
  private baseUrl = `${API_CONFIG.baseUrl}/subscriptions`;

  getPlans(): Observable<ApiResponse<SubscriptionPlan[]>> {
    return this.http.get<ApiResponse<SubscriptionPlan[]>>(`${this.baseUrl}/plans`);
  }

  getCurrentSubscription(): Observable<ApiResponse<CurrentSubscriptionData>> {
    return this.http.get<ApiResponse<CurrentSubscriptionData>>(`${this.baseUrl}/current`);
  }

  subscribePlan(planId: string, isAnnual = true): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.baseUrl}/subscribe`, { planId, isAnnual });
  }

  getInvoices(): Observable<ApiResponse<SubscriptionInvoice[]>> {
    return this.http.get<ApiResponse<SubscriptionInvoice[]>>(`${this.baseUrl}/invoices`);
  }
}

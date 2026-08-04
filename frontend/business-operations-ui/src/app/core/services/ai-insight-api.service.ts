import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { BusinessInsightResponse } from '../../features/ai-insights/models/business-insight-response.model';
import { AiSummaryResponse } from '../../features/ai-insights/models/ai-summary-response.model';
import { AiChatMessageResponse } from '../../features/ai-insights/models/ai-chat-message-response.model';

@Injectable({
  providedIn: 'root'
})
export class AiInsightApiService {
  private http = inject(HttpClient);
  private baseUrl = `${API_CONFIG.baseUrl}/ai`;

  getInsights(): Observable<ApiResponse<BusinessInsightResponse[]>> {
    return this.http.get<ApiResponse<BusinessInsightResponse[]>>(`${this.baseUrl}/insights`);
  }

  getDigest(): Observable<ApiResponse<AiSummaryResponse>> {
    return this.http.get<ApiResponse<AiSummaryResponse>>(`${this.baseUrl}/digest`);
  }

  askAiAssistant(message: string, sessionId?: string): Observable<ApiResponse<AiChatMessageResponse>> {
    return this.http.post<ApiResponse<AiChatMessageResponse>>(`${this.baseUrl}/chat`, { message, sessionId });
  }

  getChatHistory(): Observable<ApiResponse<AiChatMessageResponse[]>> {
    return this.http.get<ApiResponse<AiChatMessageResponse[]>>(`${this.baseUrl}/history`);
  }

  clearChatHistory(): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/history`);
  }
}

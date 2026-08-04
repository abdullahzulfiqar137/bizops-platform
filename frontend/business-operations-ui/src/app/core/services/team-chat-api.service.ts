import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';

export interface DirectChatUser {
  id: string;
  name: string;
  role: string;
  status: 'online' | 'away' | 'offline';
  avatarColor: string;
  unreadCount: number;
}

export interface ChatApiMessage {
  id: string;
  targetId: string;
  senderId: string;
  senderName: string;
  senderRole: string;
  avatarColor: string;
  timestampUtc: string;
  content: string;
  isSelf: boolean;
}

@Injectable({ providedIn: 'root' })
export class TeamChatApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${API_CONFIG.baseUrl}/TeamChat`;

  getDirectUsers(): Observable<ApiResponse<DirectChatUser[]>> {
    return this.http.get<ApiResponse<DirectChatUser[]>>(`${this.baseUrl}/direct-users`);
  }

  getMessages(targetId: string): Observable<ApiResponse<ChatApiMessage[]>> {
    return this.http.get<ApiResponse<ChatApiMessage[]>>(`${this.baseUrl}/messages/${encodeURIComponent(targetId)}`);
  }

  sendMessage(targetId: string, content: string): Observable<ApiResponse<ChatApiMessage>> {
    return this.http.post<ApiResponse<ChatApiMessage>>(`${this.baseUrl}/messages`, { targetId, content });
  }
}

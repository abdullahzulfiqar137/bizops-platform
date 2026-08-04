import { Injectable, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { API_CONFIG } from '../config/api-config';
import { TokenStorageService } from './token-storage.service';
import { NotificationService } from './notification.service';
import { Subject } from 'rxjs';

export interface RealTimeChatMessage {
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

@Injectable({
  providedIn: 'root',
})
export class SignalRNotificationService {
  private tokenStorage = inject(TokenStorageService);
  private uiNotifications = inject(NotificationService);

  private hubConnection?: signalR.HubConnection;
  public realTimeAlert$ = new Subject<{ title: string; message: string; type: string }>();
  public teamChatMessage$ = new Subject<RealTimeChatMessage>();

  public startConnection(): void {
    if (this.hubConnection) return;
    const token = this.tokenStorage.getAccessToken();
    if (!token) return;

    const hostUrl = API_CONFIG.baseUrl.replace(/\/api.*$/, '');
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${hostUrl}/hubs/notifications`, {
        accessTokenFactory: () => this.tokenStorage.getAccessToken() || '',
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('⚡ SignalR WebSocket Real-Time Connection Established'))
      .catch((err) => console.warn('SignalR WebSocket connection warning:', err));

    this.registerEventHandlers();
  }

  private registerEventHandlers(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('ReceiveNotification', (data: { title: string; message: string; type: string }) => {
      this.realTimeAlert$.next(data);
      if (data.type === 'danger' || data.type === 'error') {
        this.uiNotifications.error(`${data.title}: ${data.message}`);
      } else if (data.type === 'warning') {
        this.uiNotifications.warning(`${data.title}: ${data.message}`);
      } else {
        this.uiNotifications.success(`${data.title}: ${data.message}`);
      }
    });

    this.hubConnection.on('TeamChatMessage', (message: RealTimeChatMessage) => {
      this.teamChatMessage$.next(message);
    });
  }

  public stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = undefined;
    }
  }
}

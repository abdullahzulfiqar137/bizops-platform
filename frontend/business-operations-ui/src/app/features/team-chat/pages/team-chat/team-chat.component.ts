import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '@core/services/notification.service';
import { ChatApiMessage, TeamChatApiService } from '../../../../core/services/team-chat-api.service';
import { SignalRNotificationService } from '../../../../core/services/signalr-notification.service';
import { Subscription } from 'rxjs';

export interface TeamChatMember {
  id: string;
  name: string;
  role: string;
  roleBadge: string;
  status: 'online' | 'away' | 'offline';
  avatarColor: string;
}

export interface ChatMessage {
  id: string;
  senderId: string;
  senderName: string;
  senderRole: string;
  avatarColor: string;
  timestamp: string;
  content: string;
  isSelf: boolean;
}

@Component({
  selector: 'app-team-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './team-chat.component.html',
  styleUrl: './team-chat.component.css'
})
export class TeamChatComponent implements OnInit, OnDestroy {
  private notifications = inject(NotificationService);
  private chatApi = inject(TeamChatApiService);
  private signalR = inject(SignalRNotificationService);
  private realtimeSubscription?: Subscription;

  members: TeamChatMember[] = [];
  messages: Record<string, ChatMessage[]> = {};
  activeMemberId = '';

  newMessageText = '';

  ngOnInit(): void {
    this.realtimeSubscription = this.signalR.teamChatMessage$.subscribe(message => {
      if (message.targetId !== this.activeMemberId || message.isSelf) return;
      const conversation = this.messages[message.targetId] ??= [];
      if (!conversation.some(existing => existing.id === message.id)) {
        conversation.push(this.mapMessage(message));
      }
    });
    this.chatApi.getDirectUsers().subscribe({
      next: response => {
        this.members = (response.data ?? []).map(member => ({
          id: member.id,
          name: member.name,
          role: member.role,
          roleBadge: member.role,
          status: member.status,
          avatarColor: member.avatarColor
        }));
        if (this.members.length) this.selectMember(this.members[0].id);
      },
      error: () => this.notifications.error('Team members could not be loaded from the server.')
    });
  }

  ngOnDestroy(): void {
    this.realtimeSubscription?.unsubscribe();
  }

  selectMember(id: string): void {
    this.activeMemberId = id;
    this.chatApi.getMessages(id).subscribe({
      next: response => this.messages[id] = (response.data ?? []).map(message => this.mapMessage(message)),
      error: () => this.notifications.error('Conversation history could not be loaded.')
    });
  }

  getActiveMember(): TeamChatMember | undefined {
    return this.members.find((m) => m.id === this.activeMemberId);
  }

  get currentMessages(): ChatMessage[] {
    return this.messages[this.activeMemberId] || [];
  }

  sendMessage(): void {
    if (!this.newMessageText.trim()) return;
    const content = this.newMessageText.trim();
    this.newMessageText = '';
    this.chatApi.sendMessage(this.activeMemberId, content).subscribe({
      next: response => {
        if (!response.data) return;
        if (!this.messages[this.activeMemberId]) this.messages[this.activeMemberId] = [];
        this.messages[this.activeMemberId].push(this.mapMessage(response.data));
      },
      error: () => {
        this.newMessageText = content;
        this.notifications.error('Message could not be sent.')
      }
    });
  }

  addQuickEmoji(emoji: string): void {
    this.newMessageText += ` ${emoji}`;
  }

  private mapMessage(message: ChatApiMessage): ChatMessage {
    return {
      id: message.id,
      senderId: message.senderId,
      senderName: message.senderName,
      senderRole: message.senderRole,
      avatarColor: message.avatarColor,
      timestamp: new Date(message.timestampUtc).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
      content: message.content,
      isSelf: message.isSelf
    };
  }
}

import { Component, ElementRef, HostListener, OnInit, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AiInsightApiService } from '../../../../core/services/ai-insight-api.service';
import { BusinessInsightResponse } from '../../models/business-insight-response.model';
import { AiSummaryResponse } from '../../models/ai-summary-response.model';

interface ChatMessage {
  sender: 'user' | 'assistant';
  text: string;
  time: Date;
}

@Component({
  selector: 'app-ai-insights-overview',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ai-insights-overview.component.html',
  styleUrl: './ai-insights-overview.component.css'
})
export class AiInsightsOverviewComponent implements OnInit {
  private aiApi = inject(AiInsightApiService);
  private router = inject(Router);
  @ViewChild('chatContainer') chatContainer!: ElementRef;

  insights: BusinessInsightResponse[] = [];
  digest: AiSummaryResponse | null = null;

  userQuery = '';
  isThinking = false;
  sessionId: string = crypto.randomUUID();
  showHistory = false;
  isLoadingHistory = false;
  historyGroups: { period: string; sessions: { sessionId: string; title: string; firstPrompt: string; startTime: Date; messages: ChatMessage[] }[] }[] = [];

  chatMessages: ChatMessage[] = [];

  ngOnInit(): void {
    this.loadAiData();
    this.startNewChat();
  }

  loadAiData(): void {
    this.aiApi.getInsights().subscribe(res => {
      if (res.success && res.data) this.insights = res.data;
    });

    this.aiApi.getDigest().subscribe(res => {
      if (res.success && res.data) this.digest = res.data;
    });
  }

  toggleHistory(): void {
    this.showHistory = !this.showHistory;
    if (this.showHistory) {
      this.loadHistoryFromDb();
    }
  }

  loadHistoryFromDb(): void {
    this.isLoadingHistory = true;
    this.aiApi.getChatHistory().subscribe({
      next: (res) => {
        this.isLoadingHistory = false;
        if (res.success && res.data) {
          this.processHistorySessions(res.data);
        }
      },
      error: () => {
        this.isLoadingHistory = false;
      }
    });
  }

  private processHistorySessions(rawItems: any[]): void {
    const sessionMap = new Map<string, any[]>();

    rawItems.forEach(item => {
      const sId = item.sessionId || item.id || 'legacy-session';
      if (!sessionMap.has(sId)) {
        sessionMap.set(sId, []);
      }
      sessionMap.get(sId)!.push(item);
    });

    const sessions: { sessionId: string; title: string; firstPrompt: string; startTime: Date; messages: ChatMessage[] }[] = [];

    sessionMap.forEach((items, sId) => {
      items.sort((a, b) => new Date(a.timestampUtc).getTime() - new Date(b.timestampUtc).getTime());
      
      const first = items[0];
      const firstPrompt = first.userPrompt || first.answer?.substring(0, 30) || 'Chat session';
      const title = firstPrompt.length > 34 ? firstPrompt.substring(0, 34) + '...' : firstPrompt;
      const startTime = new Date(first.timestampUtc);

      const chatMsgs: ChatMessage[] = [];
      items.forEach(it => {
        if (it.userPrompt) {
          chatMsgs.push({ sender: 'user', text: it.userPrompt, time: new Date(it.timestampUtc) });
        }
        if (it.answer) {
          chatMsgs.push({ sender: 'assistant', text: it.answer, time: new Date(it.timestampUtc) });
        }
      });

      sessions.push({
        sessionId: sId,
        title,
        firstPrompt,
        startTime,
        messages: chatMsgs
      });
    });

    sessions.sort((a, b) => b.startTime.getTime() - a.startTime.getTime());

    const todaySessions: typeof sessions = [];
    const yesterdaySessions: typeof sessions = [];
    const olderSessions: typeof sessions = [];

    const now = new Date();
    const todayStr = now.toDateString();
    
    const yesterday = new Date(now);
    yesterday.setDate(now.getDate() - 1);
    const yesterdayStr = yesterday.toDateString();

    sessions.forEach(sess => {
      const dateStr = sess.startTime.toDateString();
      if (dateStr === todayStr) {
        todaySessions.push(sess);
      } else if (dateStr === yesterdayStr) {
        yesterdaySessions.push(sess);
      } else {
        olderSessions.push(sess);
      }
    });

    const groups: { period: string; sessions: typeof sessions }[] = [];
    if (todaySessions.length > 0) groups.push({ period: 'Today', sessions: todaySessions });
    if (yesterdaySessions.length > 0) groups.push({ period: 'Yesterday', sessions: yesterdaySessions });
    if (olderSessions.length > 0) groups.push({ period: 'Older', sessions: olderSessions });

    this.historyGroups = groups;
  }

  selectHistorySession(session: { sessionId: string; title: string; firstPrompt: string; startTime: Date; messages: ChatMessage[] }): void {
    this.sessionId = session.sessionId;
    this.chatMessages = [...session.messages];
    this.showHistory = false;
    this.scrollToBottom();
  }

  startNewChat(): void {
    this.sessionId = crypto.randomUUID();
    this.chatMessages = [
      {
        sender: 'assistant',
        text: 'Hello there! 👋 I am your **Universal AI Assistant**.\n\nAsk me ANYTHING — whether it\'s about business stats, platform guidance, coding, mathematics, or general questions! All history is saved to the database.',
        time: new Date()
      }
    ];
    this.showHistory = false;
  }

  clearChat(): void {
    this.startNewChat();
  }

  clearAllDbHistory(): void {
    if (confirm('Are you sure you want to permanently delete all chat history from the Database?')) {
      this.aiApi.clearChatHistory().subscribe({
        next: () => {
          this.historyGroups = [];
          this.startNewChat();
        }
      });
    }
  }

  sendMessage(): void {
    if (!this.userQuery.trim() || this.isThinking) return;

    const query = this.userQuery.trim();
    this.chatMessages.push({
      sender: 'user',
      text: query,
      time: new Date()
    });

    this.userQuery = '';
    this.isThinking = true;
    this.scrollToBottom();

    this.aiApi.askAiAssistant(query, this.sessionId).subscribe({
      next: (res) => {
        this.isThinking = false;
        if (res.success && res.data) {
          this.chatMessages.push({
            sender: 'assistant',
            text: res.data.answer,
            time: new Date(res.data.timestampUtc)
          });
        }
        this.scrollToBottom();
      },
      error: (error: { error?: { message?: string } }) => {
        this.isThinking = false;
        this.chatMessages.push({
          sender: 'assistant',
          text: error.error?.message ?? 'Unable to reach the AI assistant. Please check that the backend is running.',
          time: new Date()
        });
        this.scrollToBottom();
      }
    });
  }

  sendQuickPrompt(promptText: string): void {
    this.userQuery = promptText;
    this.sendMessage();
  }

  formatMessageText(text: string): string {
    if (!text) return '';
    let formatted = text.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
    formatted = formatted.replace(
      /\[([^\]]+)\]\(([^)]+)\)/g,
      (match, label, route) => {
        let cleanRoute = route.trim();
        if (cleanRoute.includes('localhost') || cleanRoute.includes('http')) {
          try {
            cleanRoute = new URL(cleanRoute).pathname;
          } catch {}
        }
        if (!cleanRoute.startsWith('/')) {
          cleanRoute = '/' + cleanRoute;
        }
        return `<a href="javascript:void(0)" class="ai-nav-link-btn" data-route="${cleanRoute}">${label} ➔</a>`;
      }
    );
    formatted = formatted.replace(/\n/g, '<br/>');
    return formatted;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    const linkBtn = target?.closest('.ai-nav-link-btn') as HTMLElement;
    if (linkBtn) {
      event.preventDefault();
      event.stopPropagation();
      const route = linkBtn.getAttribute('data-route');
      if (route) {
        this.router.navigateByUrl(route);
      }
    }
  }

  private scrollToBottom(): void {
    const scroll = () => {
      if (this.chatContainer?.nativeElement) {
        const el = this.chatContainer.nativeElement;
        el.scrollTop = el.scrollHeight;
      }
    };
    requestAnimationFrame(scroll);
    setTimeout(scroll, 50);
    setTimeout(scroll, 150);
    setTimeout(scroll, 350);
  }

  getSeverityBadge(severity: number): string {
    switch (severity) {
      case 3: return 'badge-danger';
      case 2: return 'badge-warning';
      default: return 'badge-info';
    }
  }
}

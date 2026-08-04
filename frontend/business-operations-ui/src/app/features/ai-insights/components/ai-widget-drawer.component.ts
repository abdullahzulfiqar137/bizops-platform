import { Component, ElementRef, HostListener, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { AiInsightApiService } from '@core/services/ai-insight-api.service';

interface ChatMessage {
  sender: 'user' | 'assistant';
  text: string;
  time: Date;
}

@Component({
  selector: 'app-ai-widget-drawer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ai-widget-drawer.component.html',
  styleUrl: './ai-widget-drawer.component.css'
})
export class AiWidgetDrawerComponent {
  private aiApi = inject(AiInsightApiService);
  private router = inject(Router);

  @ViewChild('chatContainer') private chatContainer?: ElementRef;
  @ViewChild('fabElement') private fabElement?: ElementRef;

  // Draggable position coordinates
  posX = window.innerWidth - 100;
  posY = window.innerHeight - 120;
  isDragging = false;
  isAiInsightsPage = false;
  private dragStartX = 0;
  private dragStartY = 0;
  private currentPointerId: number | null = null;
  private hasMoved = false;

  isOpen = false;
  isSending = false;
  userQuestion = '';
  sessionId: string = crypto.randomUUID();
  showHistory = false;
  isLoadingHistory = false;
  historyGroups: { period: string; sessions: { sessionId: string; title: string; firstPrompt: string; startTime: Date; messages: ChatMessage[] }[] }[] = [];

  messages: ChatMessage[] = [
    {
      sender: 'assistant',
      text: '🤖 Greetings! I am your **BizOps AI Executive Robot Assistant**. Ask me anything about sales, inventory, financial positions, or business growth tips!',
      time: new Date()
    }
  ];

  constructor() {
    this.router.events.pipe(filter((e) => e instanceof NavigationEnd)).subscribe(() => {
      this.checkCurrentRoute();
    });
    this.checkCurrentRoute();
  }

  private checkCurrentRoute(): void {
    this.isAiInsightsPage = this.router.url.includes('/ai-insights');
  }

  // POINTER DRAG HANDLERS (EXPLICIT RELEASE & PASS-THROUGH TARGETING)
  onPointerDown(event: PointerEvent): void {
    this.isDragging = true;
    this.hasMoved = false;
    this.currentPointerId = event.pointerId;
    this.dragStartX = event.clientX - this.posX;
    this.dragStartY = event.clientY - this.posY;
    try {
      (event.currentTarget as HTMLElement)?.setPointerCapture(event.pointerId);
    } catch {}
  }

  @HostListener('document:pointermove', ['$event'])
  onPointerMove(event: PointerEvent): void {
    if (!this.isDragging) return;
    const newX = event.clientX - this.dragStartX;
    const newY = event.clientY - this.dragStartY;

    if (Math.abs(newX - this.posX) > 3 || Math.abs(newY - this.posY) > 3) {
      this.hasMoved = true;
    }

    // Keep robot button within screen bounds
    const maxX = window.innerWidth - 70;
    const maxY = window.innerHeight - 80;
    this.posX = Math.max(10, Math.min(maxX, newX));
    this.posY = Math.max(10, Math.min(maxY, newY));
  }

  @HostListener('document:pointerup', ['$event'])
  @HostListener('document:pointercancel', ['$event'])
  @HostListener('document:mouseup')
  onPointerUp(event?: PointerEvent): void {
    if (this.isDragging) {
      this.isDragging = false;
      if (event && this.currentPointerId !== null) {
        try {
          (event.currentTarget as HTMLElement)?.releasePointerCapture(this.currentPointerId);
        } catch {}
        this.currentPointerId = null;
      }
    }
  }

  onFabClick(event: MouseEvent): void {
    // Only toggle if user clicked without dragging
    if (!this.hasMoved) {
      this.toggleWidget();
    }
  }

  toggleWidget(): void {
    this.isOpen = !this.isOpen;
    if (this.isOpen) {
      setTimeout(() => this.scrollToBottom(), 100);
    }
  }

  closeWidget(): void {
    this.isOpen = false;
  }

  openFullAiPage(): void {
    this.isOpen = false;
    this.router.navigate(['/ai-insights']);
  }

  sendQuickPrompt(promptText: string): void {
    this.userQuestion = promptText;
    this.sendMessage();
  }

  sendMessage(): void {
    const text = this.userQuestion.trim();
    if (!text || this.isSending) return;

    this.messages.push({
      sender: 'user',
      text: text,
      time: new Date()
    });

    this.userQuestion = '';
    this.isSending = true;
    this.scrollToBottom();

    this.aiApi.askAiAssistant(text, this.sessionId).subscribe({
      next: (res) => {
        this.isSending = false;
        if (res.success && res.data) {
          this.messages.push({
            sender: 'assistant',
            text: res.data.answer,
            time: new Date()
          });
        } else {
          this.messages.push({
            sender: 'assistant',
            text: 'I apologize, I could not retrieve an answer right now. Please try again.',
            time: new Date()
          });
        }
        this.scrollToBottom();
      },
      error: (error: { error?: { message?: string } }) => {
        this.isSending = false;
        this.messages.push({
          sender: 'assistant',
          text: error.error?.message ?? 'Unable to reach the AI assistant. Please check that the backend is running.',
          time: new Date()
        });
        this.scrollToBottom();
      }
    });
  }

  formatMessage(text: string): string {
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
        this.isOpen = false;
        this.router.navigateByUrl(route);
      }
    }
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
    this.messages = [...session.messages];
    this.showHistory = false;
    this.scrollToBottom();
  }

  startNewChat(): void {
    this.sessionId = crypto.randomUUID();
    this.messages = [
      {
        sender: 'assistant',
        text: '🤖 Greetings! I am your **BizOps AI Executive Robot Assistant**. Ask me anything about sales, inventory, financial positions, or business growth tips!',
        time: new Date()
      }
    ];
    this.showHistory = false;
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

  private scrollToBottom(): void {
    const scroll = () => {
      if (this.chatContainer?.nativeElement) {
        const el = this.chatContainer.nativeElement;
        el.scrollTop = el.scrollHeight;
      }
    };
    // Multi-stage scroll to ensure long innerHTML responses scroll to the absolute bottom
    requestAnimationFrame(scroll);
    setTimeout(scroll, 50);
    setTimeout(scroll, 150);
    setTimeout(scroll, 350);
  }
}

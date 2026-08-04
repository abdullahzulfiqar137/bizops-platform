import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG } from '../../../core/config/api-config';

@Component({
  selector: 'app-notification-center',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-center.component.html',
  styleUrl: './notification-center.component.css'
})
export class NotificationCenterComponent implements OnInit {
  private http = inject(HttpClient);
  private base = `${API_CONFIG.baseUrl}/notifications`;
  
  items: any[] = [];
  loading = true;
  filter: 'all' | 'unread' = 'all';

  get unread(): number {
    return this.items.filter(n => !n.isRead).length;
  }

  get visible(): any[] {
    return this.filter === 'unread' ? this.items.filter(n => !n.isRead) : this.items;
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.http.get<any>(this.base).subscribe({
      next: r => {
        this.items = r.data?.items ?? r.data?.notifications ?? r.data ?? [];
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  read(n: any): void {
    if (n.isRead) return;
    this.http.post(`${this.base}/${n.id}/read`, {}).subscribe(() => {
      n.isRead = true;
    });
  }

  readAll(): void {
    this.http.post(`${this.base}/read-all`, {}).subscribe(() => 
      this.items.forEach(n => n.isRead = true)
    );
  }
}

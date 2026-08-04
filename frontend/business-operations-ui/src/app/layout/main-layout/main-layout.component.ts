import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';
import { BreadcrumbComponent } from '../breadcrumb/breadcrumb.component';
import { AiWidgetDrawerComponent } from '../../features/ai-insights/components/ai-widget-drawer.component';
import { DataExportComponent } from '../../shared/components/data-export/data-export.component';
import { SignalRNotificationService } from '../../core/services/signalr-notification.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    SidebarComponent,
    TopbarComponent,
    BreadcrumbComponent,
    AiWidgetDrawerComponent,
    DataExportComponent
  ],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css'
})
export class MainLayoutComponent implements OnInit, OnDestroy {
  private readonly signalR = inject(SignalRNotificationService);
  sidebarOpen = false;
  sidebarCollapsed = false;

  ngOnInit(): void {
    this.signalR.startConnection();
  }

  ngOnDestroy(): void {
    this.signalR.stopConnection();
  }

  handleSidebarToggle(): void {
    if (window.innerWidth <= 768) {
      this.sidebarCollapsed = false;
      this.sidebarOpen = !this.sidebarOpen;
      return;
    }
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  closeMobileSidebar(): void {
    this.sidebarOpen = false;
  }
}

import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuditLogApiService } from '../../../../core/services/audit-log-api.service';
import { AuditLogResponse } from '../../models/audit-log-response.model';

@Component({
  selector: 'app-audit-logs-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './audit-logs-list.component.html',
  styleUrl: './audit-logs-list.component.css'
})
export class AuditLogsListComponent implements OnInit {
  private auditApi = inject(AuditLogApiService);

  auditLogs: AuditLogResponse[] = [];
  entityFilter = '';
  actionFilter = '';

  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  selectedLog: AuditLogResponse | null = null;

  ngOnInit(): void {
    this.loadAuditLogs();
  }

  loadAuditLogs(): void {
    this.auditApi.getAuditLogs(
      this.entityFilter || undefined,
      undefined, undefined,
      this.actionFilter || undefined,
      undefined, undefined,
      this.currentPage, this.pageSize
    ).subscribe(res => {
      if (res.success) {
        this.auditLogs = res.data;
        this.currentPage = res.metadata.currentPage;
        this.totalPages = res.metadata.totalPages;
        this.totalCount = res.metadata.totalCount;
      }
    });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadAuditLogs();
    }
  }

  viewDiff(log: AuditLogResponse): void {
    this.selectedLog = log;
  }

  closeDiffModal(): void {
    this.selectedLog = null;
  }
}

import { Component, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-data-export',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './data-export.component.html',
  styleUrl: './data-export.component.css'
})
export class DataExportComponent implements OnDestroy {
  private router = inject(Router);
  private observer?: MutationObserver;
  hasTable = false;

  constructor() {
    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(() => this.watchPage());
    queueMicrotask(() => this.watchPage());
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
  }

  exportCsv(): void {
    const rows = this.readTable();
    if (!rows.length) return;
    const csv = rows.map(row => row.map(value => `"${value.replace(/"/g, '""')}"`).join(',')).join('\r\n');
    this.download(new Blob(['\uFEFF', csv], { type: 'text/csv;charset=utf-8' }), 'csv');
  }

  exportExcel(): void {
    const table = document.querySelector<HTMLTableElement>('.page-container table');
    if (!table) return;
    const documentHtml = `<html><head><meta charset="utf-8"></head><body>${table.outerHTML}</body></html>`;
    this.download(new Blob([documentHtml], { type: 'application/vnd.ms-excel' }), 'xls');
  }

  private watchPage(): void {
    this.observer?.disconnect();
    const host = document.querySelector('.page-container');
    if (!host) return;
    const update = () => this.hasTable = !!host.querySelector('table');
    update();
    this.observer = new MutationObserver(update);
    this.observer.observe(host, { childList: true, subtree: true });
  }

  private readTable(): string[][] {
    const table = document.querySelector<HTMLTableElement>('.page-container table');
    if (!table) return [];
    return Array.from(table.rows).map(row =>
      Array.from(row.cells)
        .filter(cell => cell.textContent?.trim().toLowerCase() !== 'actions')
        .map(cell => cell.textContent?.replace(/\s+/g, ' ').trim() ?? '')
    );
  }

  private download(blob: Blob, extension: string): void {
    const routeName = this.router.url.split('?')[0].split('/').filter(Boolean).pop() || 'export';
    const date = new Date().toISOString().slice(0, 10);
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `${routeName}-${date}.${extension}`;
    link.click();
    URL.revokeObjectURL(link.href);
  }
}

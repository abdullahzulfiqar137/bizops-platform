import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import {
  DatabaseBackup,
  DatabaseBackupApiService
} from '../../../../core/services/database-backup-api.service';

@Component({
  selector: 'app-database-backups',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './database-backups.component.html',
  styleUrl: './database-backups.component.css'
})
export class DatabaseBackupsComponent implements OnInit {
  private api = inject(DatabaseBackupApiService);

  backups: DatabaseBackup[] = [];
  loading = false;
  creating = false;
  restoring = false;
  message = '';
  hasError = false;
  restoreTarget: DatabaseBackup | null = null;
  restoreConfirmation = '';

  get totalSize(): number {
    return this.backups.reduce((total, backup) => total + backup.sizeBytes, 0);
  }

  ngOnInit(): void {
    this.loadBackups();
  }

  loadBackups(): void {
    this.loading = true;
    this.api.getBackups().pipe(finalize(() => this.loading = false)).subscribe({
      next: response => {
        this.backups = [...(response.data ?? [])].sort((a, b) =>
          new Date(b.createdAtUtc).getTime() - new Date(a.createdAtUtc).getTime());
      },
      error: () => this.showMessage('Backups could not be loaded. Confirm that the API is running and you have Admin access.', true)
    });
  }

  createBackup(): void {
    this.creating = true;
    this.api.triggerBackup().pipe(finalize(() => this.creating = false)).subscribe({
      next: response => {
        this.showMessage(response.message || 'Backup created successfully.');
        this.loadBackups();
      },
      error: () => this.showMessage('The backup could not be created.', true)
    });
  }

  downloadBackup(backup: DatabaseBackup): void {
    this.api.downloadBackup(backup.fileName).subscribe({
      next: blob => {
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = backup.fileName;
        link.click();
        URL.revokeObjectURL(link.href);
      },
      error: () => this.showMessage('The backup download failed.', true)
    });
  }

  openRestore(backup: DatabaseBackup): void {
    this.restoreTarget = backup;
    this.restoreConfirmation = '';
  }

  closeRestore(): void {
    if (this.restoring) return;
    this.restoreTarget = null;
    this.restoreConfirmation = '';
  }

  restoreBackup(): void {
    if (!this.restoreTarget || this.restoreConfirmation !== 'RESTORE') return;
    this.restoring = true;
    this.api.restoreBackup(this.restoreTarget.fileName).pipe(finalize(() => this.restoring = false)).subscribe({
      next: response => {
        this.showMessage(response.message || 'Database restored successfully.');
        this.closeRestore();
      },
      error: () => this.showMessage('The database restore failed. The current database was not changed.', true)
    });
  }

  formatBytes(bytes: number): string {
    if (!bytes) return '0 B';
    const units = ['B', 'KB', 'MB', 'GB', 'TB'];
    const index = Math.min(Math.floor(Math.log(bytes) / Math.log(1024)), units.length - 1);
    return `${(bytes / Math.pow(1024, index)).toFixed(index ? 1 : 0)} ${units[index]}`;
  }

  private showMessage(message: string, error = false): void {
    this.message = message;
    this.hasError = error;
  }
}

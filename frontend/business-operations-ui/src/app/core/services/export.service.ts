import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ExportService {
  /**
   * Export JSON array to downloadable CSV file
   */
  exportToCsv(filename: string, rows: Record<string, any>[]): void {
    if (!rows || !rows.length) return;

    const headers = Object.keys(rows[0]);
    const csvContent = [
      headers.join(','),
      ...rows.map(row =>
        headers
          .map(header => {
            const val = row[header] !== undefined && row[header] !== null ? String(row[header]) : '';
            // Escape double quotes & wrap values with commas in quotes
            const escaped = val.replace(/"/g, '""');
            return `"${escaped}"`;
          })
          .join(',')
      )
    ].join('\r\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);

    link.setAttribute('href', url);
    link.setAttribute('download', `${filename}_${new Date().toISOString().slice(0, 10)}.csv`);
    link.style.visibility = 'hidden';

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}

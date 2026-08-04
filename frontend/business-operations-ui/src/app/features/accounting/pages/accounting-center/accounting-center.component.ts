import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin, finalize } from 'rxjs';
import { BusinessCurrencyPipe } from '@core/pipes/business-currency.pipe';
import { NotificationService } from '@core/services/notification.service';
import { AccountResponse, BalanceSheetResponse, JournalResponse, LedgerLineResponse, TrialBalanceResponse } from '../../models/accounting.model';
import { AccountingApiService } from '../../../../core/services/accounting-api.service';

@Component({
  selector: 'app-accounting-center',
  standalone: true,
  imports: [CommonModule, FormsModule, BusinessCurrencyPipe],
  templateUrl: './accounting-center.component.html',
  styleUrl: './accounting-center.component.css'
})
export class AccountingCenterComponent implements OnInit {
  private api = inject(AccountingApiService);
  private notifications = inject(NotificationService);

  accounts: AccountResponse[] = [];
  journals: JournalResponse[] = [];
  trial: TrialBalanceResponse | null = null;
  balance: BalanceSheetResponse | null = null;
  ledger: LedgerLineResponse[] = [];
  selectedAccount: AccountResponse | null = null;

  tab: 'trial' | 'journals' | 'accounts' | 'position' = 'trial';
  loading = false;
  syncing = false;
  error = '';
  showJournal = false;
  savingJournal = false;
  journalDescription = '';
  journalLines = [this.blankLine(), this.blankLine()];

  get journalDebits() {
    return this.journalLines.reduce((s, x) => s + (+x.debit || 0), 0);
  }

  get journalCredits() {
    return this.journalLines.reduce((s, x) => s + (+x.credit || 0), 0);
  }

  get journalBalanced() {
    return (
      this.journalDebits > 0 &&
      Math.abs(this.journalDebits - this.journalCredits) < 0.01 &&
      this.journalLines.every((x) => x.accountId && ((x.debit > 0) !== (x.credit > 0)))
    );
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';
    forkJoin({
      accounts: this.api.accounts(),
      journals: this.api.journals(),
      trial: this.api.trialBalance(),
      balance: this.api.balanceSheet(),
    })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (r) => {
          this.accounts = r.accounts.data ?? [];
          this.journals = r.journals.data ?? [];
          this.trial = r.trial.data;
          this.balance = r.balance.data;
        },
        error: () => (this.error = 'Check the backend connection and accounting migration.'),
      });
  }

  syncEntries(): void {
    this.syncing = true;
    this.api.synchronize().pipe(finalize(() => (this.syncing = false))).subscribe({
      next: (res) => {
        this.notifications.success(res.message || 'Double-entry accounting synchronized.');
        this.load();
      },
      error: () => this.notifications.error('Failed to sync automatic journal entries.')
    });
  }

  accountType(x: number): string {
    return ['', 'Asset', 'Liability', 'Equity', 'Revenue', 'Expense'][x] ?? 'Unknown';
  }

  viewLedger(a: AccountResponse): void {
    this.selectedAccount = a;
    this.api.ledger(a.id).subscribe((r) => (this.ledger = r.data ?? []));
  }

  openJournal(): void {
    this.journalDescription = '';
    this.journalLines = [this.blankLine(), this.blankLine()];
    this.showJournal = true;
  }

  addLine(): void {
    this.journalLines.push(this.blankLine());
  }

  removeLine(i: number): void {
    if (this.journalLines.length > 2) this.journalLines.splice(i, 1);
  }

  saveJournal(): void {
    if (!this.journalBalanced || this.savingJournal) return;
    this.savingJournal = true;

    this.api
      .createJournal({
        entryDateUtc: null,
        description: this.journalDescription,
        lines: this.journalLines.map((x) => ({
          accountId: x.accountId,
          debit: +x.debit || 0,
          credit: +x.credit || 0,
          memo: null,
        })),
      })
      .pipe(finalize(() => (this.savingJournal = false)))
      .subscribe({
        next: (r) => {
          if (r.success) {
            this.showJournal = false;
            this.notifications.success('Journal entry posted successfully.');
            this.load();
          } else {
            this.notifications.error(r.message);
          }
        },
        error: (e) => this.notifications.error(e.error?.message || 'Journal could not be posted.'),
      });
  }

  private blankLine() {
    return { accountId: '', debit: 0, credit: 0 };
  }
}

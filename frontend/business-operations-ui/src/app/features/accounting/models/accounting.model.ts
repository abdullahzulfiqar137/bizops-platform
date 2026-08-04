export interface AccountResponse {
  id: string;
  code: string;
  name: string;
  accountCode: string;
  accountName: string;
  type: any;
  category: string;
  balance: number;
  currency: string;
  status: string;
  isSystem?: boolean;
}

export interface JournalResponse {
  id: string;
  entryNumber: string;
  entryDateUtc: string;
  date: string;
  description: string;
  reference: string;
  referenceType: string;
  total: number;
  totalDebit: number;
  totalCredit: number;
  status: string;
  postedBy: string;
}

export interface LedgerLineResponse {
  id: string;
  dateUtc: string;
  transactionDate: string;
  entryNumber: string;
  description: string;
  reference: string;
  debit: number;
  credit: number;
  balance: number;
  runningBalance: number;
}

export interface BalanceSheetResponse {
  assets: number;
  liabilities: number;
  equity: number;
  retainedEarnings: number;
  liabilitiesAndEquity: number;
  totalAssets: number;
  totalLiabilities: number;
  totalEquity: number;
  isBalanced: boolean;
  asOfUtc: string;
}

export interface TrialBalanceResponse {
  lines: any[];
  accounts: any[];
  totalDebits: number;
  totalCredits: number;
  totalDebit: number;
  totalCredit: number;
  isBalanced: boolean;
  asOfUtc: string;
}

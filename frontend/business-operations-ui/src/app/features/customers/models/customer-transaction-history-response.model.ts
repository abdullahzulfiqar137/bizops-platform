export interface CustomerTransactionHistoryResponse {
  transactionId: string;
  transactionType: string;
  referenceNumber: string;
  amount: number;
  balanceAfter: number;
  transactionDateUtc: string;
}

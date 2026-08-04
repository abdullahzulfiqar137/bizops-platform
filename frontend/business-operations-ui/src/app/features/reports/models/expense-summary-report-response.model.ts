export interface ExpenseCategoryBreakdown {
  categoryId: string;
  categoryName: string;
  totalAmount: number;
  percentageOfTotal: number;
}

export interface ExpenseSummaryReportResponse {
  totalExpensesCount: number;
  totalExpenseAmount: number;
  categoryBreakdown: ExpenseCategoryBreakdown[];
}

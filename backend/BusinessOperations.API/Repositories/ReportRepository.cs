using System.Globalization;
using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Reports;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Repositories;

public sealed class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var activeSalesQuery = _context.Sales.Where(s => s.Status != SaleStatus.Cancelled);
        var activePurchasesQuery = _context.Purchases.Where(p => p.Status != PurchaseStatus.Cancelled);

        var totalSalesRevenue = await activeSalesQuery.SumAsync(s => s.TotalAmount, cancellationToken);
        var totalPurchases = await activePurchasesQuery.SumAsync(p => p.TotalAmount, cancellationToken);
        var totalExpenses = await _context.Expenses.SumAsync(e => e.Amount, cancellationToken);

        var totalReceivables = await _context.Customers.SumAsync(c => c.CurrentBalance, cancellationToken);
        var totalPayables = await _context.Suppliers.SumAsync(s => s.CurrentBalance, cancellationToken);

        var lowStockCount = await _context.Products.CountAsync(p => p.StockQuantity <= p.MinimumStockLevel, cancellationToken);
        var totalInventoryValue = await _context.Products.SumAsync(p => p.StockQuantity * p.CostPrice, cancellationToken);

        var netProfit = totalSalesRevenue - (totalPurchases + totalExpenses);

        return new DashboardSummaryResponse
        {
            TotalSalesRevenue = totalSalesRevenue,
            TotalPurchases = totalPurchases,
            TotalExpenses = totalExpenses,
            NetProfit = netProfit,
            TotalReceivables = totalReceivables,
            TotalPayables = totalPayables,
            LowStockCount = lowStockCount,
            TotalInventoryValue = totalInventoryValue
        };
    }

    public async Task<SalesSummaryReportResponse> GetSalesSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.Where(s => s.Status != SaleStatus.Cancelled).AsNoTracking();

        if (startDate.HasValue)
        {
            query = query.Where(s => s.SaleDateUtc >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(s => s.SaleDateUtc <= endDate.Value);
        }

        var invoices = await query.ToListAsync(cancellationToken);

        var totalInvoices = invoices.Count;
        var grossSales = invoices.Sum(s => s.SubTotal);
        var totalTaxAmount = invoices.Sum(s => s.TaxAmount);
        var totalDiscountAmount = invoices.Sum(s => s.DiscountAmount);
        var netSalesRevenue = invoices.Sum(s => s.TotalAmount);
        var totalPaidAmount = invoices.Sum(s => s.PaidAmount);
        var totalDueAmount = invoices.Sum(s => s.DueAmount);
        var averageOrderValue = totalInvoices > 0 ? Math.Round(netSalesRevenue / totalInvoices, 2) : 0.00m;

        var monthlyBreakdown = invoices
            .GroupBy(s => new { s.SaleDateUtc.Year, s.SaleDateUtc.Month })
            .Select(g => new MonthlySalesSummary
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                InvoiceCount = g.Count(),
                TotalRevenue = g.Sum(s => s.TotalAmount)
            })
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();

        return new SalesSummaryReportResponse
        {
            TotalInvoices = totalInvoices,
            GrossSales = grossSales,
            TotalTaxAmount = totalTaxAmount,
            TotalDiscountAmount = totalDiscountAmount,
            NetSalesRevenue = netSalesRevenue,
            TotalPaidAmount = totalPaidAmount,
            TotalDueAmount = totalDueAmount,
            AverageOrderValue = averageOrderValue,
            MonthlyBreakdown = monthlyBreakdown
        };
    }

    public async Task<PurchaseSummaryReportResponse> GetPurchaseSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        var query = _context.Purchases.Where(p => p.Status != PurchaseStatus.Cancelled).AsNoTracking();

        if (startDate.HasValue)
        {
            query = query.Where(p => p.PurchaseDateUtc >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(p => p.PurchaseDateUtc <= endDate.Value);
        }

        var purchases = await query.ToListAsync(cancellationToken);

        var totalPOs = purchases.Count;
        var grossPurchases = purchases.Sum(p => p.SubTotal);
        var totalTaxAmount = purchases.Sum(p => p.TaxAmount);
        var totalDiscountAmount = purchases.Sum(p => p.DiscountAmount);
        var netPurchaseSpend = purchases.Sum(p => p.TotalAmount);
        var totalPaidAmount = purchases.Sum(p => p.PaidAmount);
        var totalDueAmount = purchases.Sum(p => p.DueAmount);

        return new PurchaseSummaryReportResponse
        {
            TotalPurchaseOrders = totalPOs,
            GrossPurchases = grossPurchases,
            TotalTaxAmount = totalTaxAmount,
            TotalDiscountAmount = totalDiscountAmount,
            NetPurchaseSpend = netPurchaseSpend,
            TotalPaidAmount = totalPaidAmount,
            TotalDueAmount = totalDueAmount
        };
    }

    public async Task<ExpenseSummaryReportResponse> GetExpenseSummaryAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        var query = _context.Expenses.Include(e => e.ExpenseCategory).AsNoTracking();

        if (startDate.HasValue)
        {
            query = query.Where(e => e.ExpenseDateUtc >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(e => e.ExpenseDateUtc <= endDate.Value);
        }

        var expenses = await query.ToListAsync(cancellationToken);

        var totalExpensesCount = expenses.Count;
        var totalExpenseAmount = expenses.Sum(e => e.Amount);

        var categoryBreakdown = expenses
            .GroupBy(e => new { e.ExpenseCategoryId, CategoryName = e.ExpenseCategory?.Name ?? "Uncategorized" })
            .Select(g => new ExpenseCategoryBreakdown
            {
                CategoryId = g.Key.ExpenseCategoryId,
                CategoryName = g.Key.CategoryName,
                TotalAmount = g.Sum(e => e.Amount),
                PercentageOfTotal = totalExpenseAmount > 0 ? Math.Round((g.Sum(e => e.Amount) / totalExpenseAmount) * 100.0m, 2) : 0.00m
            })
            .OrderByDescending(b => b.TotalAmount)
            .ToList();

        return new ExpenseSummaryReportResponse
        {
            TotalExpensesCount = totalExpensesCount,
            TotalExpenseAmount = totalExpenseAmount,
            CategoryBreakdown = categoryBreakdown
        };
    }

    public async Task<ProfitAndLossReportResponse> GetProfitAndLossAsync(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default)
    {
        var salesQuery = _context.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(i => i.Product)
            .Where(s => s.Status != SaleStatus.Cancelled)
            .AsNoTracking();

        var expensesQuery = _context.Expenses.AsNoTracking();

        if (startDate.HasValue)
        {
            salesQuery = salesQuery.Where(s => s.SaleDateUtc >= startDate.Value);
            expensesQuery = expensesQuery.Where(e => e.ExpenseDateUtc >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            salesQuery = salesQuery.Where(s => s.SaleDateUtc <= endDate.Value);
            expensesQuery = expensesQuery.Where(e => e.ExpenseDateUtc <= endDate.Value);
        }

        var sales = await salesQuery.ToListAsync(cancellationToken);
        var expenses = await expensesQuery.ToListAsync(cancellationToken);

        var grossSales = sales.Sum(s => s.SubTotal);
        var totalSalesDiscounts = sales.Sum(s => s.DiscountAmount);
        var netSalesRevenue = sales.Sum(s => s.TotalAmount);

        // COGS = sum of (quantity * costPrice) for all sold items
        decimal costOfGoodsSold = 0.00m;
        foreach (var sale in sales)
        {
            foreach (var item in sale.SaleItems)
            {
                costOfGoodsSold += item.Quantity * (item.Product?.CostPrice ?? 0.00m);
            }
        }

        var grossProfit = netSalesRevenue - costOfGoodsSold;
        var totalOperatingExpenses = expenses.Sum(e => e.Amount);
        var netProfit = grossProfit - totalOperatingExpenses;
        var profitMarginPercentage = netSalesRevenue > 0 ? Math.Round((netProfit / netSalesRevenue) * 100.0m, 2) : 0.00m;

        return new ProfitAndLossReportResponse
        {
            GrossSales = grossSales,
            TotalSalesDiscounts = totalSalesDiscounts,
            NetSalesRevenue = netSalesRevenue,
            CostOfGoodsSold = costOfGoodsSold,
            GrossProfit = grossProfit,
            TotalOperatingExpenses = totalOperatingExpenses,
            NetProfit = netProfit,
            ProfitMarginPercentage = profitMarginPercentage
        };
    }
}

namespace BusinessOperations.API.Constants;

public static class SystemAccounts
{
    public static readonly Guid Cash = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid AccountsReceivable = Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid Inventory = Guid.Parse("10000000-0000-0000-0000-000000000003");
    public static readonly Guid AccountsPayable = Guid.Parse("20000000-0000-0000-0000-000000000001");
    public static readonly Guid OwnerEquity = Guid.Parse("30000000-0000-0000-0000-000000000001");
    public static readonly Guid SalesRevenue = Guid.Parse("40000000-0000-0000-0000-000000000001");
    public static readonly Guid SalesReturns = Guid.Parse("40000000-0000-0000-0000-000000000002");
    public static readonly Guid CostOfGoodsSold = Guid.Parse("50000000-0000-0000-0000-000000000001");
    public static readonly Guid OperatingExpenses = Guid.Parse("50000000-0000-0000-0000-000000000002");
    public static readonly Guid PurchaseReturns = Guid.Parse("50000000-0000-0000-0000-000000000003");
}

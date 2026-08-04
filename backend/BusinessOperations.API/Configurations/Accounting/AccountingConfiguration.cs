using BusinessOperations.API.Constants;
using BusinessOperations.API.Entities.Accounting;
using BusinessOperations.API.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Accounting;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(20).IsRequired(); builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        var created = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            Seed(SystemAccounts.Cash, "1000", "Cash and Bank", AccountType.Asset, created),
            Seed(SystemAccounts.AccountsReceivable, "1100", "Accounts Receivable", AccountType.Asset, created),
            Seed(SystemAccounts.Inventory, "1200", "Inventory", AccountType.Asset, created),
            Seed(SystemAccounts.AccountsPayable, "2000", "Accounts Payable", AccountType.Liability, created),
            Seed(SystemAccounts.OwnerEquity, "3000", "Owner Equity", AccountType.Equity, created),
            Seed(SystemAccounts.SalesRevenue, "4000", "Sales Revenue", AccountType.Revenue, created),
            Seed(SystemAccounts.SalesReturns, "4100", "Sales Returns", AccountType.Revenue, created),
            Seed(SystemAccounts.CostOfGoodsSold, "5000", "Cost of Goods Sold", AccountType.Expense, created),
            Seed(SystemAccounts.OperatingExpenses, "6000", "Operating Expenses", AccountType.Expense, created),
            Seed(SystemAccounts.PurchaseReturns, "5100", "Purchase Returns", AccountType.Expense, created));
    }
    private static Account Seed(Guid id, string code, string name, AccountType type, DateTime created) =>
        new() { Id = id, Code = code, Name = name, Type = type, IsSystem = true, IsActive = true, CreatedAtUtc = created, CreatedBy = "System" };
}

public sealed class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries"); builder.HasKey(x => x.Id);
        builder.Property(x => x.EntryNumber).HasMaxLength(50).IsRequired(); builder.HasIndex(x => new { x.TenantId, x.EntryNumber }).IsUnique();
        builder.Property(x => x.Description).HasMaxLength(250).IsRequired();
        builder.Property(x => x.ReferenceType).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.ReferenceType, x.ReferenceId }).IsUnique().HasFilter("[ReferenceId] IS NOT NULL");
        builder.HasMany(x => x.Lines).WithOne(x => x.JournalEntry).HasForeignKey(x => x.JournalEntryId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class JournalLineConfiguration : IEntityTypeConfiguration<JournalLine>
{
    public void Configure(EntityTypeBuilder<JournalLine> builder)
    {
        builder.ToTable("JournalLines"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Debit).HasPrecision(18, 2); builder.Property(x => x.Credit).HasPrecision(18, 2);
        builder.Property(x => x.Memo).HasMaxLength(250);
        builder.HasOne(x => x.Account).WithMany(x => x.JournalLines).HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
    }
}

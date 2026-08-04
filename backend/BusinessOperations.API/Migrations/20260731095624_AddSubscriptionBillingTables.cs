using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessOperations.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionBillingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE [SubscriptionPlans] (
                    [Id] uniqueidentifier NOT NULL PRIMARY KEY, [Name] nvarchar(100) NOT NULL,
                    [Code] nvarchar(50) NOT NULL, [MonthlyPrice] decimal(18,2) NOT NULL,
                    [AnnualPrice] decimal(18,2) NOT NULL, [MaxUsers] int NOT NULL,
                    [MaxSalesPerMonth] int NOT NULL, [EnableAiAssistant] bit NOT NULL,
                    [EnableDoubleEntryAccounting] bit NOT NULL, [EnablePdfGeneration] bit NOT NULL,
                    [IsActive] bit NOT NULL, [Description] nvarchar(max) NOT NULL,
                    [CreatedAtUtc] datetime2 NOT NULL, [CreatedBy] nvarchar(max) NULL,
                    [UpdatedAtUtc] datetime2 NULL, [UpdatedBy] nvarchar(max) NULL,
                    [IsDeleted] bit NOT NULL, [DeletedAtUtc] datetime2 NULL, [DeletedBy] nvarchar(max) NULL);

                CREATE TABLE [TenantSubscriptions] (
                    [Id] uniqueidentifier NOT NULL PRIMARY KEY, [PlanId] uniqueidentifier NOT NULL,
                    [Status] int NOT NULL, [StartDateUtc] datetime2 NOT NULL, [EndDateUtc] datetime2 NULL,
                    [TrialEndsAtUtc] datetime2 NULL, [AutoRenew] bit NOT NULL,
                    [PaymentMethod] nvarchar(100) NOT NULL, [ExternalCustomerId] nvarchar(100) NOT NULL,
                    [CreatedAtUtc] datetime2 NOT NULL, [CreatedBy] nvarchar(max) NULL,
                    [UpdatedAtUtc] datetime2 NULL, [UpdatedBy] nvarchar(max) NULL,
                    [IsDeleted] bit NOT NULL, [DeletedAtUtc] datetime2 NULL, [DeletedBy] nvarchar(max) NULL,
                    CONSTRAINT [FK_TenantSubscriptions_SubscriptionPlans_PlanId]
                        FOREIGN KEY ([PlanId]) REFERENCES [SubscriptionPlans] ([Id]) ON DELETE CASCADE);
                CREATE INDEX [IX_TenantSubscriptions_PlanId] ON [TenantSubscriptions] ([PlanId]);

                CREATE TABLE [SubscriptionInvoices] (
                    [Id] uniqueidentifier NOT NULL PRIMARY KEY, [SubscriptionId] uniqueidentifier NOT NULL,
                    [InvoiceNumber] nvarchar(50) NOT NULL, [Amount] decimal(18,2) NOT NULL,
                    [Currency] nvarchar(10) NOT NULL, [Status] int NOT NULL,
                    [BillingDateUtc] datetime2 NOT NULL, [PaymentTransactionReference] nvarchar(100) NOT NULL,
                    [CreatedAtUtc] datetime2 NOT NULL, [CreatedBy] nvarchar(max) NULL,
                    [UpdatedAtUtc] datetime2 NULL, [UpdatedBy] nvarchar(max) NULL,
                    [IsDeleted] bit NOT NULL, [DeletedAtUtc] datetime2 NULL, [DeletedBy] nvarchar(max) NULL,
                    CONSTRAINT [FK_SubscriptionInvoices_TenantSubscriptions_SubscriptionId]
                        FOREIGN KEY ([SubscriptionId]) REFERENCES [TenantSubscriptions] ([Id]) ON DELETE CASCADE);
                CREATE INDEX [IX_SubscriptionInvoices_SubscriptionId] ON [SubscriptionInvoices] ([SubscriptionId]);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("SubscriptionInvoices");
            migrationBuilder.DropTable("TenantSubscriptions");
            migrationBuilder.DropTable("SubscriptionPlans");
        }
    }
}

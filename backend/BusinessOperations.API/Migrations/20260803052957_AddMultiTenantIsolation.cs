using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessOperations.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenantIsolation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "UserNotifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "TenantSubscriptions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "TeamChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Suppliers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "SubscriptionPlans",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "SubscriptionInvoices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Sales",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "SaleReturns",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "SaleReturnItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "SaleItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Purchases",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "PurchaseReturns",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "PurchaseReturnItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "PurchaseItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "ProductUnits",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "JournalLines",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "JournalEntries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "InventoryMovements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "InventoryAdjustments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Expenses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "ExpenseCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "BusinessProfiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AiChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            // Preserve every pre-SaaS installation as one enterprise tenant. Operators can pin
            // this ID with Tenancy__PinnedTenantId for single-tenant enterprise deployments.
            migrationBuilder.Sql("""
                DECLARE @LegacyTenant uniqueidentifier = '11111111-1111-1111-1111-111111111111';
                INSERT INTO Tenants (Id, Name, Slug, IsActive, CreatedAtUtc)
                VALUES (@LegacyTenant, 'Legacy Enterprise', 'legacy-enterprise', 1, SYSUTCDATETIME());

                UPDATE Users SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE UserNotifications SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE TenantSubscriptions SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE TeamChatMessages SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE Suppliers SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE SubscriptionInvoices SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE Sales SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE SaleReturns SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE SaleReturnItems SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE SaleItems SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE Purchases SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE PurchaseReturns SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE PurchaseReturnItems SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE PurchaseItems SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE ProductUnits SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE Products SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE Payments SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE JournalLines SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE JournalEntries SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE InventoryMovements SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE InventoryAdjustments SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE Expenses SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE ExpenseCategories SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE Employees SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE Customers SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE Categories SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE BusinessProfiles SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE AuditLogs SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                UPDATE AiChatMessages SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
                """);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "TenantId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                table: "Tenants",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TenantSubscriptions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TeamChatMessages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SubscriptionInvoices");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SaleReturns");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SaleReturnItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PurchaseReturns");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PurchaseReturnItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ProductUnits");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "JournalLines");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventoryAdjustments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AiChatMessages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Accounts");
        }
    }
}

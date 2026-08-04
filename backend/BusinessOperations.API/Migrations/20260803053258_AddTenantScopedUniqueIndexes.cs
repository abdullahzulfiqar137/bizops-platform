using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessOperations.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantScopedUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suppliers_SupplierCode",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Sales_InvoiceNumber",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturns_ReturnNumber",
                table: "SaleReturns");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_PurchaseOrderNumber",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseReturns_ReturnNumber",
                table: "PurchaseReturns");

            migrationBuilder.DropIndex(
                name: "IX_Products_Barcode",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SKU",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentNumber",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_EntryNumber",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_ReferenceType_ReferenceId",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovements_MovementNumber",
                table: "InventoryMovements");

            migrationBuilder.DropIndex(
                name: "IX_InventoryAdjustments_AdjustmentNumber",
                table: "InventoryAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseNumber",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_EmployeeCode",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CustomerCode",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_TenantId_SupplierCode",
                table: "Suppliers",
                columns: new[] { "TenantId", "SupplierCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TenantId_InvoiceNumber",
                table: "Sales",
                columns: new[] { "TenantId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturns_TenantId_ReturnNumber",
                table: "SaleReturns",
                columns: new[] { "TenantId", "ReturnNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TenantId_PurchaseOrderNumber",
                table: "Purchases",
                columns: new[] { "TenantId", "PurchaseOrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturns_TenantId_ReturnNumber",
                table: "PurchaseReturns",
                columns: new[] { "TenantId", "ReturnNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_Barcode",
                table: "Products",
                columns: new[] { "TenantId", "Barcode" },
                unique: true,
                filter: "[Barcode] IS NOT NULL AND [Barcode] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_SKU",
                table: "Products",
                columns: new[] { "TenantId", "SKU" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TenantId_PaymentNumber",
                table: "Payments",
                columns: new[] { "TenantId", "PaymentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TenantId_EntryNumber",
                table: "JournalEntries",
                columns: new[] { "TenantId", "EntryNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TenantId_ReferenceType_ReferenceId",
                table: "JournalEntries",
                columns: new[] { "TenantId", "ReferenceType", "ReferenceId" },
                unique: true,
                filter: "[ReferenceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_TenantId_MovementNumber",
                table: "InventoryMovements",
                columns: new[] { "TenantId", "MovementNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_TenantId_AdjustmentNumber",
                table: "InventoryAdjustments",
                columns: new[] { "TenantId", "AdjustmentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TenantId_ExpenseNumber",
                table: "Expenses",
                columns: new[] { "TenantId", "ExpenseNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TenantId_Email",
                table: "Employees",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TenantId_EmployeeCode",
                table: "Employees",
                columns: new[] { "TenantId", "EmployeeCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId_CustomerCode",
                table: "Customers",
                columns: new[] { "TenantId", "CustomerCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suppliers_TenantId_SupplierCode",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Sales_TenantId_InvoiceNumber",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturns_TenantId_ReturnNumber",
                table: "SaleReturns");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TenantId_PurchaseOrderNumber",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseReturns_TenantId_ReturnNumber",
                table: "PurchaseReturns");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId_Barcode",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId_SKU",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TenantId_PaymentNumber",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_TenantId_EntryNumber",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_TenantId_ReferenceType_ReferenceId",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovements_TenantId_MovementNumber",
                table: "InventoryMovements");

            migrationBuilder.DropIndex(
                name: "IX_InventoryAdjustments_TenantId_AdjustmentNumber",
                table: "InventoryAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_TenantId_ExpenseNumber",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Employees_TenantId_Email",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_TenantId_EmployeeCode",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Customers_TenantId_CustomerCode",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierCode",
                table: "Suppliers",
                column: "SupplierCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_InvoiceNumber",
                table: "Sales",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturns_ReturnNumber",
                table: "SaleReturns",
                column: "ReturnNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PurchaseOrderNumber",
                table: "Purchases",
                column: "PurchaseOrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturns_ReturnNumber",
                table: "PurchaseReturns",
                column: "ReturnNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL AND [Barcode] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentNumber",
                table: "Payments",
                column: "PaymentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_EntryNumber",
                table: "JournalEntries",
                column: "EntryNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_ReferenceType_ReferenceId",
                table: "JournalEntries",
                columns: new[] { "ReferenceType", "ReferenceId" },
                unique: true,
                filter: "[ReferenceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_MovementNumber",
                table: "InventoryMovements",
                column: "MovementNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_AdjustmentNumber",
                table: "InventoryAdjustments",
                column: "AdjustmentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseNumber",
                table: "Expenses",
                column: "ExpenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerCode",
                table: "Customers",
                column: "CustomerCode",
                unique: true);
        }
    }
}

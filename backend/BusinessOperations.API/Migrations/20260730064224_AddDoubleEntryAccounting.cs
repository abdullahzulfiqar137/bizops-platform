using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BusinessOperations.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDoubleEntryAccounting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntryDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsAutomatic = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JournalLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JournalEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalLines_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalLines_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "DeletedBy", "IsActive", "IsDeleted", "IsSystem", "Name", "Type", "UpdatedAtUtc", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "1000", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, true, "Cash and Bank", 1, null, null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "1100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, true, "Accounts Receivable", 1, null, null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "1200", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, true, "Inventory", 1, null, null },
                    { new Guid("20000000-0000-0000-0000-000000000001"), "2000", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, true, "Accounts Payable", 2, null, null },
                    { new Guid("30000000-0000-0000-0000-000000000001"), "3000", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, true, "Owner Equity", 3, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000001"), "4000", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, true, "Sales Revenue", 4, null, null },
                    { new Guid("40000000-0000-0000-0000-000000000002"), "4100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, true, "Sales Returns", 4, null, null },
                    { new Guid("50000000-0000-0000-0000-000000000001"), "5000", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, true, "Cost of Goods Sold", 5, null, null },
                    { new Guid("50000000-0000-0000-0000-000000000002"), "6000", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, true, "Operating Expenses", 5, null, null },
                    { new Guid("50000000-0000-0000-0000-000000000003"), "5100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, true, "Purchase Returns", 5, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Code",
                table: "Accounts",
                column: "Code",
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
                name: "IX_JournalLines_AccountId",
                table: "JournalLines",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalLines_JournalEntryId",
                table: "JournalLines",
                column: "JournalEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JournalLines");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "JournalEntries");
        }
    }
}

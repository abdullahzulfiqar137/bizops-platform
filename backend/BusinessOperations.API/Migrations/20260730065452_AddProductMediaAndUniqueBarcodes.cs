using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessOperations.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductMediaAndUniqueBarcodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE Products SET Barcode = NULL WHERE LTRIM(RTRIM(ISNULL(Barcode, ''))) = '';
                WITH DuplicateBarcodes AS
                (
                    SELECT Id, ROW_NUMBER() OVER (PARTITION BY Barcode ORDER BY CreatedAtUtc, Id) AS RowNumber
                    FROM Products
                    WHERE Barcode IS NOT NULL
                )
                UPDATE Products
                SET Barcode = NULL
                WHERE Id IN (SELECT Id FROM DuplicateBarcodes WHERE RowNumber > 1);
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode",
                unique: true,
                filter: "[Barcode] IS NOT NULL AND [Barcode] <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Barcode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");
        }
    }
}

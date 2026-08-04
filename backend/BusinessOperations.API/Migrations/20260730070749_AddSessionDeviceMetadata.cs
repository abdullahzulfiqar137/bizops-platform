using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessOperations.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionDeviceMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "RefreshTokens",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "RefreshTokens");
        }
    }
}

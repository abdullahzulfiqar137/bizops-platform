using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessOperations.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionIdToAiChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "AiChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "AiChatMessages");
        }
    }
}

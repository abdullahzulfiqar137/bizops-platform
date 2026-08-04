using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessOperations.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPersistentTeamChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SenderName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    SenderRole = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    AvatarColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamChatMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamChatMessages_TargetEmployeeId_TimestampUtc",
                table: "TeamChatMessages",
                columns: new[] { "TargetEmployeeId", "TimestampUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamChatMessages");
        }
    }
}

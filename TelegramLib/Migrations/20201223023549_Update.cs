using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramLib.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "buyAfterSellTransactions",
                table: "TelegramParameters",
                newName: "StopIntervalFrames");

            migrationBuilder.AddColumn<long>(
                name: "BrokerId",
                table: "TelegramParameters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "TelegramParameters",
                type: "nvarchar(500)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Market",
                table: "TelegramParameters",
                type: "nvarchar(50)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrokerId",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "Market",
                table: "TelegramParameters");

            migrationBuilder.RenameColumn(
                name: "StopIntervalFrames",
                table: "TelegramParameters",
                newName: "buyAfterSellTransactions");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramLib.Migrations
{
    public partial class StrategyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessTimeInterval",
                table: "TelegramParameters");

            migrationBuilder.AddColumn<long>(
                name: "StrategyId",
                table: "TelegramParameters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TimeFrame",
                table: "TelegramParameters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StrategyId",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "TimeFrame",
                table: "TelegramParameters");

            migrationBuilder.AddColumn<long>(
                name: "ProcessTimeInterval",
                table: "TelegramParameters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramLib.Migrations
{
    public partial class TelegramParametersMaxIntervalremove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrokerId",
                table: "TelegramUserBotRelations");

            migrationBuilder.DropColumn(
                name: "BollingerBandsStrategy",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "IsARealBot",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "MACDStrategy",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "MACrossOverStrategy",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "MaxInterval",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "MyStrategy",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "StopIntervalFrames",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "VWAPMA200InverseStrategy",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "VWAPMA200Strategy",
                table: "TelegramParameters");

            migrationBuilder.DropColumn(
                name: "VWAPMA200Strategy2",
                table: "TelegramParameters");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BrokerId",
                table: "TelegramUserBotRelations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "BollingerBandsStrategy",
                table: "TelegramParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IsARealBot",
                table: "TelegramParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MACDStrategy",
                table: "TelegramParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MACrossOverStrategy",
                table: "TelegramParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "MaxInterval",
                table: "TelegramParameters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "MyStrategy",
                table: "TelegramParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StopIntervalFrames",
                table: "TelegramParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VWAPMA200InverseStrategy",
                table: "TelegramParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VWAPMA200Strategy",
                table: "TelegramParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VWAPMA200Strategy2",
                table: "TelegramParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

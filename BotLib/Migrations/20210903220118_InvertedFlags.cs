using Microsoft.EntityFrameworkCore.Migrations;

namespace BotLib.Migrations
{
    public partial class InvertedFlags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvertBaseCurrency",
                table: "BotsParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InvertStrategy",
                table: "BotsParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InvertBaseCurrency",
                table: "BotParametersChanges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InvertStrategy",
                table: "BotParametersChanges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvertBaseCurrency",
                table: "BotsParameters");

            migrationBuilder.DropColumn(
                name: "InvertStrategy",
                table: "BotsParameters");

            migrationBuilder.DropColumn(
                name: "InvertBaseCurrency",
                table: "BotParametersChanges");

            migrationBuilder.DropColumn(
                name: "InvertStrategy",
                table: "BotParametersChanges");
        }
    }
}

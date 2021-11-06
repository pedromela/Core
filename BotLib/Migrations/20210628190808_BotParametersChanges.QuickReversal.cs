using Microsoft.EntityFrameworkCore.Migrations;

namespace BotLib.Migrations
{
    public partial class BotParametersChangesQuickReversal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuickReversal",
                table: "BotParametersChanges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuperReversal",
                table: "BotParametersChanges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuickReversal",
                table: "BotParametersChanges");

            migrationBuilder.DropColumn(
                name: "SuperReversal",
                table: "BotParametersChanges");
        }
    }
}

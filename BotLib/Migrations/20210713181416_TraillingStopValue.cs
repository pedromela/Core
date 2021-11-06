using Microsoft.EntityFrameworkCore.Migrations;

namespace BotLib.Migrations
{
    public partial class TraillingStopValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TrailingStopValue",
                table: "BotsParameters",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TrailingStopValue",
                table: "BotParametersChanges",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrailingStopValue",
                table: "BotsParameters");

            migrationBuilder.DropColumn(
                name: "TrailingStopValue",
                table: "BotParametersChanges");
        }
    }
}

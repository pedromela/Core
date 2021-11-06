using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BotLib.Migrations
{
    public partial class BotParametersQuickReversal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuickReversal",
                table: "BotsParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuperReversal",
                table: "BotsParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuickReversal",
                table: "BotsParameters");

            migrationBuilder.DropColumn(
                name: "SuperReversal",
                table: "BotsParameters");
        }
    }
}

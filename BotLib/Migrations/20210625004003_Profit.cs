using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace BotLib.Migrations
{
    public partial class Profit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profits",
                columns: table => new
                {
                    BotId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProfitValue = table.Column<double>(type: "float", nullable: false),
                    DrawBack = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false),

                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profits", x => new { x.BotId, x.Timestamp });
                    table.UniqueConstraint("AK_Profits_BotId_UserId", x => new { x.BotId, x.Timestamp });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profits");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLib.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candles",
                columns: table => new
                {
                    TimeFrame = table.Column<string>(type: "nvarchar(7)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(7)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    Max = table.Column<decimal>(type: "money", nullable: false),
                    Min = table.Column<decimal>(type: "money", nullable: false),
                    Open = table.Column<decimal>(type: "money", nullable: false),
                    Close = table.Column<decimal>(type: "money", nullable: false),
                    Volume = table.Column<double>(type: "float", nullable: false),
                    VolumeQuote = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candles", x => new { x.TimeFrame, x.Timestamp, x.Symbol });
                    table.UniqueConstraint("AK_Candles_Symbol_TimeFrame_Timestamp", x => new { x.Symbol, x.TimeFrame, x.Timestamp });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candles");
        }
    }
}

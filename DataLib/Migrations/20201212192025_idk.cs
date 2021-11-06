using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLib.Migrations
{
    public partial class idk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Candles",
                table: "Candles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Candles",
                table: "Candles",
                columns: new[] { "TimeFrame", "Symbol", "Timestamp" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Candles",
                table: "Candles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Candles",
                table: "Candles",
                columns: new[] { "TimeFrame", "Timestamp", "Symbol" });
        }
    }
}

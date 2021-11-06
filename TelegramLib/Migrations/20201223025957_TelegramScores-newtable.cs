using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramLib.Migrations
{
    public partial class TelegramScoresnewtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelegramScores",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Positions = table.Column<long>(type: "bigint", nullable: false),
                    Successes = table.Column<long>(type: "bigint", nullable: false),
                    AmountGained = table.Column<double>(type: "float", nullable: false),
                    AmountGainedDaily = table.Column<double>(type: "float", nullable: false),
                    CurrentProfit = table.Column<double>(type: "float", nullable: false),
                    ActiveTransactions = table.Column<int>(type: "int", nullable: false),
                    BotParametersId = table.Column<long>(type: "bigint", nullable: false),
                    MaxDrawBack = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramScores", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramScores");
        }
    }
}

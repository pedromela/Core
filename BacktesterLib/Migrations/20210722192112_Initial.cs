using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BacktesterLib.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BacktesterEquitys",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    Amount = table.Column<decimal>(type: "money", nullable: false),
                    RealAvailableAmountSymbol2 = table.Column<decimal>(type: "money", nullable: false),
                    RealAvailableAmountSymbol1 = table.Column<decimal>(type: "money", nullable: false),
                    EquityValue = table.Column<decimal>(type: "money", nullable: false),
                    VirtualBalance = table.Column<decimal>(type: "money", nullable: false),
                    VirtualNAV = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacktesterEquitys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "BacktesterScores",
                columns: table => new
                {
                    BotParametersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Positions = table.Column<long>(type: "bigint", nullable: false),
                    Successes = table.Column<long>(type: "bigint", nullable: false),
                    AmountGained = table.Column<double>(type: "float", nullable: false),
                    AmountGainedDaily = table.Column<double>(type: "float", nullable: false),
                    CurrentProfit = table.Column<double>(type: "float", nullable: false),
                    ActiveTransactions = table.Column<int>(type: "int", nullable: false),
                    MaxDrawBack = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacktesterScores", x => x.BotParametersId);
                });

            migrationBuilder.CreateTable(
                name: "BacktesterTransactions",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BotId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BuyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TelegramTransactionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    AmountSymbol2 = table.Column<double>(type: "float", nullable: false),
                    Type = table.Column<long>(type: "bigint", nullable: false),
                    Market = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    LastProfitablePrice = table.Column<double>(type: "float", nullable: false),
                    States = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    StopLoss = table.Column<double>(type: "float", nullable: false),
                    TakeProfit = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacktesterTransactions", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BacktesterEquitys");

            migrationBuilder.DropTable(
                name: "BacktesterScores");

            migrationBuilder.DropTable(
                name: "BacktesterTransactions");
        }
    }
}

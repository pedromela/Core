using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramLib.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelScores",
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
                    ChannelName = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelScores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramParameters",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MutatedBotId = table.Column<long>(type: "bigint", nullable: false),
                    BotName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    ProcessTimeInterval = table.Column<long>(type: "bigint", nullable: false),
                    MaxInterval = table.Column<long>(type: "bigint", nullable: false),
                    Decrease = table.Column<double>(type: "float", nullable: false),
                    Increase = table.Column<double>(type: "float", nullable: false),
                    SmartBuyTransactions = table.Column<int>(type: "int", nullable: false),
                    SmartSellTransactions = table.Column<int>(type: "int", nullable: false),
                    StopLoss = table.Column<int>(type: "int", nullable: false),
                    TakeProfit = table.Column<int>(type: "int", nullable: false),
                    TrailingStop = table.Column<int>(type: "int", nullable: false),
                    LockProfits = table.Column<int>(type: "int", nullable: false),
                    UpPercentage = table.Column<double>(type: "float", nullable: false),
                    DownPercentage = table.Column<double>(type: "float", nullable: false),
                    minSmartBuyTransactions = table.Column<int>(type: "int", nullable: false),
                    minSmartSellTransactions = table.Column<int>(type: "int", nullable: false),
                    buyAfterSellTransactions = table.Column<int>(type: "int", nullable: false),
                    MyStrategy = table.Column<int>(type: "int", nullable: false),
                    VWAPMA200Strategy = table.Column<int>(type: "int", nullable: false),
                    VWAPMA200Strategy2 = table.Column<int>(type: "int", nullable: false),
                    VWAPMA200InverseStrategy = table.Column<int>(type: "int", nullable: false),
                    MACrossOverStrategy = table.Column<int>(type: "int", nullable: false),
                    MACDStrategy = table.Column<int>(type: "int", nullable: false),
                    BollingerBandsStrategy = table.Column<int>(type: "int", nullable: false),
                    IsARealBot = table.Column<int>(type: "int", nullable: false),
                    InitLastProfitablePrice = table.Column<int>(type: "int", nullable: false),
                    StopAfterStopLossMinutes = table.Column<int>(type: "int", nullable: false),
                    StopLossMaxAtemptsBeforeStopBuying = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramParameters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramTransactions",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Market = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    EntryValue = table.Column<double>(type: "float", nullable: false),
                    TakeProfit = table.Column<double>(type: "float", nullable: false),
                    TakeProfit2 = table.Column<double>(type: "float", nullable: false),
                    TakeProfit3 = table.Column<double>(type: "float", nullable: false),
                    StopLoss = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramTransactions", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelScores");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "TelegramParameters");

            migrationBuilder.DropTable(
                name: "TelegramTransactions");
        }
    }
}

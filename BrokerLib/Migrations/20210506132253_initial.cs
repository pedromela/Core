using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrokerLib.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessPoints",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Account = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    PublicKey = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    PrivateKey = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    BearerToken = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BrokerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPoints", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ActiveMarkets",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    Market = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    BrokerId = table.Column<long>(type: "bigint", nullable: false),
                    BrokerType = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveMarkets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Candles",
                columns: table => new
                {
                    TimeFrame = table.Column<long>(type: "bigint", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(7)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: false),
                    Min = table.Column<double>(type: "float", nullable: false),
                    Open = table.Column<double>(type: "float", nullable: false),
                    Close = table.Column<double>(type: "float", nullable: false),
                    Volume = table.Column<double>(type: "float", nullable: false),
                    VolumeQuote = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candles", x => new { x.TimeFrame, x.Symbol, x.Timestamp });
                    table.UniqueConstraint("AK_Candles_Symbol_TimeFrame_Timestamp", x => new { x.Symbol, x.TimeFrame, x.Timestamp });
                });

            migrationBuilder.CreateTable(
                name: "Equitys",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Amount = table.Column<decimal>(type: "money", nullable: false),
                    RealAvailableAmountSymbol2 = table.Column<decimal>(type: "money", nullable: false),
                    RealAvailableAmountSymbol1 = table.Column<decimal>(type: "money", nullable: false),
                    EquityValue = table.Column<decimal>(type: "money", nullable: false),
                    VirtualBalance = table.Column<decimal>(type: "money", nullable: false),
                    VirtualNAV = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equitys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Points",
                columns: table => new
                {
                    TimeFrame = table.Column<long>(type: "bigint", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(7)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    Line = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => new { x.TimeFrame, x.Symbol, x.Timestamp, x.Line });
                    table.UniqueConstraint("AK_Points_Line_Symbol_TimeFrame_Timestamp", x => new { x.Line, x.Symbol, x.TimeFrame, x.Timestamp });
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BuyTradeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BrokerTransactionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AccessPointId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Market = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    Leverage = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<long>(type: "bigint", nullable: false),
                    Profit = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
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
                    table.PrimaryKey("PK_Transactions", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessPoints");

            migrationBuilder.DropTable(
                name: "ActiveMarkets");

            migrationBuilder.DropTable(
                name: "Candles");

            migrationBuilder.DropTable(
                name: "Equitys");

            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}

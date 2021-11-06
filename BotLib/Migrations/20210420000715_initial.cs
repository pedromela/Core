using Microsoft.EntityFrameworkCore.Migrations;

namespace BotLib.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotParametersChanges",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MutatedBotId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BotName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    TimeFrame = table.Column<long>(type: "bigint", nullable: false),
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
                    InitLastProfitablePrice = table.Column<int>(nullable: false),
                    StopAfterStopLossMinutes = table.Column<int>(type: "int", nullable: false),
                    StopLossMaxAtemptsBeforeStopBuying = table.Column<int>(type: "int", nullable: false),
                    Market = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    BrokerId = table.Column<long>(type: "bigint", nullable: false),
                    BrokerType = table.Column<long>(type: "bigint", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    StrategyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaxSellTransactionsByFrame = table.Column<long>(type: "bigint", nullable: false),
                    BotId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DefaultTransactionAmount = table.Column<int>(type: "int", nullable: false),
                    StartEquity = table.Column<int>(type: "int", nullable: false),
                    IsVirtual = table.Column<int>(type: "int", nullable: false),
                    RecentlyCreated = table.Column<int>(type: "int", nullable: false),
                    RecentlyModified = table.Column<int>(type: "int", nullable: false),
                    RecentlyDeleted = table.Column<int>(type: "int", nullable: false),
                    AccessPointId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotParametersChanges", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "BotParametersRankings",
                columns: table => new
                {
                    Rank = table.Column<int>(nullable: false),
                    BotId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotParametersRankings", x => x.Rank);
                });

            migrationBuilder.CreateTable(
                name: "BotsParameters",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MutatedBotId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BotName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    TimeFrame = table.Column<long>(type: "bigint", nullable: false),
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
                    InitLastProfitablePrice = table.Column<int>(nullable: false),
                    StopAfterStopLossMinutes = table.Column<int>(type: "int", nullable: false),
                    StopLossMaxAtemptsBeforeStopBuying = table.Column<int>(type: "int", nullable: false),
                    Market = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    BrokerId = table.Column<long>(type: "bigint", nullable: false),
                    BrokerType = table.Column<long>(type: "bigint", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    StrategyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaxSellTransactionsByFrame = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotsParameters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ConditionStrategiesData",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    BuyCondition = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    SellCondition = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    BuyCloseCondition = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    SellCloseCondition = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionStrategiesData", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
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
                    table.PrimaryKey("PK_Scores", x => x.BotParametersId);
                });

            migrationBuilder.CreateTable(
                name: "UserBotRelations",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BotId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessPointId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EquityId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsVirtual = table.Column<int>(type: "int", nullable: false),
                    DefaultTransactionAmount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBotRelations", x => new { x.UserId, x.BotId });
                    table.UniqueConstraint("AK_UserBotRelations_BotId_UserId", x => new { x.BotId, x.UserId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotParametersChanges");

            migrationBuilder.DropTable(
                name: "BotParametersRankings");

            migrationBuilder.DropTable(
                name: "BotsParameters");

            migrationBuilder.DropTable(
                name: "ConditionStrategiesData");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "UserBotRelations");
        }
    }
}

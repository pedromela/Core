using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramLib.Migrations
{
    public partial class TelegramUserBotRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelegramEquities",
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
                    table.PrimaryKey("PK_TelegramEquities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramUserBotRelations",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BotId = table.Column<long>(type: "bigint", nullable: false),
                    AccessPointId = table.Column<long>(type: "bigint", nullable: false),
                    EquityId = table.Column<long>(type: "bigint", nullable: false),
                    IsVirtual = table.Column<int>(type: "int", nullable: false),
                    DefaultTransactionAmount = table.Column<double>(type: "float", nullable: false),
                    BrokerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramUserBotRelations", x => new { x.UserId, x.BotId });
                    table.UniqueConstraint("AK_TelegramUserBotRelations_BotId_UserId", x => new { x.BotId, x.UserId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramEquities");

            migrationBuilder.DropTable(
                name: "TelegramUserBotRelations");
        }
    }
}

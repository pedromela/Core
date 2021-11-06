﻿// <auto-generated />
using System;
using BotLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BotLib.Migrations
{
    [DbContext(typeof(BotDBContext))]
    [Migration("20210903220118_InvertedFlags")]
    partial class InvertedFlags
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BotLib.Models.BotParameters", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BotName")
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("BrokerId")
                        .HasColumnType("bigint");

                    b.Property<long>("BrokerType")
                        .HasColumnType("bigint");

                    b.Property<string>("Channel")
                        .HasColumnType("nvarchar(500)");

                    b.Property<double>("Decrease")
                        .HasColumnType("float");

                    b.Property<double>("DownPercentage")
                        .HasColumnType("float");

                    b.Property<double>("Increase")
                        .HasColumnType("float");

                    b.Property<int>("InitLastProfitablePrice")
                        .HasColumnType("int");

                    b.Property<int>("InvertBaseCurrency")
                        .HasColumnType("int");

                    b.Property<int>("InvertStrategy")
                        .HasColumnType("int");

                    b.Property<int>("LockProfits")
                        .HasColumnType("int");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("MaxSellTransactionsByFrame")
                        .HasColumnType("bigint");

                    b.Property<string>("MutatedBotId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("QuickReversal")
                        .HasColumnType("int");

                    b.Property<int>("SmartBuyTransactions")
                        .HasColumnType("int");

                    b.Property<int>("SmartSellTransactions")
                        .HasColumnType("int");

                    b.Property<int>("StopAfterStopLossMinutes")
                        .HasColumnType("int");

                    b.Property<int>("StopLoss")
                        .HasColumnType("int");

                    b.Property<int>("StopLossMaxAtemptsBeforeStopBuying")
                        .HasColumnType("int");

                    b.Property<string>("StrategyId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("SuperReversal")
                        .HasColumnType("int");

                    b.Property<int>("TakeProfit")
                        .HasColumnType("int");

                    b.Property<long>("TimeFrame")
                        .HasColumnType("bigint");

                    b.Property<int>("TrailingStop")
                        .HasColumnType("int");

                    b.Property<double>("TrailingStopValue")
                        .HasColumnType("float");

                    b.Property<double>("UpPercentage")
                        .HasColumnType("float");

                    b.Property<int>("minSmartBuyTransactions")
                        .HasColumnType("int");

                    b.Property<int>("minSmartSellTransactions")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("BotsParameters");
                });

            modelBuilder.Entity("BotLib.Models.BotParametersChanges", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccessPointId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BotId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BotName")
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("BrokerId")
                        .HasColumnType("bigint");

                    b.Property<long>("BrokerType")
                        .HasColumnType("bigint");

                    b.Property<string>("Channel")
                        .HasColumnType("nvarchar(500)");

                    b.Property<double>("Decrease")
                        .HasColumnType("float");

                    b.Property<int>("DefaultTransactionAmount")
                        .HasColumnType("int");

                    b.Property<double>("DownPercentage")
                        .HasColumnType("float");

                    b.Property<double>("Increase")
                        .HasColumnType("float");

                    b.Property<int>("InitLastProfitablePrice")
                        .HasColumnType("int");

                    b.Property<int>("InvertBaseCurrency")
                        .HasColumnType("int");

                    b.Property<int>("InvertStrategy")
                        .HasColumnType("int");

                    b.Property<int>("IsVirtual")
                        .HasColumnType("int");

                    b.Property<int>("LockProfits")
                        .HasColumnType("int");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("MaxSellTransactionsByFrame")
                        .HasColumnType("bigint");

                    b.Property<string>("MutatedBotId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("QuickReversal")
                        .HasColumnType("int");

                    b.Property<int>("RecentlyCreated")
                        .HasColumnType("int");

                    b.Property<int>("RecentlyDeleted")
                        .HasColumnType("int");

                    b.Property<int>("RecentlyModified")
                        .HasColumnType("int");

                    b.Property<int>("SmartBuyTransactions")
                        .HasColumnType("int");

                    b.Property<int>("SmartSellTransactions")
                        .HasColumnType("int");

                    b.Property<int>("StartEquity")
                        .HasColumnType("int");

                    b.Property<int>("StopAfterStopLossMinutes")
                        .HasColumnType("int");

                    b.Property<int>("StopLoss")
                        .HasColumnType("int");

                    b.Property<int>("StopLossMaxAtemptsBeforeStopBuying")
                        .HasColumnType("int");

                    b.Property<string>("StrategyId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("SuperReversal")
                        .HasColumnType("int");

                    b.Property<int>("TakeProfit")
                        .HasColumnType("int");

                    b.Property<long>("TimeFrame")
                        .HasColumnType("bigint");

                    b.Property<int>("TrailingStop")
                        .HasColumnType("int");

                    b.Property<double>("TrailingStopValue")
                        .HasColumnType("float");

                    b.Property<double>("UpPercentage")
                        .HasColumnType("float");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("minSmartBuyTransactions")
                        .HasColumnType("int");

                    b.Property<int>("minSmartSellTransactions")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("BotParametersChanges");
                });

            modelBuilder.Entity("BotLib.Models.BotParametersRanking", b =>
                {
                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.Property<string>("BotId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Rank");

                    b.ToTable("BotParametersRankings");
                });

            modelBuilder.Entity("BotLib.Models.ConditionStrategyData", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BuyCloseCondition")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("BuyCondition")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("SellCloseCondition")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("SellCondition")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("id");

                    b.ToTable("ConditionStrategiesData");
                });

            modelBuilder.Entity("BotLib.Models.Profit", b =>
                {
                    b.Property<string>("BotId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime");

                    b.Property<double>("DrawBack")
                        .HasColumnType("float");

                    b.Property<double>("ProfitValue")
                        .HasColumnType("float");

                    b.HasKey("BotId", "Timestamp");

                    b.ToTable("Profits");
                });

            modelBuilder.Entity("BotLib.Models.Score", b =>
                {
                    b.Property<string>("BotParametersId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ActiveTransactions")
                        .HasColumnType("int");

                    b.Property<double>("AmountGained")
                        .HasColumnType("float");

                    b.Property<double>("AmountGainedDaily")
                        .HasColumnType("float");

                    b.Property<double>("CurrentProfit")
                        .HasColumnType("float");

                    b.Property<double>("MaxDrawBack")
                        .HasColumnType("float");

                    b.Property<long>("Positions")
                        .HasColumnType("bigint");

                    b.Property<long>("Successes")
                        .HasColumnType("bigint");

                    b.HasKey("BotParametersId");

                    b.ToTable("Scores");
                });

            modelBuilder.Entity("BotLib.Models.UserBotRelation", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BotId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccessPointId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("DefaultTransactionAmount")
                        .HasColumnType("float");

                    b.Property<string>("EquityId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("IsVirtual")
                        .HasColumnType("int");

                    b.HasKey("UserId", "BotId");

                    b.HasAlternateKey("BotId", "UserId");

                    b.ToTable("UserBotRelations");
                });
#pragma warning restore 612, 618
        }
    }
}

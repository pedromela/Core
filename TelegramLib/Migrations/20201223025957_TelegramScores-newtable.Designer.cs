﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TelegramLib.Models;

namespace TelegramLib.Migrations
{
    [DbContext(typeof(TelegramDBContext))]
    [Migration("20201223025957_TelegramScores-newtable")]
    partial class TelegramScoresnewtable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TelegramLib.Models.ChannelScore", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActiveTransactions")
                        .HasColumnType("int");

                    b.Property<double>("AmountGained")
                        .HasColumnType("float");

                    b.Property<double>("AmountGainedDaily")
                        .HasColumnType("float");

                    b.Property<long>("BotParametersId")
                        .HasColumnType("bigint");

                    b.Property<string>("ChannelName")
                        .HasColumnType("nvarchar(50)");

                    b.Property<double>("CurrentProfit")
                        .HasColumnType("float");

                    b.Property<long>("Positions")
                        .HasColumnType("bigint");

                    b.Property<long>("Successes")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.ToTable("ChannelScores");
                });

            modelBuilder.Entity("TelegramLib.Models.Messages", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("TelegramLib.Models.TelegramParameters", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BollingerBandsStrategy")
                        .HasColumnType("int");

                    b.Property<string>("BotName")
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("BrokerId")
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

                    b.Property<int>("IsARealBot")
                        .HasColumnType("int");

                    b.Property<int>("LockProfits")
                        .HasColumnType("int");

                    b.Property<int>("MACDStrategy")
                        .HasColumnType("int");

                    b.Property<int>("MACrossOverStrategy")
                        .HasColumnType("int");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("MaxInterval")
                        .HasColumnType("bigint");

                    b.Property<long>("MutatedBotId")
                        .HasColumnType("bigint");

                    b.Property<int>("MyStrategy")
                        .HasColumnType("int");

                    b.Property<long>("ProcessTimeInterval")
                        .HasColumnType("bigint");

                    b.Property<int>("SmartBuyTransactions")
                        .HasColumnType("int");

                    b.Property<int>("SmartSellTransactions")
                        .HasColumnType("int");

                    b.Property<int>("StopAfterStopLossMinutes")
                        .HasColumnType("int");

                    b.Property<int>("StopIntervalFrames")
                        .HasColumnType("int");

                    b.Property<int>("StopLoss")
                        .HasColumnType("int");

                    b.Property<int>("StopLossMaxAtemptsBeforeStopBuying")
                        .HasColumnType("int");

                    b.Property<int>("TakeProfit")
                        .HasColumnType("int");

                    b.Property<int>("TrailingStop")
                        .HasColumnType("int");

                    b.Property<double>("UpPercentage")
                        .HasColumnType("float");

                    b.Property<int>("VWAPMA200InverseStrategy")
                        .HasColumnType("int");

                    b.Property<int>("VWAPMA200Strategy")
                        .HasColumnType("int");

                    b.Property<int>("VWAPMA200Strategy2")
                        .HasColumnType("int");

                    b.Property<int>("minSmartBuyTransactions")
                        .HasColumnType("int");

                    b.Property<int>("minSmartSellTransactions")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("TelegramParameters");
                });

            modelBuilder.Entity("TelegramLib.Models.TelegramScore", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActiveTransactions")
                        .HasColumnType("int");

                    b.Property<double>("AmountGained")
                        .HasColumnType("float");

                    b.Property<double>("AmountGainedDaily")
                        .HasColumnType("float");

                    b.Property<long>("BotParametersId")
                        .HasColumnType("bigint");

                    b.Property<double>("CurrentProfit")
                        .HasColumnType("float");

                    b.Property<double>("MaxDrawBack")
                        .HasColumnType("float");

                    b.Property<long>("Positions")
                        .HasColumnType("bigint");

                    b.Property<long>("Successes")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.ToTable("TelegramScores");
                });

            modelBuilder.Entity("TelegramLib.Models.TelegramTransaction", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Channel")
                        .HasColumnType("nvarchar(100)");

                    b.Property<double>("EntryValue")
                        .HasColumnType("float");

                    b.Property<string>("Market")
                        .HasColumnType("nvarchar(10)");

                    b.Property<double>("StopLoss")
                        .HasColumnType("float");

                    b.Property<double>("TakeProfit")
                        .HasColumnType("float");

                    b.Property<double>("TakeProfit2")
                        .HasColumnType("float");

                    b.Property<double>("TakeProfit3")
                        .HasColumnType("float");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("id");

                    b.ToTable("TelegramTransactions");
                });
#pragma warning restore 612, 618
        }
    }
}

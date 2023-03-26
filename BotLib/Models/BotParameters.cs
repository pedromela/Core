using BrokerLib.Brokers;
using BrokerLib.Market;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Utils.Utils;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace BotLib.Models
{

    public class RankingView 
    {
        public BotParameters botParameters { get; set; }
        public Score score { get; set; }
        public int rank { get; set; }

        public RankingView(BotParameters botParameters, Score score, int rank) 
        {
            this.botParameters = botParameters;
            this.score = score;
            this.rank = rank;
        }
    }
    public class BotParameters : DBModelBase
    {
        [Key]
        [Column(TypeName = "nvarchar(450)")]
        public string id { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string MutatedBotId { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string BotName { get; set; }
        [Column(TypeName = "bigint")]
        public TimeFrames TimeFrame { get; set; }
        [Column(TypeName = "float")]
        public float TrailingStopValue { get; set; }
        [Column(TypeName = "float")]
        public float Decrease { get; set; }
        [Column(TypeName = "float")]
        public float Increase { get; set; }
        [Column(TypeName = "int")]
        public bool SmartBuyTransactions { get; set; }// to be removed
        [Column(TypeName = "int")]
        public bool SmartSellTransactions { get; set; }// to be removed
        [Column(TypeName = "int")]
        public bool StopLoss { get; set; }
        [Column(TypeName = "int")]
        public bool TakeProfit { get; set; }
        [Column(TypeName = "int")]
        public bool TrailingStop { get; set; }
        [Column(TypeName = "int")]
        public bool LockProfits { get; set; }
        [Column(TypeName = "float")]
        public float UpPercentage { get; set; }
        [Column(TypeName = "float")]
        public float DownPercentage { get; set; }
        [Column(TypeName = "int")]
        public int minSmartBuyTransactions { get; set; }// to be removed
        [Column(TypeName = "int")]
        public int minSmartSellTransactions { get; set; }// to be removed
        public int InitLastProfitablePrice { get; set; }
        [Column(TypeName = "int")]
        public int StopAfterStopLossMinutes { get; set; }// to be removed
        [Column(TypeName = "int")]
        public int StopLossMaxAtemptsBeforeStopBuying { get; set; }// to be removed
        [Column(TypeName = "nvarchar(50)")]
        public string Market { get; set; }
        [Column(TypeName = "bigint")]
        public Brokers BrokerId { get; set; }
        [Column(TypeName = "bigint")]
        public BrokerType BrokerType { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string Channel { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string StrategyId { get; set; }
        [Column(TypeName = "bigint")]
        public int MaxSellTransactionsByFrame { get; set; }
        [NotMapped]
        public BrokerDescription BrokerDescription { get; set; }
        [Column(TypeName = "int")]
        public bool QuickReversal { get; set; }
        [Column(TypeName = "int")]
        public bool SuperReversal { get; set; }
        [Column(TypeName = "int")]
        public bool InvertStrategy { get; set; }
        [Column(TypeName = "int")]
        public bool InvertBaseCurrency { get; set; }

        public BotParameters()
        : base(BotDBContext.providers)
        {

        }

        public virtual bool Valid()
        {
            try
            {
                return !string.IsNullOrEmpty(BotName) &&
                    (int)BrokerId >= 0 &&
                    (int)BrokerType >= 0 &&
                    (TakeProfit ? Increase > 0.0f : true) &&
                    (StopLoss ? Decrease > 0.0f : true) &&
                    (LockProfits ? InitLastProfitablePrice < 0 : true);
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }

            return false;
        }

        public virtual bool ValidStart(MarketInfo marketInfo)
        {
            try
            {
                if (BrokerId < 0 || BrokerType < 0 || string.IsNullOrEmpty(Market))
                {
                    return false;
                }
                return Valid() &&
                    !string.IsNullOrEmpty(StrategyId) &&
                    TimeFrame > 0 &&
                    marketInfo != null &&
                    marketInfo.GetMarket() == Market;
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }

            return false;
        }



        public virtual bool ValidStart2()
        {
            try
            {
                if (BrokerId < 0 || string.IsNullOrEmpty(Market))
                {
                    return false;
                }
                bool strategyExists = !string.IsNullOrEmpty(StrategyId);
                if (!strategyExists)
                {
                    return false;
                }
                bool scoreExists = false;

                (strategyExists, scoreExists) = BotDBContext.Execute(context => {
                    var strategyExists = context.ConditionStrategiesData.AsNoTracking().Any(m => m.id == StrategyId);
                    var scoreExists = context.Scores.AsNoTracking().Any(m => m.BotParametersId == id);
                    return (strategyExists, scoreExists);
                });

                return Valid() &&
                    strategyExists &&
                    scoreExists &&
                    TimeFrame > 0;
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }

            return false;
        }

        public BotParameters(BotParameters botParameters)
        : base(BotDBContext.providers)
        {
            ChangeParameters(botParameters);
        }

        public BotParameters(BotParametersChanges botParameterChanges)
        : base(BotDBContext.providers)
        {
            ChangeParameters(botParameterChanges);
        }

        public void ChangeParameters(BotParameters botParameters)
        {
            Increase = botParameters.Increase;
            Decrease = botParameters.Decrease;
            TrailingStopValue = botParameters.TrailingStopValue;
            DownPercentage = botParameters.DownPercentage;
            UpPercentage = botParameters.UpPercentage;
            SmartSellTransactions = botParameters.SmartSellTransactions;
            SmartBuyTransactions = botParameters.SmartBuyTransactions;
            TimeFrame = botParameters.TimeFrame;
            minSmartSellTransactions = botParameters.minSmartSellTransactions;
            minSmartBuyTransactions = botParameters.minSmartBuyTransactions;
            BotName = botParameters.BotName;
            StopLoss = botParameters.StopLoss;
            TakeProfit = botParameters.TakeProfit;
            LockProfits = botParameters.LockProfits;
            TrailingStop = botParameters.TrailingStop;
            MutatedBotId = botParameters.MutatedBotId;
            InitLastProfitablePrice = botParameters.InitLastProfitablePrice;
            StopAfterStopLossMinutes = botParameters.StopAfterStopLossMinutes;
            StopLossMaxAtemptsBeforeStopBuying = botParameters.StopLossMaxAtemptsBeforeStopBuying;
            Channel = botParameters.Channel;
            StrategyId = botParameters.StrategyId;
            Market = botParameters.Market;
            BrokerId = botParameters.BrokerId;
            BrokerType = botParameters.BrokerType;
            MaxSellTransactionsByFrame = botParameters.MaxSellTransactionsByFrame;
            SuperReversal = botParameters.SuperReversal;
            QuickReversal = botParameters.QuickReversal;
            InvertBaseCurrency = botParameters.InvertBaseCurrency;
            InvertStrategy = botParameters.InvertStrategy;
            BrokerDescription = new BrokerDescription(BrokerId, BrokerType);
        }


        public void ChangeParameters(BotParametersChanges botParameters)
        {
            //id = botParameters.BotId;
            Increase = botParameters.Increase;
            Decrease = botParameters.Decrease;
            TrailingStopValue = botParameters.TrailingStopValue;
            DownPercentage = botParameters.DownPercentage;
            UpPercentage = botParameters.UpPercentage;
            SmartSellTransactions = botParameters.SmartSellTransactions;
            SmartBuyTransactions = botParameters.SmartBuyTransactions;
            TimeFrame = botParameters.TimeFrame;
            minSmartSellTransactions = botParameters.minSmartSellTransactions;
            minSmartBuyTransactions = botParameters.minSmartBuyTransactions;
            BotName = botParameters.BotName;
            StopLoss = botParameters.StopLoss;
            TakeProfit = botParameters.TakeProfit;
            LockProfits = botParameters.LockProfits;
            TrailingStop = botParameters.TrailingStop;
            MutatedBotId = botParameters.MutatedBotId;
            InitLastProfitablePrice = botParameters.InitLastProfitablePrice;
            StopAfterStopLossMinutes = botParameters.StopAfterStopLossMinutes;
            StopLossMaxAtemptsBeforeStopBuying = botParameters.StopLossMaxAtemptsBeforeStopBuying;
            Market = botParameters.Market;
            BrokerId = botParameters.BrokerId;
            BrokerType = botParameters.BrokerType;
            StrategyId = botParameters.StrategyId;
            MaxSellTransactionsByFrame = botParameters.MaxSellTransactionsByFrame;
            SuperReversal = botParameters.SuperReversal;
            QuickReversal = botParameters.QuickReversal;
            InvertBaseCurrency = botParameters.InvertBaseCurrency;
            InvertStrategy = botParameters.InvertStrategy;
            BrokerDescription = new BrokerDescription(BrokerId, BrokerType);
        }

        public override void Store()
        {
            try
            {
                id = Guid.NewGuid().ToString();
                base.Store();
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }
        }

        ///////////////////////////////////////////////////////////////////
        //////////////////////// STATIC FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public static BotParameters GetRandomBotParameters(int count, TimeFrames timeFrame, string market, BrokerDescription brokerDescription, string strategyId)
        {
            try
            {
                BotParameters botParameters = GetRandomBotParameters(count, strategyId);
                botParameters.TimeFrame = timeFrame;
                botParameters.Market = market;
                botParameters.BrokerId = brokerDescription == null ? Brokers.None : brokerDescription.BrokerId;
                botParameters.BrokerType = brokerDescription == null ? BrokerType.exchange : brokerDescription.BrokerType;
                return botParameters;
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }
            return null;
        }

        public static BotParameters GetRandomBotParameters(int count, string strategyId) 
        {
            try
            {
                BotParameters botParameters = new BotParameters();
                botParameters.BotName = "cash" + count;
                int generatedRandomTimeFrame = RandomGenerator.RandomNumber(1, 6);
                botParameters.TimeFrame   = 0;
                botParameters.UpPercentage          = 0;
                botParameters.DownPercentage        = 0;
                botParameters.Decrease              = 0;
                botParameters.Increase              = 0;
                botParameters.TrailingStopValue     = 0;
                switch (generatedRandomTimeFrame)
                {
                    case 1:
                        botParameters.TimeFrame             = TimeFrames.M1;
                        botParameters.UpPercentage          = RandomGenerator.RandomDouble(0.0005F, 0.002F);
                        botParameters.DownPercentage        = RandomGenerator.RandomDouble(0.0005F, 0.002F);
                        botParameters.Decrease              = RandomGenerator.RandomDouble(0.002F, 0.01F);
                        botParameters.Increase              = RandomGenerator.RandomDouble(botParameters.Decrease, botParameters.Decrease * 2);
                        botParameters.TrailingStopValue     = RandomGenerator.RandomDouble(botParameters.Decrease, botParameters.Decrease * 2);
                        break;
                    case 2:
                        botParameters.TimeFrame             = TimeFrames.M5;
                        botParameters.UpPercentage          = RandomGenerator.RandomDouble(0.002f, 0.005f);
                        botParameters.DownPercentage        = RandomGenerator.RandomDouble(0.002f, 0.005f);
                        botParameters.Decrease              = RandomGenerator.RandomDouble(0.006f, 0.015f);
                        botParameters.Increase              = RandomGenerator.RandomDouble(botParameters.Decrease, botParameters.Decrease * 2);
                        botParameters.TrailingStopValue     = RandomGenerator.RandomDouble(botParameters.Decrease, botParameters.Decrease * 2);
                        break;
                    case 3:
                        botParameters.TimeFrame             = TimeFrames.M15;
                        botParameters.UpPercentage          = RandomGenerator.RandomDouble(0.003F, 0.008F);
                        botParameters.DownPercentage        = RandomGenerator.RandomDouble(0.003F, 0.008F);
                        botParameters.Decrease              = RandomGenerator.RandomDouble(0.008F, 0.02F);
                        botParameters.Increase              = RandomGenerator.RandomDouble(botParameters.Decrease, botParameters.Decrease * 2);
                        botParameters.TrailingStopValue     = RandomGenerator.RandomDouble(botParameters.Decrease, botParameters.Decrease * 2);
                        break;
                    case 4:
                        botParameters.TimeFrame             = TimeFrames.M30;
                        botParameters.UpPercentage          = RandomGenerator.RandomDouble(0.005F, 0.012F);
                        botParameters.DownPercentage        = RandomGenerator.RandomDouble(0.005F, 0.012F);
                        botParameters.Decrease              = RandomGenerator.RandomDouble(0.012F, 0.02F);
                        botParameters.Increase              = RandomGenerator.RandomDouble(botParameters.Decrease, botParameters.Decrease * 2);
                        botParameters.TrailingStopValue     = RandomGenerator.RandomDouble(botParameters.Decrease, botParameters.Decrease * 2);
                        break;
                    case 5:
                        botParameters.TimeFrame             = TimeFrames.H1;
                        botParameters.UpPercentage          = RandomGenerator.RandomDouble(0.008F, 0.018F);
                        botParameters.DownPercentage        = RandomGenerator.RandomDouble(0.008F, 0.018F);
                        botParameters.Decrease              = RandomGenerator.RandomDouble(0.018F, 0.04F);
                        botParameters.Increase              = RandomGenerator.RandomDouble(botParameters.Decrease, botParameters.Decrease * 2);
                        botParameters.TrailingStopValue     = RandomGenerator.RandomDouble(botParameters.Decrease, botParameters.Decrease * 2);
                        break;
                    default:
                        break;
                }

                botParameters.UpPercentage      += BrokerLib.BrokerLib.FEE;
                botParameters.DownPercentage    += BrokerLib.BrokerLib.FEE;
                botParameters.Decrease          += BrokerLib.BrokerLib.FEE;
                botParameters.Increase          += BrokerLib.BrokerLib.FEE;

                botParameters.SmartBuyTransactions  = RandomGenerator.RandomBoolean();
                botParameters.SmartSellTransactions = RandomGenerator.RandomBoolean();
                botParameters.StopLoss              = RandomGenerator.RandomBoolean();
                botParameters.TakeProfit            = RandomGenerator.RandomBoolean();
                botParameters.LockProfits           = RandomGenerator.RandomBoolean();
                botParameters.TrailingStop          = RandomGenerator.RandomBoolean();

                botParameters.InitLastProfitablePrice = RandomGenerator.RandomNumber(-10, -1);
                botParameters.StopAfterStopLossMinutes = RandomGenerator.RandomNumber(-60, 120);
                botParameters.StopLossMaxAtemptsBeforeStopBuying = RandomGenerator.RandomNumber(-10, 10);

                botParameters.minSmartBuyTransactions = RandomGenerator.RandomNumber(1, 10);
                botParameters.minSmartSellTransactions = RandomGenerator.RandomNumber(1, 10);

                BrokerDescription[] brokerDescriptions = Broker.GetBrokerDescriptions();
                if (brokerDescriptions == null || brokerDescriptions.Length == 0)
                {
                    BotLib.DebugMessage("BotParameters::GetRandomBotParameters() : BrokerDescriptions are not initialized!");
                    return null;
                }
                int brokerDescIdx = RandomGenerator.RandomNumber(0, brokerDescriptions.Length);
                BrokerDescription brokerDescription = brokerDescriptions[brokerDescIdx];

                botParameters.BrokerId = brokerDescription.BrokerId;
                botParameters.BrokerType = brokerDescription.BrokerType;

                Broker broker = Broker.DecideBroker(brokerDescription);
                List<string> markets = broker.GetMarkets();
                if (markets == null || markets.Count == 0)
                {
                    BotLib.DebugMessage("BotParameters::GetRandomBotParameters() : Markets are not initialized!");
                    return null;
                }
                int randomIdx = RandomGenerator.RandomNumber(0, markets.Count);
                botParameters.Market = markets[randomIdx];
                botParameters.StrategyId = strategyId;
                botParameters.MaxSellTransactionsByFrame = RandomGenerator.RandomNumber(0, 100);
                return botParameters;
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }
            return null;
        }
    }
}

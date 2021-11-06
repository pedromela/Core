using BrokerLib.Market;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace BotLib.Models
{
    public class BotParametersChanges : DBModelBase
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
        public bool SmartBuyTransactions { get; set; }
        [Column(TypeName = "int")]
        public bool SmartSellTransactions { get; set; }
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
        public int minSmartBuyTransactions { get; set; }
        [Column(TypeName = "int")]
        public int minSmartSellTransactions { get; set; }
        public int InitLastProfitablePrice { get; set; }
        [Column(TypeName = "int")]
        public int StopAfterStopLossMinutes { get; set; }
        [Column(TypeName = "int")]
        public int StopLossMaxAtemptsBeforeStopBuying { get; set; }
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
        [Column(TypeName = "nvarchar(450)")]
        public string BotId { get; set; }
        [Column(TypeName = "int")]
        public float DefaultTransactionAmount { get; set; }
        [Column(TypeName = "int")]
        public float StartEquity { get; set; }
        [Column(TypeName = "int")]
        public bool IsVirtual { get; set; }
        [Column(TypeName = "int")]
        public bool RecentlyCreated { get; set; }
        [Column(TypeName = "int")]
        public bool RecentlyModified { get; set; }
        [Column(TypeName = "int")]
        public bool RecentlyDeleted { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string AccessPointId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }
        [Column(TypeName = "int")]
        public bool QuickReversal { get; set; }
        [Column(TypeName = "int")]
        public bool SuperReversal { get; set; }
        [Column(TypeName = "int")]
        public bool InvertStrategy { get; set; }
        [Column(TypeName = "int")]
        public bool InvertBaseCurrency { get; set; }

        [NotMapped]
        public ConditionStrategyData conditionStrategyData { get; set; }

        public BotParametersChanges()
        : base(BotDBContext.providers)
        {

        }

        public BotParametersChanges(BotParameters botParameters, UserBotRelation userBotRelation)
        : base(BotDBContext.providers)
        {
            BotId = botParameters.id;
            BotName = botParameters.BotName;
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
            MaxSellTransactionsByFrame = botParameters.MaxSellTransactionsByFrame;
            BrokerId = botParameters.BrokerId;
            BrokerType = botParameters.BrokerType;
            Market = botParameters.Market;
            SuperReversal = botParameters.SuperReversal;
            QuickReversal = botParameters.QuickReversal;
            InvertBaseCurrency = botParameters.InvertBaseCurrency;
            InvertStrategy = botParameters.InvertStrategy;
            DefaultTransactionAmount = userBotRelation.DefaultTransactionAmount;
            AccessPointId = userBotRelation.AccessPointId;
            IsVirtual = userBotRelation.IsVirtual;
        }
        public string Validation()
        {
            try
            {
                string errors = "";
                if (string.IsNullOrEmpty(BotName))
                {
                    errors += "Bot name must have a value." + Environment.NewLine;
                }
                if (string.IsNullOrEmpty(Market))
                {
                    errors += "Market must have a value." + Environment.NewLine;
                }
                if (TimeFrame == 0)
                {
                    errors += "TimeFrame must have a value." + Environment.NewLine;
                }
                if (string.IsNullOrEmpty(StrategyId))
                {
                    errors += "Strategy must have a value." + Environment.NewLine;
                }
                if (!IsVirtual)
                {
                    if (string.IsNullOrEmpty(AccessPointId))
                    {
                        errors += "The virtual flag is set, so accessPointId should be greater than 0." + Environment.NewLine;
                    }
                    if ((int)BrokerId < 0)
                    {
                        errors += "The virtual flag is set and BrokerName is invalid." + Environment.NewLine;
                    }
                    if ((int)BrokerType < 0)
                    {
                        errors += "The virtual flag is set and BrokerType is invalid." + Environment.NewLine;
                    }
                }
                if (string.IsNullOrEmpty(UserId))
                {
                    errors += "userId is invalid." + Environment.NewLine;
                }
                if (TakeProfit && Increase <= 0.0f)
                {
                    errors += "takeProfit flag is set, must be greater than 0." + Environment.NewLine;
                }
                if (StopLoss && Decrease <= 0.0f)
                {
                    errors += "stopLoss flag is set, must be greater than 0." + Environment.NewLine;
                }
                if (LockProfits && InitLastProfitablePrice >= 0)
                {
                    errors += "LockProfits flag is set, initLastProfitablePrice must be lesser than 0." + Environment.NewLine;
                }
                if (!AtLeastOneFlag())
                {
                    errors += "At least one RecentlyFlag must be set." + Environment.NewLine;
                }
                if (!FlagDependencies())
                {
                    errors += "Flag dependencies not met." + Environment.NewLine;
                }
                if (conditionStrategyData != null && !conditionStrategyData.Valid())
                {
                    errors += "Strategy is invalid." + Environment.NewLine;
                }
                if (string.IsNullOrEmpty(errors))
                {
                    return "ok";
                }
                return errors;
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(e);
            }
            return null;
        }

        public virtual bool Valid()
        {
            try
            {
                return !string.IsNullOrEmpty(BotName) &&
                    IsVirtual ? true : (!string.IsNullOrEmpty(AccessPointId) && (int)BrokerId >= 0) &&
                    !string.IsNullOrEmpty(UserId) &&
                    (TakeProfit ? Increase > 0.0f : true) &&
                    (StopLoss ? Decrease > 0.0f : true) &&
                    (LockProfits ? InitLastProfitablePrice < 0 : true) &&
                    AtLeastOneFlag() &&
                    FlagDependencies();
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
                if (BrokerId < 0 || string.IsNullOrEmpty(Market))
                {
                    return false;
                }
                return Valid() &&
                    (!string.IsNullOrEmpty(StrategyId)  || (conditionStrategyData != null && conditionStrategyData.Valid())) &&
                    IsVirtual ? true : (DefaultTransactionAmount > 0) &&
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

        public bool AtLeastOneFlag() 
        {
            return RecentlyCreated || RecentlyModified || RecentlyDeleted;
        }

        public bool FlagDependencies() 
        {
            if (RecentlyModified)
            {
                return !string.IsNullOrEmpty(BotId);
            }
            else
            {
                return true;
            }
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
    }
}

using BotLib.Models;
using BrokerLib.Lib;
using BrokerLib.Market;
using BrokerLib.Models;
using SignalsEngine.Conditions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Strategys
{
    public class ConditionStrategy : IStrategy
    {
        public Dictionary<TransactionType, List<TextCondition>> _conditionsDictionary;
        public string _name;
        public MarketInfo _marketInfo;
        public string _strategyId;
        public TimeFrames _timeFrame;
        public string _botId;
        public ConditionStrategyData _conditionStrategyData;

        public ConditionStrategy(string name, MarketInfo marketInfo, TimeFrames timeFrame) 
        {
            _name = name;
            _marketInfo = marketInfo;
            _timeFrame = timeFrame;
            _strategyId = null;
            _conditionsDictionary = new Dictionary<TransactionType, List<TextCondition>>();
        }

        public ConditionStrategy(ConditionStrategyData conditionStrategyData, MarketInfo marketInfo, TimeFrames timeFrame, string botId = null)
        {
            _botId = botId;
            _name = conditionStrategyData.Name;
            _strategyId = conditionStrategyData.id;
            _conditionStrategyData = conditionStrategyData;
            _timeFrame = timeFrame;
            _marketInfo = marketInfo;// MarketInfo.Construct((Brokers)conditionStrategyData.BrokerId, conditionStrategyData.Market, true);
            _conditionsDictionary = new Dictionary<TransactionType, List<TextCondition>>();
            TextCondition textCondition = null;
            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            if (!string.IsNullOrEmpty(conditionStrategyData.BuyCondition))
            {
                textCondition = new TextCondition(_marketInfo, conditionStrategyData.BuyCondition, transactionType, _timeFrame);
                AddCondition(textCondition);
            }
            if (!string.IsNullOrEmpty(conditionStrategyData.BuyCloseCondition))
            {
                transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
                textCondition = new TextCondition(_marketInfo, conditionStrategyData.BuyCloseCondition, transactionType, _timeFrame);
                AddCondition(textCondition);
            }
            if (!string.IsNullOrEmpty(conditionStrategyData.SellCondition))
            {
                transactionType = BrokerLib.BrokerLib.TransactionType.sell;
                textCondition = new TextCondition(_marketInfo, conditionStrategyData.SellCondition, transactionType, _timeFrame);
                AddCondition(textCondition);
            }
            if (!string.IsNullOrEmpty(conditionStrategyData.SellCloseCondition))
            {
                transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
                textCondition = new TextCondition(_marketInfo, conditionStrategyData.SellCloseCondition, transactionType, _timeFrame);
                AddCondition(textCondition);
            }
        }

        public void Save(bool isPublic = false) 
        {
            try
            {
                ConditionStrategyData conditionStrategyData = new ConditionStrategyData();
                conditionStrategyData.Name = _name;
                conditionStrategyData.BuyCondition = GetConditionsString(TransactionType.buy);
                conditionStrategyData.BuyCloseCondition = GetConditionsString(TransactionType.buyclose);
                conditionStrategyData.SellCondition = GetConditionsString(TransactionType.sell);
                conditionStrategyData.SellCloseCondition = GetConditionsString(TransactionType.sellclose);
                if (isPublic)
                {
                    conditionStrategyData.UserId = "public";
                }

                conditionStrategyData.Store();
                _strategyId = conditionStrategyData.id;
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
        }

        public virtual void AddConditions() 
        {

        }

        public void AddCondition(TextCondition textCondition) 
        {
            try
            {
                if (!_conditionsDictionary.ContainsKey(textCondition._transactionType))
                {
                    _conditionsDictionary.Add(textCondition._transactionType, new List<TextCondition>());
                }
                _conditionsDictionary[textCondition._transactionType].Add(textCondition);

            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
        }

        public List<TextCondition> GetConditions(TransactionType transactionType)
        {
            try
            {
                if (!_conditionsDictionary.ContainsKey(transactionType))
                {
                    BrokerLib.BrokerLib.DebugMessage(String.Format("TextCondition::GetCondition({0}) : TransactionType text condiction not present in strategy dictionary.", transactionType));
                }
                return _conditionsDictionary[transactionType];

            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public string GetConditionsString(TransactionType transactionType)
        {
            try
            {
                List<TextCondition> textConditions = GetConditions(transactionType);
                string conditionsStr = "";
                if (textConditions == null || textConditions.Count == 0)
                {
                    return null;
                }
                else
                {
                    conditionsStr = textConditions[0]._expression;
                }
                for (int i = 1; i < textConditions.Count; i++)
                {
                    conditionsStr += " and " + textConditions[i]._expression;

                }
                return conditionsStr;
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public float CalculateFitness(TransactionType transactionType, bool invertedStrategy)
        {
            try
            {
                if (invertedStrategy)
                {
                    transactionType = BrokerLib.BrokerLib.OppositeTransactionType(transactionType);
                }
                if (!_conditionsDictionary.ContainsKey(transactionType))
                {
                    BrokerLib.BrokerLib.DebugMessage(String.Format("ConditionStrategy::CalculateFitness({0},{1}) : transaction type not present in ConditionStrategy dictionary!", _name, transactionType.ToString()));
                    return 0;
                }
                var conditions = _conditionsDictionary[transactionType];
                float max = _conditionsDictionary[transactionType].Count;
                float nextFitness = 0;
                foreach (var condition in conditions)
                {
                    if (condition.True())
                    {
                        nextFitness++;
                    }
                }
                return nextFitness/max;
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public static ConditionStrategy Construct(string strategyId, MarketInfo marketInfo, TimeFrames timeFrame, string botId) 
        {
            try
            {
                ConditionStrategyData conditionStrategyData = null;
                using (BotDBContext botContext = BotDBContext.newDBContext())
                {
                    conditionStrategyData = botContext.ConditionStrategiesData.Find(strategyId);
                }
                ConditionStrategy conditionStrategy = new ConditionStrategy(conditionStrategyData, marketInfo, timeFrame, botId);
                return conditionStrategy;
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public Dictionary<string, List<string>> GetIndicatorsNames()
        {
            try
            {
                if (_conditionStrategyData == null)
                {
                    return null;
                }
                List<string> indicatorLineNames = new List<string>();

                GetLineNames(_conditionStrategyData.BuyCondition, ref indicatorLineNames);
                GetLineNames(_conditionStrategyData.BuyCloseCondition, ref indicatorLineNames);
                GetLineNames(_conditionStrategyData.SellCondition, ref indicatorLineNames);
                GetLineNames(_conditionStrategyData.SellCloseCondition, ref indicatorLineNames);

                Dictionary<string, List<string>> indicators = new Dictionary<string, List<string>>();

                foreach (string indicatorLineName in indicatorLineNames)
                {
                    var tokens = indicatorLineName.Split("_");
                    if (tokens.Length == 3)
                    {
                        string name = String.Format("{0}_{1}", tokens[0], tokens[1]);
                        string lineName = tokens[2];
                        if (!indicators.ContainsKey(name))
                        {
                            indicators.Add(name, new List<string>());
                        }
                        indicators[name].Add(lineName);

                    }
                }

                return indicators;
            }
            catch (Exception e)
            {
                UtilsLib.UtilsLib.DebugMessage(e);
            }
            return null;
        }

        public static void GetLineNames(string condition, ref List<string> indicatorNames)
        {
            try
            {
                MatchCollection matches = Regex.Matches(condition, @"(?<!>\w)i_[A-Za-z]+(:\d*)*_[A-Za-z]+");
                foreach (Match match in matches)
                {
                    if (indicatorNames.Contains(match.Value))
                    {
                        continue;
                    }
                    indicatorNames.Add(match.Value);
                }
            }
            catch (Exception e)
            {
                UtilsLib.UtilsLib.DebugMessage(e);
            }
        }

    }
}

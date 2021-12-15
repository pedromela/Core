using BrokerLib.Models;
using SignalsEngine.Strategys;
using System;
using BrokerLib.Brokers;
using static BrokerLib.BrokerLib;
using BotLib.Models;
using BacktesterLib.Models;
using TelegramLib.Models;
using SignalsEngine.Indicators;
using BrokerLib.Market;
using System.Collections.Generic;
using System.Linq;
using UtilsLib.Utils;
using BrokerLib.Exceptions;
using BacktesterLib.Lib;

namespace BotEngine.Bot
{
    public class CurrentProfits
    {
        public int CurrentTransactions = 0;
        public float CurrentProfit = 0.0f;
        public CurrentProfits()
        {
            this.CurrentTransactions = 0;
            this.CurrentProfit = 0;
        }

        public CurrentProfits(int CurrentTransactions, float CurrentProfit)
        {
            this.CurrentTransactions = CurrentTransactions;
            this.CurrentProfit = CurrentProfit;
        }

        public void Set(int CurrentTransactions, float CurrentProfit)
        {
            this.CurrentTransactions = CurrentTransactions;
            this.CurrentProfit = CurrentProfit;
        }

        public static CurrentProfits operator +(CurrentProfits a, CurrentProfits b)
        {
            try
            {
                return new CurrentProfits(a.CurrentTransactions + b.CurrentTransactions, a.CurrentProfit + b.CurrentProfit);
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public static CurrentProfits operator -(CurrentProfits a, CurrentProfits b)
        {
            return new CurrentProfits(a.CurrentTransactions - b.CurrentTransactions, a.CurrentProfit - b.CurrentProfit);
        }

    }

    public abstract class BotBase
    {
        public readonly BacktesterDBContext _backtesterContext;
        public bool _backtest = false;

        protected Broker _broker = null;
        protected IndicatorsEngine _signalsEngine = null;
        public BotParameters _botParameters = null;
        public Score _score = null;
        public MarketTypes _marketType = MarketTypes.Crypto;
        public MarketInfo _marketInfo = null;
        public string _signalsEngineId = null;
        public ConditionStrategy _conditionStrategy = null;
        public BacktestData _backtestData = null;

        public Dictionary<TransactionType, Dictionary<string, Transaction>> _transactionsDict = null;
        public Dictionary<TransactionType, Dictionary<string, Trade>> _tradesDict = null;

        protected float FitnessLimit = 0.9f;

        public bool Keepgoin = true;
        public bool StopBuying = false;

        public float MinimumTransactionAmount = 0;
        public int auxStopAfterStopLossMinutes = 0;

        public List<Transaction> transactionErrors = new List<Transaction>();
        public List<Trade> tradeErrors = new List<Trade>();

        public float sellFitness = 0;
        public float buyFitness = 0;

        public BotBase(BotParameters botParameters, bool backtest = false)
        {
            _botParameters = botParameters;
            _signalsEngineId = IndicatorsEngine.DecideSignalsEngineId(botParameters.BrokerId, botParameters.Market == null ? "" : botParameters.Market);
            _backtest = backtest;
            BrokerDescription brokerDescription = new BrokerDescription(_botParameters.BrokerId, _botParameters.BrokerType);
            _broker = Broker.DecideBroker(brokerDescription);
            _transactionsDict = new Dictionary<TransactionType, Dictionary<string, Transaction>>(); 
            _tradesDict = new Dictionary<TransactionType, Dictionary<string, Trade>>();

            if (backtest)
            {
                _backtesterContext = BacktesterDBContext.newDBContext();
            }

            if (!(botParameters is TelegramParameters))
            {
                MarketDescription marketDescription = new MarketDescription(botParameters.Market, _broker.GetMarketType(), _broker.GetBrokerType());
                _marketInfo = IndicatorsSharedData.Instance.GetMarketInfo(marketDescription, botParameters.TimeFrame);
                if (!string.IsNullOrEmpty(botParameters.StrategyId))
                {
                    _conditionStrategy = ConditionStrategy.Construct(botParameters.StrategyId, _marketInfo, botParameters.TimeFrame, botParameters.id);
                }
                else
                {
                    _conditionStrategy = null;
                }

                if (MinimumTransactionAmount == 0)
                {
                    MinimumTransactionAmount = _marketInfo._minimumTransactionAmount;//MarketInfo.CalculateMinimumTransactionAmount(_broker, botParameters.Market);
                    if (MinimumTransactionAmount == 0)
                    {
                        BrokerLib.BrokerLib.DebugMessage(String.Format("Bot::CalculateMinimumTransactionAmount({0},{1},{2}) : MinimumTransactionAmount is 0! Please correct this.", _broker.GetBrokerId(), _broker.GetBrokerType(), botParameters.Market));
                        MinimumTransactionAmount = 1;
                    }
                    else
                    {
                        BrokerLib.BrokerLib.DebugMessage(String.Format("Bot::CalculateMinimumTransactionAmount({0},{1},{2}) : MinimumTransactionAmount is {3}.", _broker.GetBrokerId(), _broker.GetBrokerType(), botParameters.Market, MinimumTransactionAmount));
                    }
                }

                InitScore();
                _backtestData = new BacktestData();
            }
        }

        ///////////////////////////////////////////////////////////////////
        //////////////////////// ABSTRACT FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public abstract float CalculateBuyFitness();
        public abstract bool IsTransactionBuyTypes(TransactionType type);
        public abstract bool IsTransactionSellTypes(TransactionType type);
        public abstract TransactionType GetTransactionLongType();
        public abstract TransactionType GetTransactionShortType();
        public abstract TransactionType GetTransactionLongCloseType();
        public abstract TransactionType GetTransactionShortCloseType();
        public abstract void ProcessTransactions();
        public abstract int ProcessCloseTransactions(Candle lastCandle, IEnumerable<Transaction> buyTransactions, IEnumerable<Transaction> sellTransactions);


        ///////////////////////////////////////////////////////////////////
        //////////////////////// PUBLIC FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public Broker GetBroker() 
        {
            return _broker;
        }

        public void ChangeParameters(BotParameters botParameters)
        {
            _botParameters.Increase = botParameters.Increase;
            _botParameters.Decrease = botParameters.Decrease;
            _botParameters.DownPercentage = botParameters.DownPercentage;
            _botParameters.UpPercentage = botParameters.UpPercentage;
            _botParameters.SmartSellTransactions = botParameters.SmartSellTransactions;
            _botParameters.SmartBuyTransactions = botParameters.SmartBuyTransactions;
            _botParameters.TimeFrame = botParameters.TimeFrame;
            _botParameters.minSmartSellTransactions = botParameters.minSmartSellTransactions;
            _botParameters.minSmartBuyTransactions = botParameters.minSmartBuyTransactions;
            _botParameters.BotName = botParameters.BotName;
            _botParameters.StopLoss = botParameters.StopLoss;
            _botParameters.TakeProfit = botParameters.TakeProfit;
            _botParameters.LockProfits = botParameters.LockProfits;
            _botParameters.MutatedBotId = botParameters.MutatedBotId;
            _botParameters.InitLastProfitablePrice = botParameters.InitLastProfitablePrice;
            _botParameters.Market = botParameters.Market;
            _botParameters.BrokerId = botParameters.BrokerId;
            
            if (_botParameters.StrategyId != _botParameters.StrategyId && !string.IsNullOrEmpty(botParameters.StrategyId))
            {
                MarketDescription marketDescription = new MarketDescription(_botParameters.Market, _broker.GetMarketType(), _broker.GetBrokerType());
                MarketInfo marketInfo = IndicatorsSharedData.Instance.GetMarketInfo(marketDescription, _botParameters.TimeFrame);
                _conditionStrategy = ConditionStrategy.Construct(botParameters.StrategyId, marketInfo, _botParameters.TimeFrame, _botParameters.id);
            }
            _botParameters.StrategyId = botParameters.StrategyId;

            _signalsEngineId = IndicatorsEngine.DecideSignalsEngineId((Brokers)botParameters.BrokerId, botParameters.Market);
        }

        public void ChangeParameters(BotParametersChanges botParameters)
        {
            _botParameters.Increase = botParameters.Increase;
            _botParameters.Decrease = botParameters.Decrease;
            _botParameters.DownPercentage = botParameters.DownPercentage;
            _botParameters.UpPercentage = botParameters.UpPercentage;
            _botParameters.SmartSellTransactions = botParameters.SmartSellTransactions;
            _botParameters.SmartBuyTransactions = botParameters.SmartBuyTransactions;
            _botParameters.TimeFrame = botParameters.TimeFrame;
            _botParameters.minSmartSellTransactions = botParameters.minSmartSellTransactions;
            _botParameters.minSmartBuyTransactions = botParameters.minSmartBuyTransactions;
            _botParameters.BotName = botParameters.BotName;
            _botParameters.StopLoss = botParameters.StopLoss;
            _botParameters.TakeProfit = botParameters.TakeProfit;
            _botParameters.LockProfits = botParameters.LockProfits;
            _botParameters.MutatedBotId = botParameters.MutatedBotId;
            _botParameters.InitLastProfitablePrice = botParameters.InitLastProfitablePrice;
            _botParameters.Market = botParameters.Market;
            _botParameters.BrokerId = botParameters.BrokerId;

            if (_botParameters.StrategyId != _botParameters.StrategyId && !string.IsNullOrEmpty(botParameters.StrategyId))
            {
                MarketDescription marketDescription = new MarketDescription(_botParameters.Market, _broker.GetMarketType(), _broker.GetBrokerType());
                MarketInfo marketInfo = IndicatorsSharedData.Instance.GetMarketInfo(marketDescription, _botParameters.TimeFrame);
                _conditionStrategy = ConditionStrategy.Construct(botParameters.StrategyId, marketInfo, _botParameters.TimeFrame, _botParameters.id);
            }
            _botParameters.StrategyId = botParameters.StrategyId;

            _signalsEngineId = IndicatorsEngine.DecideSignalsEngineId(botParameters.BrokerId, botParameters.Market);
        }

        public void BacktestDataUpdate(DateTime dateTime) 
        {
            Dictionary<string, Candle> points = new Dictionary<string, Candle>();
            var indicatorNames = _conditionStrategy.GetIndicatorsNames();
            foreach (var indicatorNameDesc in indicatorNames)
            {
                var indicator = _signalsEngine.GetIndicator(indicatorNameDesc.Key, _botParameters.TimeFrame);
                var indicatorLinePoints = indicator.GetLastValue();
                foreach (var pair in indicatorLinePoints)
                {
                    if (points.ContainsKey(pair.Key) || !indicatorNameDesc.Value.Contains(pair.Key))
                    {
                        continue;
                    }
                    points.Add(String.Join("_", indicatorNameDesc.Key, pair.Key), pair.Value);
                }
            }
            _backtestData.Update(_score, dateTime, _signalsEngine.GetCurrentCandle(_botParameters.TimeFrame), points, null , null, BacktestingState.running);
        }

        public void BacktestDataReset() 
        {
            _backtestData.Reset();
        }

        public float Profit(Transaction transaction)
        {
            try
            {
                if (transaction.Price == 0)
                {
                    BotEngine.DebugMessage("TransactionID " + transaction.id + " has invalid Price. ");
                    return 0.0f;
                }

                Candle sell = _signalsEngine.GetCurrentCandle(_botParameters.TimeFrame);

                float fees = transaction.AmountSymbol2 * BrokerLib.BrokerLib.FEE;
                float spread = transaction.AmountSymbol2 * BrokerLib.BrokerLib.SPREAD;
                float profit = 0;


                if (transaction.Type.Equals(TransactionType.buy))
                {
                    profit = (sell.Close - transaction.Price) / sell.Close * transaction.AmountSymbol2 - fees - spread;
                }
                else if (transaction.Type.Equals(TransactionType.sell))
                {
                    profit = (transaction.Price - sell.Close) / sell.Close * transaction.AmountSymbol2 - fees - spread;
                }
                else
                {
                    return 0;
                }
                return profit;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return 0;
        }

        public float ProfitPercentage(Transaction transaction)
        {
            try
            {
                return Profit(transaction) / transaction.AmountSymbol2;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return 0;
        }


        public IEnumerable<Transaction> InitTransactionsToBeProcessed(IEnumerable<Transaction> transactions)
        {
            try
            {
                //if (_backtest)
                //{
                //    List<BacktesterTransaction> transactions = null;
                //    using (BacktesterDBContext backtesterContext = BacktesterDBContext.newDBContext())
                //    {
                //        transactions = backtesterContext.BacktesterTransactions.Where(t => t.BotId == _botParameters.id && (t.Type.Equals(GetTransactionBuyType()) || t.Type.Equals(GetTransactionSellType()))).ToList();
                //    }
                //    return transactions;
                //}

                foreach (Transaction transaction in transactions)
                {
                    if (!_transactionsDict.ContainsKey(transaction.Type))
                    {
                        _transactionsDict.Add(transaction.Type, new Dictionary<string, Transaction>());
                    }
                    _transactionsDict[transaction.Type].Add(transaction.id, transaction);
                }
                    
                return transactions;
                
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public IEnumerable<Trade> InitTradesToBeProcessed(IEnumerable<Trade> trades)
        {
            try
            {
                //if (_backtest)
                //{
                //    List<BacktesterTransaction> transactions = null;
                //    using (BacktesterDBContext backtesterContext = BacktesterDBContext.newDBContext())
                //    {
                //        transactions = backtesterContext.BacktesterTransactions.Where(t => t.BotId == _botParameters.id && (t.Type.Equals(GetTransactionBuyType()) || t.Type.Equals(GetTransactionSellType()))).ToList();
                //    }
                //    return transactions;
                //}

                foreach (Trade trade in trades)
                {
                    if (!_tradesDict.ContainsKey(trade.Type))
                    {
                        _tradesDict.Add(trade.Type, new Dictionary<string, Trade>());
                    }
                    _tradesDict[trade.Type].Add(trade.id, trade);
                }

                return trades;

            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public bool AnyTransactionsByType(TransactionType type)
        {
            try
            {
                if (_backtest)
                {
                    using (BacktesterDBContext backtesterContext = BacktesterDBContext.newDBContext())
                    {
                        return backtesterContext.BacktesterTransactions.Where(t => t.BotId == _botParameters.id && t.Type.Equals(type)).Count() > 0;
                    }
                }
                else
                {
                    if (!_transactionsDict.ContainsKey(type))
                    {
                        _transactionsDict.Add(type, new Dictionary<string, Transaction>());
                    }
                    return _transactionsDict[type].Values.Where(t => t.BotId == _botParameters.id && t.Type.Equals(type)).Count() > 0;
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return false;
        }

        public IEnumerable<Transaction> GetTransactionsByType(TransactionType type, bool init = false)
        {
            try
            {
                if (_backtest)
                {
                    List<BacktesterTransaction> transactions = null;
                    using (BacktesterDBContext backtesterContext = BacktesterDBContext.newDBContext())
                    {
                        transactions = backtesterContext.BacktesterTransactions.Where(t => t.BotId == _botParameters.id && t.Type.Equals(type)).ToList();
                    }
                    return transactions;
                }
                else
                {
                    IEnumerable<Transaction> transactions = new List<Transaction>();
                    if (init)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            try
                            {
                                using (BrokerDBContext brokerContext = BrokerDBContext.newDBContext())
                                {
                                    transactions = brokerContext.Transactions.Where(t => t.BotId == _botParameters.id && t.Type.Equals(type)).ToList();
                                }
                            }
                            catch (Exception)
                            {
                                BotEngine.DebugMessage("GetTransactionsToBeProcessed() retrying...");
                                continue;
                            }
                            if (transactions.Count() > 0)
                            {
                                break;
                            }
                        }
                        if (!_transactionsDict.ContainsKey(type))
                        {
                            _transactionsDict.Add(type, new Dictionary<string, Transaction>());
                        }
                        foreach (Transaction transaction in transactions)
                        {
                            _transactionsDict[type].Add(transaction.id, transaction);
                        }
                    }
                    else
                    {
                        if (!_transactionsDict.ContainsKey(type))
                        {
                            _transactionsDict.Add(type, new Dictionary<string, Transaction>());
                            return transactions;
                        }
                        transactions = _transactionsDict[type].Values.Where(t => t.BotId == _botParameters.id && t.Type.Equals(type));
                    }
                    return transactions;
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public CurrentProfits GetAllProfits(Candle lastCandle, IEnumerable<Transaction> transactions, ref int countSells)
        {
            try
            {
                CurrentProfits currentProfits = new CurrentProfits();
                if (transactions == null)
                {
                    return currentProfits;
                }
                int CurrentTransactions = transactions.Count();
                float currentProfit = 0.0F;
                int sellTransactionsByFrame = 0;
                foreach (var t in transactions)
                {
                    if (_botParameters.MaxSellTransactionsByFrame > 0 && sellTransactionsByFrame > _botParameters.MaxSellTransactionsByFrame)
                    {
                        break;
                    }
                    bool result = false;
                    currentProfit += ProcessTransaction(t, lastCandle, ref result);
                    if (result)
                    {
                        countSells++;
                    }
                    sellTransactionsByFrame++;
                }
                float CurrentProfit = currentProfit;

                currentProfits.Set(CurrentTransactions, CurrentProfit);
                return currentProfits;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public CurrentProfits GetAllProfits(Candle lastCandle, ref int countSells, TransactionType type)
        {
            try
            {
                IEnumerable<Transaction> transactions = GetTransactionsByType(type);

                return GetAllProfits(lastCandle, transactions, ref countSells);
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public void ProcessTransactions(Candle lastCandle, IEnumerable<Transaction> buyTransactions, IEnumerable<Transaction> sellTransactions)
        {
            try
            {
                buyFitness = CalculateBuyFitness();
                if (buyFitness > FitnessLimit)
                {
                    if (_botParameters.BotName.Equals("macdcross-ada"))
                    {
                        Console.WriteLine("macdcross-ada DEBUG");
                    }
                    ProcessTransactions(lastCandle, sellTransactions, TransactionType.buy);
                }
                else if (buyFitness < -FitnessLimit)
                {
                    if (_botParameters.BotName.Equals("macdcross-ada"))
                    {
                        Console.WriteLine("macdcross-ada DEBUG");
                    }
                    buyFitness = -buyFitness;
                    ProcessTransactions(lastCandle, buyTransactions, TransactionType.sell);
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public void ProcessTransactions(Candle lastCandle, IEnumerable<Transaction> transactions, TransactionType transactionType)
        {
            if (buyFitness < 1.0f)
            {
                buyFitness = 1.0f;
            }

            buyFitness = MathF.Round(buyFitness);
            if (!_botParameters.QuickReversal && !_botParameters.SuperReversal)
            {
                if (transactions.Any())
                {
                    Transaction t = transactions.First();
                    CloseTrades(t);
                }
                else
                {
                    StoreOrderTransaction(lastCandle, transactionType, MinimumTransactionAmount * buyFitness);
                }
            }
            else if (_botParameters.QuickReversal)
            {
                if (transactions.Any())
                {
                    Transaction t = transactions.First();
                    CloseTrades(t);
                }
                StoreOrderTransaction(lastCandle, transactionType, MinimumTransactionAmount * buyFitness);
            }
            else if (_botParameters.SuperReversal)
            {
                if (transactions.Any())
                {
                    int max = transactions.Count();
                    foreach (Transaction transaction in transactions)
                    {
                        CloseTrades(transaction);
                    }
                    for (int i = 0; i < max; i++)
                    {
                        StoreOrderTransaction(lastCandle, transactionType, MinimumTransactionAmount * buyFitness);
                    }
                }
            }
        }

        public virtual float ProcessTransaction(Transaction t, Candle lastCandle, ref bool result)
        {
            try
            {
                float currentProfit = 0.0F;
                float profit = 0;
                float fitness = CalculateSellFitness(t, ref profit);

                if (float.IsNaN(profit))
                {
                    profit = 0.0f;
                }

                if (fitness > FitnessLimit)
                {
                    result = CloseTrades(t);
                }
                else if (fitness < 0) // Stop Loss
                {
                    result = ProcessStopLoss(t, lastCandle);
                }
                else
                {
                    if (_botParameters.TrailingStop)
                    {
                        result = ProcessTrailingStop(t, lastCandle);
                    }
                    else if (_botParameters.LockProfits)
                    {
                        result = ProcessLockProfits(t, lastCandle, profit);
                    }
                }
                currentProfit += profit;
                return currentProfit;
            }
            catch (Exception e)
            {
                transactionErrors.Add(t);
                BotEngine.DebugMessage(e);
            }
            return 0;
        }

        public Transaction StoreSellOrderTransaction(Transaction Buy, Candle SellCandle, string States = "")
        {
            try
            {
                if (string.IsNullOrEmpty(States))
                {
                    States = "";
                }

                if (Buy.Price == 0)
                {
                    BotEngine.DebugMessage("StoreSellTransaction(): BuyCandle null!");
                    return null;
                }

                Candle BuyCandle = new Candle();
                BuyCandle.Close = Buy.Price;

                if (States.Contains("halftransaction"))
                {
                    Buy.Amount = Buy.Amount / 2.0f;
                    Buy.AmountSymbol2 = Buy.AmountSymbol2 / 2.0f;
                    StoreOrderTransaction(SellCandle, Buy.Type, Buy.Amount, "halftransaction");
                }

                Transaction t = DecideTransactionType();
                float spread = BrokerLib.BrokerLib.SPREAD;
                float oldAmount = 0;
                float newAmount = 0;
                t.BuyId = Buy.id;

                if (Buy.Type == TransactionType.buy)
                {
                    t.Type = TransactionType.buyclose;
                    t.Amount = Buy.Amount;// * (SellCandle.Close / BuyCandle.Close);
                    t.AmountSymbol2 = Buy.AmountSymbol2 * (SellCandle.Close / BuyCandle.Close);
                    t.AmountSymbol2 -= Buy.AmountSymbol2 * BrokerLib.BrokerLib.FEE;
                    t.AmountSymbol2 -= Buy.AmountSymbol2 * spread;
                }
                else if (Buy.Type == TransactionType.sell)
                {
                    t.Type = TransactionType.sellclose;
                    t.Amount = Buy.Amount;// * (BuyCandle.Close / SellCandle.Close);
                    t.AmountSymbol2 = Buy.AmountSymbol2 * (BuyCandle.Close / SellCandle.Close);
                    t.AmountSymbol2 -= Buy.AmountSymbol2 * BrokerLib.BrokerLib.FEE;
                    t.AmountSymbol2 -= Buy.AmountSymbol2 * spread;
                }
                else
                {
                    BotEngine.DebugMessage("StoreSellTransaction() : Undefined transaction type.");
                    return null;
                }

                oldAmount = Buy.AmountSymbol2;
                newAmount = t.AmountSymbol2;

                t.States = States;
                t.Timestamp = SellCandle.Timestamp;
                t.Market = Buy.Market;
                t.Price = SellCandle.Close;

                Buy.Type = BrokerLib.BrokerLib.DoneTransactionType(Buy.Type);

                UpdateTransaction(Buy);

                float gainedAmount = newAmount - oldAmount;
                if (newAmount > oldAmount)
                {
                    t.States += ";WIN:" + gainedAmount;
                    StoreScore(true, gainedAmount);
                }
                else
                {
                    t.States += ";LOSS:" + gainedAmount;
                    StoreScore(false, gainedAmount);
                }

                return StoreTransaction(t);
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public Transaction StoreOrderTransaction(Candle Buy, TransactionType transactionType, float quantity, string States = "")
        {
            try
            {
                if (Buy == null)
                {
                    BotEngine.DebugMessage("StoreOrderTransaction(): buy candle is null!");
                    //lastCandle = gettickerprice;//TODO
                }

                //quantity = MathF.Round(quantity, 8, MidpointRounding.ToEven);
                if (string.IsNullOrEmpty(States))
                {
                    States = "";
                }

                Transaction t = DecideTransactionType();
                //t.BuyPriceId = Buy.id;

                t.Type = transactionType;
                float stopLossFactor = 0;
                float takeProfitFactor = 0;
                if (t.Type == TransactionType.buy)
                {
                    stopLossFactor = 1 - _botParameters.Decrease;
                    takeProfitFactor = 1 + _botParameters.Increase;
                }
                else if (t.Type == TransactionType.sell)
                {
                    stopLossFactor = 1 + _botParameters.Decrease;
                    takeProfitFactor = 1 - _botParameters.Increase;
                }
                else
                {
                    BotEngine.DebugMessage("StoreOrderTransaction() : transaction type is undefined.");
                    return null;
                }

                t.Price = Buy.Close;
                t.StopLoss = Buy.Close * stopLossFactor;
                t.TakeProfit = Buy.Close * takeProfitFactor;

                t.Amount = quantity;
                t.AmountSymbol2 = t.Amount * Buy.Close;

                t.States = States;

                t.LastProfitablePrice = _botParameters.InitLastProfitablePrice;
                t.Timestamp = Buy.Timestamp;

                t = StoreTransaction(t);

                SubscribedUsersOrder(t);

                return t;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public virtual void UserOrder(Transaction transaction, UserBotRelation userBotRelation, Candle lastCandle, Candle Buy = null)
        {
            try
            {
                if (userBotRelation.IsVirtual)
                {
                    return;
                }

                AccessPoint ap = null;

                foreach (BrokerBDProvider provider in BrokerDBContext.providers)
                {
                    using (BrokerDBContext brokerContext = (BrokerDBContext)provider.GetDBContext())
                    {
                        ap = brokerContext.AccessPoints.Find(userBotRelation.AccessPointId);
                    }
                    if (ap != null)
                    {
                        break;
                    }
                }
                if (ap == null)
                {
                    BotEngine.DebugMessage("BotBase::UserOrder(): " + userBotRelation.UserId + " / " + userBotRelation.BotId + " acess point not found.");
                    return;
                }

                Equity equity = null;

                using (var context = BrokerDBContext.newDBContext())
                {
                    equity = context.Equitys.Find(userBotRelation.EquityId);
                }
                if (equity == null)
                {
                    BotEngine.DebugMessage("BotBase::UserOrder(): EquityId " + userBotRelation.EquityId + " not found. userBotRelation botId/userId: " + userBotRelation.BotId + "/" + userBotRelation.UserId);
                    equity = Equity.Initialize(_broker, ap, _botParameters.Market);
                    userBotRelation.EquityId = equity.id;
                    userBotRelation.Update();
                }

                equity = CalculateEquity(transaction.Type, equity, ap, lastCandle, userBotRelation.DefaultTransactionAmount, Buy);

                if (!CheckEquity(transaction.Type, equity, lastCandle, userBotRelation.DefaultTransactionAmount))
                {
                    return;
                }

                Trade trade = null;

                Retry.Do(() => {
                    trade = _broker.Order(transaction, ap, userBotRelation.DefaultTransactionAmount);
                }, TimeSpan.FromSeconds(1));

                if (trade == null)
                {
                    trade = new Trade(ap.id, transaction.id, null, userBotRelation.DefaultTransactionAmount, lastCandle.Close, transaction.Market, transaction.Type );
                    tradeErrors.Add(trade);
                    throw new TradeErrorException("Trade was null...");
                }
                StoreTrade(trade);
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e.StackTrace);
            }
        }

        public bool ProcessLockProfits(Transaction t, Candle lastCandle, float profit)
        {
            try
            {
                if (profit > _botParameters.Increase / 2.0 && !string.IsNullOrEmpty(t.States) && !t.States.Contains("halftransaction"))
                {
                    CloseTrades(t, t.States + ";halftransaction;takeprofit50"); // sell half of the transaction amount, 
                                                                                // the sell function creates a new transaction and change(divide by 2) the amount of the old transaction
                    return true;
                }
                else if (profit > _botParameters.Increase / 10.0)
                {
                    if (t.LastProfitablePrice >= 0)
                    {
                        bool condition = false;
                        if (t.Type == TransactionType.buy)
                        {
                            condition = lastCandle.Close > t.LastProfitablePrice;
                        }
                        else if (t.Type == TransactionType.sell)
                        {
                            condition = lastCandle.Close < t.LastProfitablePrice;
                        }
                        else
                        {
                            BotEngine.DebugMessage(String.Format("BotBase::ProcessLockProfits() : transactionType not valid."));
                            return false;
                        }

                        if (condition)
                        {
                            t.LastProfitablePrice = lastCandle.Close;
                            UpdateTransaction(t);

                        }
                        else
                        {
                            CloseTrades(t, t.States + ";lockedremainingprofit");
                            return true;
                        }
                    }
                }
                else if (profit < -_botParameters.Decrease / 2.0)
                {
                    if (t.LastProfitablePrice < 0)
                    {
                        t.LastProfitablePrice++;
                        UpdateTransaction(t);
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return false;
        }

        public bool ProcessTrailingStop(Transaction t, Candle lastCandle)
        {
            try
            {
                if (t.Type == TransactionType.buy)
                {
                    if (lastCandle.Close > t.LastProfitablePrice || t.LastProfitablePrice <= 0)
                    {
                        t.LastProfitablePrice = lastCandle.Close;
                    }
                    if (lastCandle.Close <= t.LastProfitablePrice * (1.0f - _botParameters.TrailingStopValue))
                    {
                        CloseTrades(t, t.States + ";trailingstop");
                        return true;
                    }
                }
                else if (t.Type == TransactionType.sell)
                {
                    if (lastCandle.Close < t.LastProfitablePrice || t.LastProfitablePrice <= 0)
                    {
                        t.LastProfitablePrice = lastCandle.Close;
                    }
                    if (lastCandle.Close >= t.LastProfitablePrice * (1.0f + _botParameters.TrailingStopValue))
                    {
                        CloseTrades(t, t.States + ";trailingstop");
                        return true;
                    }
                }

            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return false;
        }

        public bool ProcessStopLoss(Transaction t, Candle lastCandle)
        {
            try
            {
                if (_botParameters.StopLossMaxAtemptsBeforeStopBuying > 0)
                {
                    auxStopAfterStopLossMinutes++;
                    if (auxStopAfterStopLossMinutes >= _botParameters.StopAfterStopLossMinutes)
                    {
                        StopBuying = true;
                    }
                }
                CloseTrades(t, t.States + ";stoploss");
                return true;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return false;
        }

        public virtual bool CloseTrades(Transaction t, string description = "")
        {
            try
            {
                if (!_backtest)
                {
                    List<Trade> trades = _tradesDict[t.Type].Values.Where(m => m.TransactionId == t.id && m.Type == t.Type).ToList();

                    foreach (Trade trade in trades)
                    {
                        CloseTrade(trade, t, description);
                    }
                }
                StoreSellOrderTransaction(t, _signalsEngine.GetCurrentCandle(_botParameters.TimeFrame), description);
                return true;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return false;
        }

        public void CloseTrade(Trade trade, Transaction transaction, string description = "")
        {
            try
            {
                if (!_backtest)
                {
                    AccessPoint accessPoint = AccessPoint.Construct(trade.AccessPointId);
                    if (accessPoint == null || string.IsNullOrEmpty(accessPoint.id))
                    {
                        return;
                    }
                    Trade closetrade = null;
                    Retry.Do(() => {
                        closetrade = _broker.CloseTrade(trade, transaction, accessPoint, description);
                    }, TimeSpan.FromSeconds(1));

                    if (closetrade == null)
                    {
                        closetrade = new Trade(accessPoint.id, transaction.id, null, trade.Amount, transaction.Price, transaction.Market, transaction.Type, trade.id);
                        tradeErrors.Add(closetrade);
                        throw new TradeErrorException("Trade close was null...");
                    }
                    StoreTrade(closetrade);
                    trade.Type = BrokerLib.BrokerLib.DoneTransactionType(trade.Type);
                    UpdateTrade(trade);
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public virtual void SubscribedUsersOrder(Transaction t)
        {
            try
            {
                if (!_backtest)
                {
                    List<UserBotRelation> userBotRelations = null;
                    using (BotDBContext botContext = BotDBContext.newDBContextClient())
                    {
                        userBotRelations = botContext.UserBotRelations.Where(m => m.BotId == _botParameters.id).ToList();
                    }
                    Candle lastCandle = _signalsEngine.GetCurrentCandle(TimeFrames.M1);

                    foreach (UserBotRelation userBotRelation in userBotRelations)
                    {
                        if (userBotRelation.IsVirtual)
                        {
                            continue;
                        }
                        try
                        {
                            UserOrder(t, userBotRelation, lastCandle);
                        }
                        catch (Exception e)
                        {
                            BotEngine.DebugMessage(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public Trade StoreTrade(Trade t)
        {
            try
            {
                if (string.IsNullOrEmpty(t.Market))
                {
                    t.Market = _botParameters.Market;
                }
                //t.BotId = _botParameters.id;

                t.Store();

                if (!_tradesDict.ContainsKey(t.Type))
                {
                    _tradesDict.Add(t.Type, new Dictionary<string, Trade>());
                }
                _tradesDict[t.Type].Add(t.id, t);

                return t;
            }
            catch (Exception e)
            {
                tradeErrors.Add(t);
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public void UpdateTrade(Trade t)
        {
            try
            {
                lock (_tradesDict)
                {
                    if (!_tradesDict.ContainsKey(t.Type))
                    {
                        _tradesDict.Add(t.Type, new Dictionary<string, Trade>());
                    }
                    foreach (TransactionType key in _tradesDict.Keys)
                    {
                        if (key == t.Type)
                        {
                            continue;
                        }
                        if (_tradesDict[key].ContainsKey(t.id))
                        {
                            Trade t2 = _tradesDict[key][t.id];
                            if (t.Type != t2.Type)
                            {
                                _tradesDict[key].Remove(t.id);
                            }
                        }
                    }
                    if (_tradesDict[t.Type].ContainsKey(t.id))
                    {
                        _tradesDict[t.Type][t.id] = t;
                    }
                    else
                    {
                        _tradesDict[t.Type].Add(t.id, t);
                    }
                }
                t.Update();
            }
            catch (Exception e)
            {
                tradeErrors.Add(t);
                BotEngine.DebugMessage(e);
            }
        }

        public Transaction StoreTransaction(Transaction t)
        {
            try
            {
                if (string.IsNullOrEmpty(t.Market))
                {
                    t.Market = _botParameters.Market;
                }
                t.BotId = _botParameters.id;

                t.Store();

                if (!_transactionsDict.ContainsKey(t.Type))
                {
                    _transactionsDict.Add(t.Type, new Dictionary<string, Transaction>());
                }
                _transactionsDict[t.Type].Add(t.id, t);

                if (BotLib.BotLib.Backtest)
                {
                    if (IsTransactionBuyTypes(t.Type))
                    {
                        _backtestData.Update(null, DateTime.MinValue, null, null, t, null, BacktestingState.running);
                    }
                    else if (IsTransactionSellTypes(t.Type))
                    {
                        _backtestData.Update(null, DateTime.MinValue, null, null, null, t, BacktestingState.running);
                    }
                }

                return t;
            }
            catch (Exception e)
            {
                transactionErrors.Add(t);
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public void UpdateTransaction(Transaction t)
        {
            try
            {
                lock (_transactionsDict)
                {
                    if (!_transactionsDict.ContainsKey(t.Type))
                    {
                        _transactionsDict.Add(t.Type, new Dictionary<string, Transaction>());
                    }
                    foreach (TransactionType key in _transactionsDict.Keys)
                    {
                        if (key == t.Type)
                        {
                            continue;
                        }
                        if (_transactionsDict[key].ContainsKey(t.id))
                        {
                            Transaction t2 = _transactionsDict[key][t.id];
                            if (t.Type != t2.Type)
                            {
                                _transactionsDict[key].Remove(t.id);
                            }
                        }
                    }
                    if (_transactionsDict[t.Type].ContainsKey(t.id))
                    {
                        _transactionsDict[t.Type][t.id] = t;
                    }
                    else
                    {
                        _transactionsDict[t.Type].Add(t.id, t);
                    }
                }
                t.Update();
            }
            catch (Exception e)
            {
                transactionErrors.Add(t);
                BotEngine.DebugMessage(e);
            }
        }

        public void ProcessErrors() 
        {
            foreach (var trade in tradeErrors)
            {
                BotEngine.DebugMessage("Trade Error!");
                ProcessErrorTrade(trade);
            }
            foreach (var transaction in transactionErrors)
            {
                BotEngine.DebugMessage("Transacton Error!");
                ProcessErrorTransaction(transaction);
            }
        }

        public void RecreateSmartSellTransactions(bool _SellTransactions)
        {
            try
            {
                if (_SellTransactions)
                {
                    List<Transaction> transactions;
                    using (BrokerDBContext brokerContext = BrokerDBContext.newDBContext())
                    {
                        transactions = brokerContext.Transactions.Where(t => t.BotId == _botParameters.id && t.Type.Equals("smartsell")).ToList();
                    }
                    foreach (var t in transactions)
                    {
                        //SmartSell(t, GetCandleById(t.BuyPriceId));
                    }
                }

            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);

            }
        }
        public void UpdateSignals(IndicatorsEngine signalsEngine)
        {
            _signalsEngine = signalsEngine;
            _signalsEngineId = IndicatorsEngine.DecideSignalsEngineId(signalsEngine.GetBrokerId(),
                                                                                signalsEngine.GetMarketInfo().GetMarket());
        }

        public void SetMutatedBotId(string _MutatedBotId)
        {
            _botParameters.MutatedBotId = _MutatedBotId;
        }
        public string GetMutatedBotId(string _MutatedBotId)
        {
            return _botParameters.MutatedBotId;
        }

        public void DebugMeasures()
        {
            try
            {
                BotEngine.DebugMessage("-------------------------------------");
                BotEngine.DebugMessage("BotName:" + _botParameters.BotName);
                BotEngine.DebugMessage("TimeFrame:" + _botParameters.TimeFrame);
                BotEngine.DebugMessage("Mutated Bot id:" + _botParameters.MutatedBotId);

                try
                {
                    Score score;
                    using (BotDBContext botContext = BotDBContext.newDBContext())
                    {
                        score = botContext.Scores.Single(m => m.BotParametersId == _botParameters.id);
                    }

                    BotEngine.DebugMessage("Win rate:" + (score.Positions > 0 ? score.Successes / score.Positions : 0));
                    BotEngine.DebugMessage("Current profit:" + score.CurrentProfit);
                    BotEngine.DebugMessage("Active transactions:" + score.ActiveTransactions);
                }
                catch (Exception e)
                {
                    BotEngine.DebugMessage(e.StackTrace);
                }
                if (sellFitness > FitnessLimit)
                {
                    BotEngine.DebugMessage("State:SELLING");
                }
                if (buyFitness > FitnessLimit)
                {
                    BotEngine.DebugMessage("State:BUYING");
                }
                BotEngine.DebugMessage("-------------------------------------");
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public Transaction DecideTransactionType()
        {
            try
            {
                if (_backtest)
                {
                    return new BacktesterTransaction();
                }
                else
                {
                    return new Transaction();
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public Transaction DecideTransactionType(Transaction transaction)
        {
            try
            {
                if (_backtest)
                {
                    return new BacktesterTransaction(transaction);
                }
                else
                {
                    return new Transaction(transaction);
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public void ProcessErrorTrade(Trade t)
        {
            try
            {
                if (!_backtest)
                {
                    Transaction transaction = null;
                    Trade trade = null;
                    using (BrokerDBContext brokerContext = BrokerDBContext.newDBContextClient())
                    {
                        trade = brokerContext.Trades.Find(t.id);
                        if (trade != null)
                        {
                            tradeErrors.Remove(t);
                            return;
                        }
                        transaction = _transactionsDict[t.Type].Values.SingleOrDefault(m => m.id == t.TransactionId);
                    }
                    if (IsTransactionBuyTypes(t.Type))
                    {
                        UserBotRelation userBotRelation = null;
                        using (BotDBContext botContext = BotDBContext.newDBContextClient())
                        {
                            userBotRelation = botContext.UserBotRelations.SingleOrDefault(m => m.BotId == _botParameters.id);
                        }
                        Candle lastCandle = _signalsEngine.GetCurrentCandle(TimeFrames.M1);
                        try
                        {
                            UserOrder(transaction, userBotRelation, lastCandle);
                        }
                        catch (Exception e)
                        {
                            BotEngine.DebugMessage(e);
                        }
                    }
                    else if (IsTransactionSellTypes(t.Type))
                    {
                        trade = _tradesDict[t.Type].Values.SingleOrDefault(m => m.BuyTradeId == t.id);

                        CloseTrade(trade, transaction, transaction.States + ";ERROR");
                    }
                    tradeErrors.Remove(t);
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public void ProcessErrorTransaction(Transaction t)
        {
            try
            {
                if (!_backtest)
                {
                    Transaction transaction = null;
                    using (BrokerDBContext brokerContext = BrokerDBContext.newDBContextClient())
                    {
                        transaction = brokerContext.Transactions.Find(t.id);
                        if (transaction != null)
                        {
                            transactionErrors.Remove(t);
                            return;
                        }
                    }
                    if (IsTransactionBuyTypes(t.Type))
                    {
                        SubscribedUsersOrder(t);
                    }
                    else if (IsTransactionSellTypes(t.Type))
                    {
                        CloseTrades(t, t.States + ";ERROR");
                    }
                    transactionErrors.Remove(t);
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }


        ///////////////////////////////////////////////////////////////////
        //////////////////////// VIRTUAL FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public virtual float CalculateSellFitness(Transaction t, ref float profit)
        {
            try
            {
                float fitness = 0;
                profit = Profit(t);
                float profitPercentage = profit / t.AmountSymbol2;
                bool takeProfit = _botParameters.TakeProfit ? profit > _botParameters.Increase : true;

                if (takeProfit && _botParameters.TakeProfit)
                {
                    fitness += 1;
                    return fitness;
                }

                if (profit < -_botParameters.Decrease && _botParameters.StopLoss)
                {
                    fitness -= 1;
                    return fitness;
                }

                if (_conditionStrategy == null)
                {
                    return fitness;
                }

                fitness += _conditionStrategy.CalculateFitness(BrokerLib.BrokerLib.CloseTransactionType(t.Type), _botParameters.InvertStrategy);

                return fitness;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return 0;
        }

        public virtual float CalculateBuyFitness(TransactionType transactionType)
        {
            try
            {
                float fitness = 0;

                if (_conditionStrategy == null)
                {
                    return fitness;
                }

                fitness += _conditionStrategy.CalculateFitness(transactionType, _botParameters.InvertStrategy);

                return fitness;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return 0;
        }

        public virtual bool CheckEquity(TransactionType transactionType, Equity equity, Candle lastCandle, float amount)
        {
            try
            {
                return _broker.CheckEquity(transactionType, equity, lastCandle, amount);
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return false;

        }

        public virtual Equity CalculateEquity(TransactionType transactionType, Equity equity, AccessPoint accessPoint, Candle lastCandle, float amount, Candle Buy = null)
        {
            try
            {
                if (transactionType == GetTransactionLongType() || transactionType == GetTransactionShortType())
                {
                    return CalculateBuyEquity(equity, accessPoint, lastCandle, amount, transactionType);
                }
                else if (transactionType == GetTransactionLongCloseType() || transactionType == GetTransactionShortCloseType())
                {
                    return CalculateSellEquity(equity, accessPoint, lastCandle, amount, Buy, transactionType);
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public virtual Equity CalculateBuyEquity(Equity equity, AccessPoint accessPoint, Candle lastCandle, float amount, TransactionType transactionType)
        {
            try
            {
                float AmountInSymbol2 = amount;
                float lastClose = lastCandle.Close;
                equity.RealAvailableAmountSymbol1 = _broker.GetCurrencyBalance(accessPoint, _botParameters.Market, lastClose);
                equity.RealAvailableAmountSymbol2 = _broker.GetCurrencyBalance(accessPoint, _botParameters.Market, lastClose, true);
                float spread = BrokerLib.BrokerLib.SPREAD;
                equity.Amount = _broker.GetTotalMarketBalance(accessPoint, _botParameters.Market, lastClose);
                equity.EquityValue = 0;
                equity.VirtualBalance -= AmountInSymbol2 * (1 + (BrokerLib.BrokerLib.FEE + spread));
                equity.VirtualNAV -= AmountInSymbol2 * (BrokerLib.BrokerLib.FEE + spread);
                equity.Update();
                return equity;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public virtual Equity CalculateSellEquity(Equity equity, AccessPoint accessPoint, Candle lastCandle, float amount, Candle Buy, TransactionType transactionType)
        {
            try
            {
                float AmountInSymbol2 = amount;
                float lastClose = lastCandle.Close;
                equity.RealAvailableAmountSymbol1 = _broker.GetCurrencyBalance(accessPoint, _botParameters.Market, lastClose);
                equity.RealAvailableAmountSymbol2 = _broker.GetCurrencyBalance(accessPoint, _botParameters.Market, lastClose, true);
                float spread = BrokerLib.BrokerLib.SPREAD;
                float AmountInSymbol2AfterSell = 0;
                if (transactionType == TransactionType.buyclose)
                {
                    AmountInSymbol2AfterSell = AmountInSymbol2 * (lastCandle.Close / Buy.Close);
                }
                else if (transactionType == TransactionType.sellclose)
                {
                    AmountInSymbol2AfterSell = AmountInSymbol2 * (Buy.Close / lastCandle.Close);
                }
                equity.Amount = _broker.GetTotalMarketBalance(accessPoint, _botParameters.Market, lastClose);
                equity.EquityValue = 0;
                equity.VirtualBalance += AmountInSymbol2AfterSell * (1 - (BrokerLib.BrokerLib.FEE + spread));
                equity.VirtualNAV += AmountInSymbol2AfterSell * (1 - (BrokerLib.BrokerLib.FEE + spread)) - AmountInSymbol2;
                equity.Update();
                return equity;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public void InitScore()
        {
            try
            {
                if (_backtest)
                {
                    using (var context = BacktesterDBContext.newDBContext())
                    {
                        _score = context.BacktesterScores.SingleOrDefault(s => s.BotParametersId == _botParameters.id);
                        if (_score == null)
                        {
                            _score = new BacktesterScore();
                            _score.Positions = 0;
                            _score.BotParametersId = _botParameters.id;
                            _score.Successes = 0;
                            _score.ActiveTransactions = 0;
                            _score.CurrentProfit = 0.0f;
                            _score.MaxDrawBack = 0.0f;
                            context.BacktesterScores.Add((BacktesterScore)_score);
                            context.SaveChanges();
                        }
                    }
                }
                else
                {
                    using (var context = BotDBContext.newDBContext())
                    {
                        _score = context.Scores.SingleOrDefault(s => s.BotParametersId == _botParameters.id);
                        if (_score == null)
                        {
                            _score = new Score();
                            _score.Positions = 0;
                            _score.BotParametersId = _botParameters.id;
                            _score.Successes = 0;
                            _score.ActiveTransactions = 0;
                            _score.CurrentProfit = 0.0f;
                            _score.MaxDrawBack = 0.0f;
                            _score.Store();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public void StoreScore(bool Success, float gainedAmount, bool UpdateOnlyProfit = false, CurrentProfits currentProfits = null)
        {
            try
            {
                if (!UpdateOnlyProfit)
                {
                    if (Success)
                    {
                        _score.Successes += 1;
                    }
                    _score.Positions += 1;
                    _score.AmountGained += gainedAmount;
                    _score.AmountGainedDaily += gainedAmount;
                }
                if (currentProfits != null)
                {
                    _score.ActiveTransactions = currentProfits.CurrentTransactions;
                    _score.CurrentProfit = (float.IsNaN(currentProfits.CurrentProfit) || float.IsInfinity(currentProfits.CurrentProfit)) ? 0.0f : currentProfits.CurrentProfit;
                }

                if (_score.CurrentProfit < _score.MaxDrawBack)
                {
                    _score.MaxDrawBack = _score.CurrentProfit;
                }
                _score.Update();
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        ///////////////////////////////////////////////////////////////////
        //////////////////////// STATIC FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public static BotBase DecideBotType(BotParameters botParameters, bool backtest = false)
        {
            try
            {
                if (botParameters.BrokerId == Brokers.HitBTC)
                {
                    if (botParameters.BrokerType.Equals(BrokerType.exchange))
                    {
                        return new Bot(botParameters, backtest);
                    }
                    else if (botParameters.BrokerType.Equals(BrokerType.margin))
                    {
                        return new CFDBot(botParameters, backtest);
                    }
                    else if (botParameters.BrokerType.Equals(BrokerType.exchange_dev))
                    {
                        return new ExchangeBot(botParameters, backtest);
                    }
                    else if (botParameters.BrokerType.Equals(BrokerType.margin_dev))
                    {
                        return new MarginBot(botParameters, backtest);
                    }
                }
                else if (botParameters.BrokerId == Brokers.OANDA)
                {
                    return new MarginBot(botParameters);
                }
                return new Bot(botParameters);
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        //public static Bot GenerateMutatedBot(Bot bot, Bot oldBot)
        //{
        //    try
        //    {
        //        using (BotDBContext context = BotDBContext.newDBContext())
        //        {
        //            ChangeParameters(,);
        //            Bot newBot = new Bot(
        //                            oldBot._botParameters.id,
        //                            oldBot._botParameters.BotName,
        //                            bot._botParameters.TimeFrame,
        //                            GeneticAlgorithm.Mutate(bot._botParameters.Increase),
        //                            GeneticAlgorithm.Mutate(bot._botParameters.Decrease),
        //                            bot._botParameters.SmartBuyTransactions,
        //                            bot._botParameters.SmartSellTransactions,
        //                            GeneticAlgorithm.Mutate(bot._botParameters.UpPercentage),
        //                            GeneticAlgorithm.Mutate(bot._botParameters.DownPercentage),
        //                            GeneticAlgorithm.Mutate(bot._botParameters.minSmartBuyTransactions, 0.5f),
        //                            GeneticAlgorithm.Mutate(bot._botParameters.minSmartSellTransactions, 0.5f),
        //                            RandomGenerator.RandomDouble(0.0000f, 1.0000f) > GeneticAlgorithm.MutationRate ? bot._botParameters.StopLoss : RandomGenerator.RandomBoolean(),
        //                            RandomGenerator.RandomDouble(0.0000f, 1.0000f) > GeneticAlgorithm.MutationRate ? bot._botParameters.TakeProfit : RandomGenerator.RandomBoolean(),
        //                            RandomGenerator.RandomDouble(0.0000f, 1.0000f) > GeneticAlgorithm.MutationRate ? bot._botParameters.LockProfits : RandomGenerator.RandomBoolean(),
        //                            RandomGenerator.RandomDouble(0.0000f, 1.0000f) > GeneticAlgorithm.MutationRate ? bot._botParameters.TrailingStop : RandomGenerator.RandomBoolean(),
        //                            GeneticAlgorithm.Mutate(bot._botParameters.InitLastProfitablePrice),
        //                            GeneticAlgorithm.Mutate(bot._botParameters.StopAfterStopLossMinutes),
        //                            GeneticAlgorithm.Mutate(bot._botParameters.StopLossMaxAtemptsBeforeStopBuying),
        //                            bot._botParameters.Market,
        //                            (Brokers)bot._botParameters.BrokerId);

        //            newBot.SetMutatedBotId(bot._botParameters.id);
        //            newBot.ResetSmartTransactions();
        //            return newBot;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        BotEngine.DebugMessage(e);
        //    }
        //    return null;
        //}
        public static BotBase GenerateBotFromParameters(BotParameters botParameters, bool backtest = false)
        {
            try
            {
                BotBase bot = DecideBotType(botParameters, backtest);

                bot.RecreateSmartSellTransactions(botParameters.SmartSellTransactions);
                return bot;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public static BotBase GenerateRandomBot(int botId = -1, bool save = true, string strategyId = null)
        {
            try
            {
                BotParameters botParameters = null;

                using (BotDBContext context = BotDBContext.newDBContext())
                {
                    botParameters = BotParameters.GetRandomBotParameters(context.BotsParameters.Count(), strategyId);
                }
                if (save)
                {
                    botParameters.Store();
                }

                if (save)
                {
                    Score score = new Score();
                    score.BotParametersId = botParameters.id;
                    score.Positions = 0;
                    score.Successes = 0;
                    score.AmountGained = 0;
                    score.AmountGainedDaily = 0;
                    score.Store();
                }

                return DecideBotType(botParameters);

            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;

        }

        public static BotParameters GenerateRandomBotParameters(TimeFrames timeFrame, string market, BrokerDescription brokerDescription, string strategyId, string botId = null, bool save = true)
        {
            try
            {
                BotParameters botParameters = null;

                using (BotDBContext context = BotDBContext.newDBContext())
                {
                    botParameters = BotParameters.GetRandomBotParameters(context.BotsParameters.Count(), timeFrame, market, brokerDescription, strategyId);
                }
                if (save)
                {
                    botParameters.Store();
                }

                if (save)
                {
                    Score score = new Score();
                    score.BotParametersId = botParameters.id;
                    score.Positions = 0;
                    score.Successes = 0;
                    score.AmountGained = 0;
                    score.AmountGainedDaily = 0;
                    score.Store();
                }
                return botParameters;

            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public static BotBase GenerateRandomBot(TimeFrames timeFrame, string market, BrokerDescription brokerDescription, string strategyId, string botId = null, bool save = true)
        {
            try
            {
                BotParameters botParameters = null;

                using (BotDBContext context = BotDBContext.newDBContext())
                {
                    botParameters = BotParameters.GetRandomBotParameters(context.BotsParameters.Count(), timeFrame, market, brokerDescription, strategyId);
                }
                if (save)
                {
                    botParameters.Store();
                }

                if (save)
                {
                    Score score = new Score();
                    score.BotParametersId = botParameters.id;
                    score.Positions = 0;
                    score.Successes = 0;
                    score.AmountGained = 0;
                    score.AmountGainedDaily = 0;
                    score.Store();
                }

                return DecideBotType(botParameters);
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }
    }
}

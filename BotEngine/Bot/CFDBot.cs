using BotLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using BrokerLib.Models;
using static BrokerLib.BrokerLib;

namespace BotEngine.Bot
{
    public class CFDBot : BotBase
    {
        public CFDBot(BotParameters botParameters)
        : base(botParameters)
        {

        }

        public int ProcessSellTransactions(Candle lastCandle, IEnumerable<Transaction> sellTransactions, IEnumerable<Transaction> buyTransactions)
        {
            try
            {
                int countSells = 0;
                CurrentProfits currentProfits = GetAllProfits(lastCandle, sellTransactions, ref countSells);
                currentProfits = currentProfits + GetAllProfits(lastCandle, buyTransactions, ref countSells);

                StoreScore(true, 0.0f, true, currentProfits);

                return countSells;

            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return 0;
        }

        public void ProcessBuyTransactions(Candle lastCandle, IEnumerable<Transaction> sellTransactions, IEnumerable<Transaction> buyTransactions)
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
                    if (buyFitness < 1.0f)
                    {
                        buyFitness = 1.0f;
                    }

                    buyFitness = MathF.Round(buyFitness);
                    if (!_botParameters.QuickReversal && !_botParameters.SuperReversal) {
                        if (sellTransactions.Any())
                        {
                            Transaction t = sellTransactions.First();
                            //StoreSellOrderTransaction(t, lastCandle);
                            CloseTrades(t);
                        }
                        else
                        {
                            StoreOrderTransaction(lastCandle, TransactionType.buy, MinimumTransactionAmount * buyFitness);
                        }
                    }
                    else if (_botParameters.QuickReversal) {
                        if (sellTransactions.Any())
                        {
                            Transaction t = sellTransactions.First();
                            //StoreSellOrderTransaction(t, lastCandle);
                            CloseTrades(t);
                        }
                        StoreOrderTransaction(lastCandle, TransactionType.buy, MinimumTransactionAmount * buyFitness);
                    }
                    else if (_botParameters.SuperReversal) {
                        if (sellTransactions.Any())
                        {
                            int max = sellTransactions.Count();
                            foreach (Transaction transaction in sellTransactions)
                            {
                                //StoreSellOrderTransaction(t, lastCandle);
                                CloseTrades(transaction);
                            }
                            for (int i = 0; i < max; i++)
                            {
                                StoreOrderTransaction(lastCandle, TransactionType.buy, MinimumTransactionAmount * buyFitness);
                            }
                        }
                    }
                }
                else if (buyFitness < -FitnessLimit)
                {
                    if (_botParameters.BotName.Equals("macdcross-ada"))
                    {
                        Console.WriteLine("macdcross-ada DEBUG");
                    }
                    buyFitness = -buyFitness;
                    if (buyFitness < 1.0f)
                    {
                        buyFitness = 1.0f;
                    }

                    buyFitness = MathF.Round(buyFitness);

                    if (!_botParameters.QuickReversal && !_botParameters.SuperReversal)
                    {
                        if (buyTransactions.Any())
                        {
                            Transaction t = buyTransactions.First();
                            //StoreSellOrderTransaction(t, lastCandle);
                            CloseTrades(t);
                        }
                        else
                        {
                            StoreOrderTransaction(lastCandle, TransactionType.sell, MinimumTransactionAmount * buyFitness);

                        }
                    }
                    else if (_botParameters.QuickReversal)
                    {
                        if (buyTransactions.Any())
                        {
                            Transaction t = buyTransactions.First();
                            //StoreSellOrderTransaction(t, lastCandle);
                            CloseTrades(t);
                        }
                        StoreOrderTransaction(lastCandle, TransactionType.sell, MinimumTransactionAmount * buyFitness);
                    }
                    else if (_botParameters.SuperReversal)
                    {
                        if (buyTransactions.Any())
                        {
                            int max = sellTransactions.Count();
                            foreach (Transaction transaction in buyTransactions)
                            {
                                //StoreSellOrderTransaction(t, lastCandle);
                                CloseTrades(transaction);
                            }
                            for (int i = 0; i < max; i++)
                            {
                                StoreOrderTransaction(lastCandle, TransactionType.sell, MinimumTransactionAmount * buyFitness);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }


        public virtual Transaction StoreSellOrderTransaction(Transaction Buy, Candle SellCandle, string States = "")
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

                if (Buy.Type.Equals(TransactionType.buy))
                {
                    t.Type = TransactionType.buyclose;
                    t.Amount = Buy.Amount;// * (SellCandle.Close / BuyCandle.Close);
                    t.AmountSymbol2 = Buy.AmountSymbol2 * (SellCandle.Close / BuyCandle.Close);
                    t.AmountSymbol2 -= Buy.AmountSymbol2 * BrokerLib.BrokerLib.FEE;
                    t.AmountSymbol2 -= Buy.AmountSymbol2 * spread;
                }
                else if (Buy.Type.Equals(TransactionType.sell))
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

                if (Buy.Type == TransactionType.buy)
                {
                    Buy.Type = TransactionType.buydone;
                }
                else if (Buy.Type == TransactionType.sell)
                {
                    Buy.Type = TransactionType.selldone;
                }

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

        public virtual Transaction StoreOrderTransaction(Candle Buy, TransactionType transactionType, float quantity, string States = "")
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
                        UserOrder(t, userBotRelation, lastCandle);
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public virtual float CalculateBuyFitness()
        {
            try
            {
                float fitnessBuy = CalculateBuyFitness(TransactionType.buy);
                float fitnessSell = CalculateBuyFitness(TransactionType.sell);
                float fitnessMax = Math.Max(fitnessBuy, fitnessSell);
                if (fitnessBuy > fitnessSell)
                {
                    return fitnessBuy;
                }
                else
                {
                    return -fitnessSell;
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return 0;
        }

        ///////////////////////////////////////////////////////////////////
        //////////////////////// OVERRIDE METHODS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public override TransactionType GetTransactionBuyType()
        {
            return TransactionType.buy;
        }

        public override TransactionType GetTransactionSellType()
        {
            return TransactionType.sell;
        }

        public override TransactionType GetTransactionBuyCloseType()
        {
            return TransactionType.buyclose;
        }

        public override TransactionType GetTransactionSellCloseType()
        {
            return TransactionType.sellclose;
        }

        public override TransactionType GetTransactionBuyDoneType()
        {
            return TransactionType.buydone;
        }

        public override void ProcessTransactions()
        {
            try
            {
                if (Keepgoin)
                {
                    ProcessErrors();

                    Candle lastCandle = _signalsEngine.GetCurrentCandle(_botParameters.TimeFrame);

                    IEnumerable<Transaction> sellTransactions = GetTransactionsByType(TransactionType.sell);
                    IEnumerable<Transaction> buyTransactions = GetTransactionsByType(TransactionType.buy);

                    if (ProcessSellTransactions(lastCandle, sellTransactions, buyTransactions) == 0)
                    {
                        ProcessBuyTransactions(lastCandle, sellTransactions, buyTransactions);
                    }
                    //ProcessBuyTransactions(lastCandle, sellTransactions, buyTransactions);

                    //FIXME
                    //if (_botParameters.SmartBuyTransactions)
                    //{
                    //    Candle lastCandle = _signalsEngine.indicatorsEngine.GetCurrentCandle(_botParameters.TimeFrame);
                    //    ProcessSmartBuyTransactions(lastCandle);
                    //}
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public override float ProcessTransaction(Transaction t, Candle lastCandle, ref bool result)
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
                    CloseTrades(t);
                    result = true;
                }
                else if (fitness < 0) // Stop Loss
                {
                    ProcessStopLoss(t, lastCandle);
                    result = true;
                }
                else
                {
                    if (_botParameters.TrailingStop)
                    {
                        ProcessTrailingStop(t, lastCandle);
                        result = true;
                    }
                    else if (_botParameters.LockProfits)
                    {
                        ProcessLockProfits(t, lastCandle, profit);
                        result = true;
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

        public override void ProcessErrorTrade(Trade t)
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
                    if (t.Type == GetTransactionBuyType() || t.Type == GetTransactionSellType())
                    {
                        UserBotRelation userBotRelation = null;
                        using (BotDBContext botContext = BotDBContext.newDBContextClient())
                        {
                            userBotRelation = botContext.UserBotRelations.SingleOrDefault(m => m.BotId == _botParameters.id);
                        }
                        Candle lastCandle = _signalsEngine.GetCurrentCandle(TimeFrames.M1);
                        UserOrder(transaction, userBotRelation, lastCandle);
                    }
                    else if (t.Type == GetTransactionBuyCloseType() || t.Type == GetTransactionSellCloseType())
                    {
                        trade = _tradesDict[t.Type].Values.SingleOrDefault(m => m.TransactionId == t.id && m.Type == t.Type);

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

        public override void ProcessErrorTransaction(Transaction t)
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
                    if (t.Type == GetTransactionBuyType() || t.Type == GetTransactionSellType())
                    {
                        SubscribedUsersOrder(t);
                    }
                    else if (t.Type == GetTransactionBuyCloseType() || t.Type == GetTransactionSellCloseType())
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

        public override void UserOrder(Transaction transaction, UserBotRelation userBotRelation, Candle lastCandle, Candle Buy = null)
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
                    BotEngine.DebugMessage("CFDBot::UserOrder(): " + userBotRelation.UserId + " / " + userBotRelation.BotId + " acess point not found.");
                    return;
                }

                Equity equity = null;

                using (var context = BrokerDBContext.newDBContext())
                {
                    equity = context.Equitys.Find(userBotRelation.EquityId);
                }
                if (equity == null)
                {
                    BotEngine.DebugMessage("CFDBot::UserOrder(): EquityId " + userBotRelation.EquityId + " not found. userBotRelation botId/userId: " + userBotRelation.BotId + ":" + userBotRelation.UserId);
                    equity = Equity.Initialize(_broker, ap, _botParameters.Market);
                    userBotRelation.EquityId = equity.id;
                    userBotRelation.Update();
                }

                equity = CalculateEquity(transaction.Type, equity, ap, lastCandle, userBotRelation.DefaultTransactionAmount, Buy);

                if (!CheckEquity(transaction.Type, equity, lastCandle, userBotRelation.DefaultTransactionAmount))
                {
                    return;
                }

                Trade trade = _broker.Order(transaction, ap, userBotRelation.DefaultTransactionAmount);

                if (trade == null)
                {
                    tradeErrors.Add(trade);
                }
                else
                {
                    StoreTrade(trade);
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e.StackTrace);
            }
        }

        public override Equity CalculateEquity(TransactionType transactionType, Equity equity, AccessPoint accessPoint, Candle lastCandle, float amount, Candle Buy = null)
        {
            try
            {
                if (transactionType == TransactionType.buy || transactionType == TransactionType.sell)
                {
                    return CalculateBuyEquity(equity, accessPoint, lastCandle, amount);
                }
                else if (transactionType == TransactionType.buyclose || transactionType == TransactionType.sellclose)
                {
                    return CalculateSellEquity(equity, accessPoint, lastCandle, amount, Buy);
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public override void ProcessLockProfits(Transaction t, Candle lastCandle, float profit)
        {
            try
            {
                if (profit > _botParameters.Increase / 2.0 && !string.IsNullOrEmpty(t.States) && !t.States.Contains("halftransaction"))
                {
                    CloseTrades(t, t.States + ";halftransaction;takeprofit50"); // sell half of the transaction amount, 
                                                                                // the sell function creates a new transaction and change(divide by 2) the amount of the old transaction
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
                            BotEngine.DebugMessage(String.Format("CFDBot::ProcessLockProfits() : transactionType not valid."));
                            return;
                        }

                        if (condition)
                        {
                            t.LastProfitablePrice = lastCandle.Close;
                            UpdateTransaction(t);

                        }
                        else
                        {
                            CloseTrades(t, t.States + ";lockedremainingprofit");
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
        }

        public override void ProcessTrailingStop(Transaction t, Candle lastCandle)
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
                    }
                }

            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public override void ProcessStopLoss(Transaction t, Candle lastCandle)
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
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public override void CloseTrades(Transaction t, string description = "")
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
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public override void CloseTrade(Trade trade, Transaction transaction, string description = "")
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
                    Trade closetrade = _broker.CloseTrade(trade, transaction, accessPoint, description);
                    if (closetrade == null)
                    {
                        tradeErrors.Add(trade);
                        return;
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
    }
}

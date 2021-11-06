using System;
using System.Collections.Generic;
using System.Linq;
using BotLib.Models;
using BrokerLib.Models;
using static BrokerLib.BrokerLib;
using TelegramLib.Models;

namespace BotEngine.Bot
{
    public class Bot : BotBase
    {
        public List<SmartTransaction> smartBuyTransactions = new List<SmartTransaction>();
        public List<SmartTransaction> smartSellTransactions = new List<SmartTransaction>();

        public Bot(BotParameters botParameters, bool backtest = false)
        : base(botParameters, backtest)
        {

        }

        public override TransactionType GetTransactionBuyType() 
        {
            return _botParameters.InvertBaseCurrency ? TransactionType.sell : TransactionType.buy;
        }

        public override TransactionType GetTransactionSellType()
        {
            return _botParameters.InvertBaseCurrency ? TransactionType.buy : TransactionType.sell;
        }

        public override TransactionType GetTransactionBuyCloseType()
        {
            return _botParameters.InvertBaseCurrency ? TransactionType.sellclose : TransactionType.buyclose;
        }

        public override TransactionType GetTransactionSellCloseType()
        {
            return _botParameters.InvertBaseCurrency ? TransactionType.buyclose : TransactionType.sellclose;
        }

        public override TransactionType GetTransactionBuyDoneType()
        {
            return _botParameters.InvertBaseCurrency ? TransactionType.buydone: TransactionType.selldone;
        }
       

        public override void ProcessTransactions()
        {
            try
            {
                if (Keepgoin)
                {
                    ProcessErrors();
                    DrasticChanges();
                    if (ProcessSellTransactions() == 0)
                    {
                        if (StopBuying)
                        {
                            if (auxStopAfterStopLossMinutes >= _botParameters.StopLossMaxAtemptsBeforeStopBuying)
                            {
                                StartBuying();
                            }
                        }
                        else
                        {
                            ProcessBuyTransactions();
                        }
                    }
                    if (_botParameters.SmartBuyTransactions)
                    {
                        Candle lastCandle = _signalsEngine.GetCurrentCandle(_botParameters.TimeFrame);
                        ProcessSmartBuyTransactions(lastCandle);
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public void ProcessBuys(Candle last)
        {
            try
            {
                buyFitness = CalculateBuyFitness(GetTransactionBuyType());
                if (buyFitness > FitnessLimit)
                {
                    if (buyFitness < 1.0f)
                    {
                        buyFitness = 1.0f;
                    }

                    buyFitness = MathF.Round(buyFitness);

                    if (_botParameters.SmartBuyTransactions)
                    {
                        SmartBuy(last, MinimumTransactionAmount * buyFitness);
                    }
                    else
                    {
                        Buy(last, MinimumTransactionAmount * buyFitness);
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public int ProcessSellTransactions()
        {
            try
            {
                if (_signalsEngine == null)
                {
                    BotEngine.DebugMessage("ProcessSellTransactions(): signalEngine missing!");
                    return 0;
                }
                Candle lastCandle = _signalsEngine.GetCurrentCandle(_botParameters.TimeFrame);
                if (lastCandle != null)
                {
                    if (_botParameters.SmartSellTransactions)
                    {
                        ProcessSmartSellTransactions(lastCandle);
                    }
                    int countSells = 0;
                    CurrentProfits currentProfits = GetAllProfits(lastCandle, ref countSells, GetTransactionBuyType());
                    StoreScore(true, 0.0f, true, currentProfits);
                    return countSells;
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return 0;
        }

        public void ProcessBuyTransactions()
        {
            try
            {
                if (_signalsEngine == null)
                {
                    BotEngine.DebugMessage("ProcessSellTransactions(): signalEngine missing!");
                    return;
                }
                Candle lastCandle = _signalsEngine.GetCurrentCandle(_botParameters.TimeFrame);
                if (lastCandle != null)
                {
                    ProcessBuys(lastCandle);
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public void Stop()
        {
            Keepgoin = false;
        }

        public void DrasticChanges() 
        {
            try
            {
                LinkedListNode<Dictionary<string, Candle>> last = _signalsEngine.GetCurrentCandleNode(_botParameters.TimeFrame);
                if (last == null) return;
                LinkedListNode<Dictionary<string, Candle>> lastMinus1 = last.Previous;
                float difference = lastMinus1.Value["middle"].Close - last.Value["middle"].Close;
                float factor = MathF.Sqrt((int)_botParameters.TimeFrame);
                if (difference > last.Value["middle"].Close * 0.015f*factor) 
                {
                    //if (SmartBuyTransactions)
                    //{
                    //    SmartBuy(last.Value);
                    //}
                    //else
                    //{
                        Buy(last.Value["middle"], 0.0005F * factor);
                    //}
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);

            }
        }

        public bool ProcessSmartTransaction(SmartTransaction smartTransaction, Candle lastCandle)
        {
            try
            {
                if (smartTransaction.Type == GetTransactionBuyType() || smartTransaction.Type == GetTransactionSellCloseType())
                {
                    if (lastCandle.Close > smartTransaction.PriceAtCreation * (1 + _botParameters.UpPercentage) || smartTransaction.Count > _botParameters.StopAfterStopLossMinutes)
                    {

                        smartTransaction.BuyTransaction = Buy(lastCandle, MinimumTransactionAmount);
                        if (smartTransaction.BuyTransaction == null) return false;
                        return true;
                    }
                    else
                    {
                        smartTransaction.Count++;
                        smartTransaction.PriceAtCreation = lastCandle.Close;
                    }
                }
                else if (smartTransaction.Type == GetTransactionSellType() || smartTransaction.Type == GetTransactionBuyCloseType())
                {
                    if (lastCandle.Close < smartTransaction.PriceAtCreation * (1 - _botParameters.DownPercentage) || smartTransaction.Count > _botParameters.StopAfterStopLossMinutes)
                    {
                        Sell(smartTransaction.BuyTransaction, lastCandle, "smartsell");
                        return true;
                    }
                    else
                    {
                        smartTransaction.Count++;
                        smartTransaction.PriceAtCreation = lastCandle.Close;
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return false;
        }

        public void ProcessSmartBuyTransactions(Candle lastCandle)
        {
            try
            {
                List<SmartTransaction> toRemove = new List<SmartTransaction>();
                foreach (SmartTransaction t in smartBuyTransactions)
                {
                    if (ProcessSmartTransaction(t, lastCandle))
                    {
                        toRemove.Add(t);
                    }
                }
                foreach (var t in toRemove)
                {
                    smartBuyTransactions.Remove(t);
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public void ProcessSmartSellTransactions(Candle lastCandle)
        {
            try
            {
                List<SmartTransaction> toRemove = new List<SmartTransaction>();
                foreach (SmartTransaction t in smartSellTransactions)
                {
                    if (ProcessSmartTransaction(t, lastCandle))
                    {
                        toRemove.Add(t);
                    }
                }
                foreach (var t in toRemove)
                {
                    smartSellTransactions.Remove(t);
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public void SmartBuy(Candle last, float quantity)
        {
            smartBuyTransactions.Add(new SmartTransaction(last.Close, null, GetTransactionBuyType(), quantity));
        }

        public void BuyQueue(Candle last, float quantity)
        {
            try
            {
                //buyTransactionsQuantity += quantity;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public Transaction Buy(Candle last, float quantity)
        {
            try
            {
                Transaction t = StoreBuyTransaction(last, quantity);
                return t;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public void SellQueue(Candle last, float quantity)
        {
            try
            {
                //sellTransactionsQuantity += quantity;
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public void SmartSell(Transaction Buy, Candle LastCandle)
        {
            Buy.Type = TransactionType.smartsell;
            smartSellTransactions.Add(new SmartTransaction(LastCandle.Close, Buy, GetTransactionBuyCloseType()));
            UpdateTransaction(Buy);
        }

        public Transaction Sell(Transaction Buy, Candle LastCandle, string Description = "")
        {
            try
            {
                return StoreSellTransaction(Buy, LastCandle, Description);
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public void SubscribedUsersBuy(Transaction transaction) 
        {
            try
            {
                if (!_backtest)
                {
                    List<UserBotRelation> userBotRelations = null;
                    using (var context = BotDBContext.newDBContextClient())
                    {
                        userBotRelations = context.UserBotRelations.Where(m => m.BotId == _botParameters.id).ToList();
                    }

                    Candle lastCandle = _signalsEngine.GetCurrentCandle(TimeFrames.M1);

                    foreach (UserBotRelation userBotRelation in userBotRelations)
                    {
                        UserOrder(transaction, userBotRelation, lastCandle);
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e.StackTrace);
            }
        }

        public void SubscribedUsersSell(Transaction transaction, Candle Buy)
        {
            try
            {
                if (!_backtest)
                {
                    List<UserBotRelation> userBotRelations = null;
                    using (var context = BotDBContext.newDBContextClient())
                    {
                        userBotRelations = context.UserBotRelations.Where(m => m.BotId == _botParameters.id).ToList();
                    }

                    transaction.Type = BrokerLib.BrokerLib.CloseTransactionType(transaction.Type);

                    Candle lastCandle = _signalsEngine.GetCurrentCandle(TimeFrames.M1);
                    foreach (UserBotRelation userBotRelation in userBotRelations)
                    {
                        UserOrder(transaction, userBotRelation, lastCandle, Buy);
                    }
                }
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e.StackTrace);
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

                using (BrokerDBContext brokerContext = BrokerDBContext.newDBContextClient())
                {
                    ap = brokerContext.AccessPoints.Find(userBotRelation.AccessPointId);
                }
                if (ap == null)
                {
                    BotEngine.DebugMessage("Bot::UserOrder(): " + userBotRelation.UserId + " / " + userBotRelation.BotId + " acess point not found.");
                    return;
                }
                Equity equity = null;
                using (var context = BrokerDBContext.newDBContext())
                {
                    equity = context.Equitys.Find(userBotRelation.EquityId);
                }
                if (equity == null)
                {
                    BotEngine.DebugMessage("Bot::UserOrder(): Equity " + userBotRelation.EquityId + " not found. userBotRelation botId/userId: " + userBotRelation.BotId + ":" + userBotRelation.UserId);
                    equity = Equity.Initialize(_broker, ap, _botParameters.Market);
                    userBotRelation.EquityId = equity.id;
                    userBotRelation.Update();
                }
                equity = CalculateEquity(transaction.Type, equity, ap, lastCandle, userBotRelation.DefaultTransactionAmount, Buy);

                if (!CheckEquity(transaction.Type, equity, lastCandle, userBotRelation.DefaultTransactionAmount))
                {
                    return;
                }

                if (transaction.Type == TransactionType.buy || transaction.Type == TransactionType.sellclose)
                {
                    _broker.Buy(userBotRelation.DefaultTransactionAmount, _botParameters.Market, ap);
                }
                else if (transaction.Type == TransactionType.sell || transaction.Type == TransactionType.buyclose)
                {
                    _broker.Sell(userBotRelation.DefaultTransactionAmount, _botParameters.Market, ap);
                }
                
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e.StackTrace);
            }
        }

        public Transaction StoreBuyTransaction(Candle Buy, float quantity, string States = "")
        {
            try
            {
                quantity = MathF.Round(quantity, 4, MidpointRounding.ToEven);
                if (string.IsNullOrEmpty(States))
                {
                    States = "";
                }

                Transaction t = DecideTransactionType();
                //t.BuyPriceId = Buy.id;
                t.Price = Buy.Close;
                t.StopLoss = Buy.Close * (1 - _botParameters.Decrease);
                t.TakeProfit = Buy.Close * (1 + _botParameters.Increase);

                if (quantity < MinimumTransactionAmount)
                {
                    quantity = MinimumTransactionAmount;
                }

                t.Amount = quantity;
                t.AmountSymbol2 = t.Amount * Buy.Close;

                t.States = States;

                t.LastProfitablePrice = _botParameters.InitLastProfitablePrice;
                t.Type = GetTransactionBuyType();
                t.Timestamp = Buy.Timestamp;

                SubscribedUsersBuy(t);

                return StoreTransaction(t);
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public virtual Transaction StoreSellTransaction(Transaction Buy, Candle SellCandle, string States = "")
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
                    StoreBuyTransaction(BuyCandle, Buy.Amount, "halftransaction");
                }

                Transaction t = DecideTransactionType();
                float spread = BrokerLib.BrokerLib.SPREAD;
                t.BuyId = Buy.id;
                //t.SellPriceId = SellCandle.id;
                t.Amount = Buy.Amount;// * (SellCandle.Close / BuyCandle.Close);
                //t.Amount -= Buy.Amount * BrokerLib.BrokerLib.FEE;
                //t.Amount -= Buy.Amount * spread;
                float oldAmountEUR = Buy.AmountSymbol2;
                t.AmountSymbol2 = Buy.AmountSymbol2 * (SellCandle.Close / BuyCandle.Close);
                t.AmountSymbol2 -= Buy.AmountSymbol2 * BrokerLib.BrokerLib.FEE;
                t.AmountSymbol2 -= Buy.AmountSymbol2 * spread;
                t.States = States;
                t.Type = GetTransactionBuyCloseType();
                t.Timestamp = SellCandle.Timestamp;
                t.Price = SellCandle.Close;

                float newAmountEUR = t.AmountSymbol2;

                float gainedAmount = newAmountEUR - oldAmountEUR;
                if (newAmountEUR > oldAmountEUR)
                {
                    t.States += ";WIN:" + gainedAmount;
                    StoreScore(true, gainedAmount);
                }
                else
                {
                    t.States += ";LOSS:" + gainedAmount;
                    StoreScore(false, gainedAmount);
                }

                SubscribedUsersSell(Buy, BuyCandle);

                Buy.Type = GetTransactionBuyDoneType();

                UpdateTransaction(Buy);

                return StoreTransaction(t);
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public void StartBuying() 
        {
            StopBuying = false;
            auxStopAfterStopLossMinutes = 0;
        }

        public override void ProcessLockProfits(Transaction t, Candle lastCandle, float profit) 
        {
            try
            {
                if (profit > _botParameters.Increase / 2.0 && !string.IsNullOrEmpty(t.States) && !t.States.Contains("halftransaction"))
                {
                    Sell(t, lastCandle, t.States + ";halftransaction;takeprofit50"); // sell half of the transaction amount, 
                                                                                     // the sell function creates a new transaction and change(divide by 2) the amount of the old transaction
                }
                else if (profit > _botParameters.Increase / 10.0)
                {
                    if (t.LastProfitablePrice >= 0)
                    {
                        if (lastCandle.Close > t.LastProfitablePrice)
                        {
                            t.LastProfitablePrice = lastCandle.Close;
                            UpdateTransaction(t);
                        }
                        else
                        {
                            Sell(t, lastCandle, t.States + ";lockedremainingprofit");
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
                if (lastCandle.Close > t.LastProfitablePrice || t.LastProfitablePrice <= 0)
                {
                    t.LastProfitablePrice = lastCandle.Close;
                }

                if (lastCandle.Close <= t.LastProfitablePrice * (1.0f - _botParameters.TrailingStopValue))
                {
                    Sell(t, lastCandle, t.States + ";trailingstop");
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
                        //TaskScheduler.Instance.ScheduleTaskOnlyOnce(StartBuying, StopAfterStopLossMinutes, 0);
                    }
                }
                Sell(t, lastCandle, t.States + ";stoploss");
            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
        }

        public override void ProcessErrorTrade(Trade t)
        {
            try
            {
                if (!_backtest)
                {
                    if (t.Type == GetTransactionBuyType())
                    {
                        //UserOrder(t, userBotRelation, lastCandle);
                    }
                    else if (t.Type == GetTransactionBuyCloseType())
                    {
                        //ProcessTransaction(t, lastCandle, ref result);
                    }
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
                if (_backtest)
                {
                    //Candle lastCandle = _signalsEngine.GetCurrentCandle(TimeFrames.M1);
                    //if (t.Type == GetTransactionBuyType() || t.Type == GetTransactionSellType())
                    //{
                    //    SubscribedUsersOrder(t);
                    //}
                    //else if (t.Type == GetTransactionBuyCloseType() || t.Type == GetTransactionSellCloseType())
                    //{
                    //    CloseTrades(t, "ERROR");
                    //}
                    //if (t.Type == GetTransactionBuyType())
                    //{
                    //    Buy(lastCandle, MinimumTransactionAmount);
                    //}
                    //else if (t.Type == GetTransactionBuyCloseType())
                    //{
                    //    ProcessTransaction(t, lastCandle, ref result);
                    //}
                    //transactionErrors.Remove(t);
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
                    if (_botParameters.SmartSellTransactions)
                    {
                        SmartSell(t, lastCandle);
                    }
                    else
                    {
                        Sell(t, lastCandle, t.States);
                    }
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
                BotEngine.DebugMessage(e);
            }
            return 0;
        }

        public void ResetSmartTransactions() 
        {
            smartBuyTransactions.Clear();
            smartSellTransactions.Clear();
        }

        ///////////////////////////////////////////////////////////////////
        //////////////////////// ABSTRACT METHODS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public virtual TelegramTransaction ProcessNewTelegramTransaction()
        {
            try
            {

            }
            catch (Exception e)
            {
                BotEngine.DebugMessage(e);
            }
            return null;
        }

        public override void CloseTrade(Trade t, Transaction transaction, string description = "")
        {
            throw new NotImplementedException();
        }

        public override void CloseTrades(Transaction t, string description = "")
        {
            throw new NotImplementedException();
        }
    }
}

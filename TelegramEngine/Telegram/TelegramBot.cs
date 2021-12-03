using BotEngine.Bot;
using BotLib.Models;
using BrokerLib.Brokers;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TelegramEngine.Telegram.Channels;
using TelegramLib.Models;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace TelegramEngine.Telegram
{
    public class TelegramBot : CFDBot
    {
        public TelegramScrapper _scrapper = null;
        public readonly TelegramDBContext _telegramContext;
        public bool first = true;
        public TelegramBot(BotParameters botParameters, Channel channel)
        : base(botParameters)
        {
            _broker = Broker.DecideBroker(botParameters.BrokerDescription);
            _botParameters.BrokerId = Brokers.OANDA;
            _scrapper = new TelegramScrapper(channel);
            _telegramContext = TelegramDBContext.newDBContext();
        }

        public TelegramBot(TelegramParameters telegramParameters)
        : base(telegramParameters)
        {
            _broker = Broker.DecideBroker(telegramParameters.BrokerDescription);
            _botParameters.BrokerId = Brokers.OANDA;
            _scrapper = new TelegramScrapper(Channel.DecideChannel(telegramParameters.Channel));
            _telegramContext = TelegramDBContext.newDBContext();
        }

        public Channel[] EnabledChannels()
        {
            return null;
        }

        public float GetTransactionEURAmount(string market)
        {
            try
            {
                //TODO: t.AmountEUR , function to get eur amount
                //get EUR/currency 1 value and after convert currency 1 amount(normally 100) to eur
                string eurcurrency1 = "EUR" + market.Substring(0, 3);

            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return 0;
        }

        public Transaction GetTransactionFromTelegramTransaction(TelegramTransaction telegramTransaction, float takeProfit)
        {
            try
            {
                Transaction t = DecideTransactionType();
                if (telegramTransaction.Type == TransactionType.buy)
                {
                    t.Amount = 100;
                    //TODO: t.AmountEUR , function to get eur amount
                    //get EUR/currency 1 and after convert currency 1 amount(normally 100) to eur
                }
                else if (telegramTransaction.Type == TransactionType.sell)
                {
                    //TODO: t.AmountEUR , function to get eur amount
                    //get EUR/currency 1 and after convert currency 1 amount(normally 100) to eur
                    t.Amount = -100;
                }
                else
                {
                    return null;
                }

                t.BotId = _botParameters.id;
                t.TelegramTransactionId = telegramTransaction.id.ToString();
                t.Market = telegramTransaction.Market;
                t.Type = telegramTransaction.Type;
                t.Price = telegramTransaction.EntryValue;
                t.TakeProfit = takeProfit;
                t.StopLoss = telegramTransaction.StopLoss;
                t.Timestamp = telegramTransaction.Timestamp;

                return t;
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return null;
        }

        public List<Transaction> GetTransactionsFromTelegramTransaction(TelegramTransaction telegramTransaction) 
        {
            List<Transaction> transactions = new List<Transaction>();
            try
            {
                if (telegramTransaction.StopLoss == 0)
                {
                    return transactions;
                }
                if (telegramTransaction.TakeProfit > 0)
                {
                    transactions.Add(GetTransactionFromTelegramTransaction(telegramTransaction, telegramTransaction.TakeProfit));
                }
                if (telegramTransaction.TakeProfit2 > 0)
                {
                    transactions.Add(GetTransactionFromTelegramTransaction(telegramTransaction, telegramTransaction.TakeProfit3));
                }
                if (telegramTransaction.TakeProfit3 > 0)
                {
                    transactions.Add(GetTransactionFromTelegramTransaction(telegramTransaction, telegramTransaction.TakeProfit3));
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return transactions;

        }

        public TelegramTransaction ProcessNewTelegramTransaction()
        {
            try
            {
                if (Keepgoin)
                {

                    TelegramTransaction telegramTransaction = _scrapper.ProcessFunction();

                    if (first)
                    {
                        first = false;
                        return null;
                    }

                    if (telegramTransaction == null)
                    {
                        return null;
                    }

                    List<Transaction> allBotTransactions = new List<Transaction>();

                    List<Transaction> botTransactions = GetTransactionsFromTelegramTransaction(telegramTransaction);
                    foreach (Transaction t in botTransactions)
                    {
                        t.TelegramTransactionId = telegramTransaction.id;
                        SubscribedUsersOrder(t);
                    }
                    allBotTransactions.AddRange(botTransactions);


                    using (TelegramDBContext telegramContext = TelegramDBContext.newDBContext())
                    {
                        telegramContext.TelegramTransactions.Add(telegramTransaction);
                        telegramContext.SaveChanges();
                    }
                    using (BrokerDBContext brokerContext = BrokerDBContext.newDBContext())
                    {
                        brokerContext.Transactions.AddRange(allBotTransactions);
                        brokerContext.SaveChanges();
                    }
                    
                    return telegramTransaction;
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return null;
        }

        public override void SubscribedUsersOrder(Transaction t)
        {
            try
            {
                using (BrokerDBContext brokerContext = BrokerDBContext.newDBContext())
                {
                    using (TelegramDBContext botContext = TelegramDBContext.newDBContext())
                    {
                        List<TelegramUserBotRelation> userBotRelations = botContext.TelegramUserBotRelations.Where(m => m.BotId == _botParameters.id).ToList();
                        if (userBotRelations.Count == 0)
                        {
                            return;
                        }

                        List<Trade> trades = new List<Trade>();

                        bool save = false;

                        foreach (TelegramUserBotRelation userBotRelation in userBotRelations)
                        {
                            save = true;
                            TelegramEquity equity = botContext.TelegramEquities.Find(userBotRelation.EquityId);
                            if (equity == null)
                            {
                                TelegramEngine.DebugMessage("SubscribedUsersOrder(): TelegramEquity" + userBotRelation.EquityId + " not found. userBotRelation botId/userId: " + userBotRelation.BotId + ":" + userBotRelation.UserId);
                                continue;
                            }

                            trades.Add(_broker.Order(t, AccessPoint.Construct(userBotRelation.AccessPointId), userBotRelation.DefaultTransactionAmount));
                        }

                        if (!save)
                        {
                            return;
                        }

                        brokerContext.Trades.AddRange(trades);
                        brokerContext.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
        }

        public float ProcessTransaction(Transaction t, Candle lastCandle)
        {
            try
            {
                float currentProfit = 0.0F;
                float profit = 0;
                float fitness = CalculateSellOrderFitness(t, lastCandle, ref profit);

                if (float.IsNaN(profit))
                {
                    profit = 0.0f;
                }

                if (fitness > FitnessLimit)
                {
                    CloseTrades(t);
                }
                else if (fitness < 0) // Stop Loss
                {
                    ProcessStopLoss(t, lastCandle);
                }
                else if (_botParameters.TrailingStop)
                {
                    ProcessTrailingStop(t, lastCandle);
                }
                else if (_botParameters.LockProfits)
                {
                    ProcessLockProfits(t, lastCandle, profit);
                }
                
                currentProfit += profit;

                StoreChannelScore(t, profit);

                return currentProfit;
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return 0;
        }

        public virtual float CalculateSellOrderFitness(Transaction t, Candle lastCandle, ref float profit) 
        {
            try
            {
                float fitness = 0;
                //profit = Profit(t);
                //bool takeProfit = _botParameters.TakeProfit ? profit > _botParameters.Increase : true;

                //if (takeProfit && _botParameters.TakeProfit)
                //{
                //    fitness += 1;
                //    return fitness;
                //}

                //if (profit < -_botParameters.Decrease && _botParameters.StopLoss)
                //{
                //    fitness -= 1;
                //    return fitness;
                //}

                if (t.Type == TransactionType.buy)
                {
                    if (t.TakeProfit < lastCandle.Close)
                    {
                        fitness += 1;
                        return fitness;
                    }

                    if (t.StopLoss > lastCandle.Close)
                    {
                        fitness -= 1;
                        return fitness;
                    }
                }
                else if (t.Type == TransactionType.sell)
                {
                    if (t.TakeProfit > lastCandle.Close)
                    {
                        fitness += 1;
                        return fitness;
                    }

                    if (t.StopLoss < lastCandle.Close)
                    {
                        fitness -= 1;
                        return fitness;
                    }
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return 0;
        }

        public void SaveBotParameters()
        {
            try
            {
                TelegramParameters botParameters = (TelegramParameters) _botParameters;
                //botParameters.id = BotId;

                if (_telegramContext.TelegramScores.Any(s => s.BotParametersId == botParameters.id))
                {
                    try
                    {
                        TelegramScore botScore = _telegramContext.TelegramScores.Single(s => s.BotParametersId == botParameters.id);
                        botScore.Positions = 0;
                        botScore.Successes = 0;
                        botScore.AmountGained = 0;
                        botScore.AmountGainedDaily = 0;
                        _telegramContext.TelegramScores.Update(botScore);
                    }
                    catch (Exception e)
                    {
                        TelegramEngine.DebugMessage(e);
                    }
                }

                botParameters.Store();

            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
        }

        public override bool CloseTrades(Transaction t, string description = "")
        {
            try
            {
                if (!_backtest)
                {
                    List<Trade> trades;
                    using (BrokerDBContext brokerContext = BrokerDBContext.newDBContext())
                    {
                        trades = brokerContext.Trades.Where(m => m.TransactionId == t.id).ToList();
                    }
                    List<UserBotRelation> userBotRelations;
                    using (BotDBContext botContext = BotDBContext.newDBContext())
                    {
                        userBotRelations = botContext.UserBotRelations.Where(m => m.BotId == _botParameters.id).ToList();
                    }

                    foreach (Trade trade in trades)
                    {
                        AccessPoint accessPoint = AccessPoint.Construct(trade.AccessPointId);
                        _broker.CloseTrade(trade, t, accessPoint, description);
                    }
                }

                StoreSellOrderTransaction(t, _signalsEngine.GetCurrentCandle(_botParameters.TimeFrame), description);
                return true;
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return false;
        }

        public bool TelegramTransactionsProcessed(IEnumerable<Transaction> transactions) 
        {
            try
            {
                foreach (Transaction transaction in transactions)
                {
                    if (!transaction.Type.ToString().Contains("done"))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return false;
        }

        public virtual void StoreChannelScore(Transaction t, float gainedAmount, bool UpdateOnlyProfit = false)
        {
            try
            {
                using (var context = TelegramDBContext.newDBContext())
                {

                    if (gainedAmount > -0.000001 && gainedAmount < 0.000001)
                    {
                        gainedAmount = 0.0f;
                    }

                    ChannelScore score;

                    if (!context.ChannelScores.Any(s => s.BotParametersId == _botParameters.id))
                    {
                        TelegramTransaction telegramTransaction = context.TelegramTransactions.Find(t.TelegramTransactionId);
                        score = new ChannelScore();
                        score.Positions = 0;
                        score.BotParametersId = _botParameters.id;
                        score.Successes = 0;
                        score.ActiveTransactions = 0;
                        score.CurrentProfit = 0.0f;
                        score.ChannelName = telegramTransaction.Channel;
                        context.ChannelScores.Add(score);
                        context.SaveChanges();
                    }
                    score = context.ChannelScores.Single(s => s.BotParametersId == _botParameters.id);

                    if (!UpdateOnlyProfit)
                    {
                        if (gainedAmount > 0)
                        {
                            score.Successes += 1;
                        }
                        score.Positions += 1;
                        score.AmountGained += gainedAmount;
                        score.AmountGainedDaily += gainedAmount;
                    }
                    //score.ActiveTransactions = CurrentTransactions;

                    //score.CurrentProfit = (float.IsNaN(CurrentProfit) || float.IsInfinity(CurrentProfit)) ? 0.0f : CurrentProfit;

                    context.ChannelScores.Update(score);

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
        }

        ///////////////////////////////////////////////////////////////////
        //////////////////////// STATIC FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public static TelegramBot GenerateTelegramBotFromParameters(TelegramParameters telegramParameters)
        {
            try
            {
                using (BotDBContext context = BotDBContext.newDBContext())
                {
                    TelegramBot bot = new TelegramBot(telegramParameters, Channel.DecideChannel(telegramParameters.Channel));
                    bot.RecreateSmartSellTransactions(telegramParameters.SmartSellTransactions);
                    return bot;
                }
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return null;
        }

        public static TelegramBot GenerateRandomTelegramBot(int botId = -1, int channelId = -1)
        {
            try
            {
                BotParameters botParameters = GenerateRandomBotParameters(TimeFrames.M1, "", null, null, null, false);
                TelegramParameters telegramParameters = new TelegramParameters(botParameters);
                telegramParameters.Channel = channelId == -1 ? Channel.RandomChannelURL() : Channel.DecideChannelURL(channelId);
                return GenerateTelegramBotFromParameters(telegramParameters);
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return null;
        }
    }
}

using BotEngine.Bot;
using BotLib.Models;
using BrokerLib.Brokers;
using BrokerLib.Models;
using SignalsEngine.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using TelegramEngine.Telegram.Channels;
using TelegramLib.Models;
using static BrokerLib.BrokerLib;

namespace TelegramEngine.Telegram
{
    public class TelegramBot : CFDBot
    {
        public TelegramScrapper _scrapper = null;
        public readonly TelegramDBContext _telegramContext;
        public bool first = true;
        public TelegramBot(BotParameters botParameters, Channel channel, bool backtest = false)
        : base(botParameters, backtest)
        {
            //_broker = Broker.DecideBroker(botParameters.BrokerDescription);
            _botParameters.BrokerId = Brokers.OANDA;
            _scrapper = new TelegramScrapper(channel);
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

        public Transaction GetTransactionFromTelegramTransaction(TelegramTransaction telegramTransaction, float takeProfit, string description)
        {
            try
            {
                Transaction t = DecideTransactionType();
                if (telegramTransaction.Type == TransactionType.buy)
                {
                    var lastCandles = IndicatorsSharedData.Instance.GetLastCandles(new BrokerLib.Market.MarketDescription(telegramTransaction.Market, MarketTypes.Forex, BrokerType.margin), _botParameters.TimeFrame);
                    var candle = lastCandles.LastOrDefault() ?? new Candle();
                    t.Amount = 100;
                    t.AmountSymbol2 = candle.Open > 0 ? t.Amount/candle.Open : 100;
                }
                else if (telegramTransaction.Type == TransactionType.sell)
                {
                    var lastCandles = IndicatorsSharedData.Instance.GetLastCandles(new BrokerLib.Market.MarketDescription(telegramTransaction.Market, MarketTypes.Forex, BrokerType.margin), _botParameters.TimeFrame);
                    var candle = lastCandles.LastOrDefault() ?? new Candle();
                    t.Amount = -100;
                    t.AmountSymbol2 = candle.Open > 0 ? t.Amount / candle.Open : 100;
                }
                else
                {
                    return null;
                }
                t.id = telegramTransaction.id + description;
                t.BotId = _botParameters.id;
                t.TelegramTransactionId = telegramTransaction.id ?? string.Empty;
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

        public List<Transaction> GetTransactionsFromTelegramTransactions(List<TelegramTransaction> telegramTransactions)
        {
            List<Transaction> transactions = new List<Transaction>();
            foreach (var telegramTransaction in telegramTransactions)
            {
                transactions.AddRange(GetTransactionsFromTelegramTransaction(telegramTransaction));
            }
            return transactions;

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
                    transactions.Add(GetTransactionFromTelegramTransaction(telegramTransaction, telegramTransaction.TakeProfit, "tp1"));
                }
                if (telegramTransaction.TakeProfit2 > 0)
                {
                    transactions.Add(GetTransactionFromTelegramTransaction(telegramTransaction, telegramTransaction.TakeProfit3, "tp2"));
                }
                if (telegramTransaction.TakeProfit3 > 0)
                {
                    transactions.Add(GetTransactionFromTelegramTransaction(telegramTransaction, telegramTransaction.TakeProfit3, "tp3"));
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

                    if (telegramTransaction == null || !telegramTransaction.IsConsistent())
                    {
                        return null;
                    }

                    List<Transaction> allBotTransactions = new List<Transaction>();

                    List<Transaction> botTransactions = GetTransactionsFromTelegramTransaction(telegramTransaction);
                    foreach (Transaction t in botTransactions)
                    {
                        t.id = telegramTransaction.id ?? Guid.NewGuid().ToString();
                        t.TelegramTransactionId ??= Guid.NewGuid().ToString();
                        SubscribedUsersOrder(t);
                    }
                    allBotTransactions.AddRange(botTransactions);


                    using (TelegramDBContext telegramContext = TelegramDBContext.newDBContext())
                    {
                        telegramTransaction.id ??= Guid.NewGuid().ToString();
                        telegramContext.TelegramTransactions.Add(telegramTransaction);
                        telegramContext.SaveChanges();
                    }
                    BrokerDBContext.Execute(brokerContext => {
                        brokerContext.Transactions.AddRange(allBotTransactions);
                        return brokerContext.SaveChanges();
                    }, true);
                    
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

                    BrokerDBContext.Execute(brokerContext => {
                        brokerContext.Trades.AddRange(trades);
                        return brokerContext.SaveChanges();
                    }, true);
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
                if (t.Timestamp > lastCandle.Timestamp)
                {
                    return 0;
                }
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

                //StoreChannelScore(t, profit);

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
                if (!BotLib.BotLib.Backtest)
                {
                    List<Trade> trades = BrokerDBContext.Execute(brokerContext => {
                        return brokerContext.Trades.Where(m => m.TransactionId == t.id).ToList();
                    });

                    List<UserBotRelation> userBotRelations = BotDBContext.Execute(botContext => {
                        return botContext.UserBotRelations.Where(m => m.BotId == _botParameters.id).ToList();
                    });

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
                        score = new ChannelScore();
                        score.Positions = 0;
                        score.BotParametersId = _botParameters.id;
                        score.Successes = 0;
                        score.ActiveTransactions = 0;
                        score.CurrentProfit = 0.0f;
                        score.ChannelName = _botParameters.Channel;
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

        public static TelegramBot GenerateTelegramBotFromParameters(TelegramParameters telegramParameters, bool backtest = false)
        {
            try
            {
                TelegramBot bot = new TelegramBot(telegramParameters, Channel.DecideChannel(telegramParameters.Channel), true);
                //bot.RecreateSmartSellTransactions(telegramParameters.SmartSellTransactions);
                return bot;
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e);
            }
            return null;
        }

        public static TelegramBot GenerateTelegramBotFromChannelUrl(string channelUrl, TimeFrames timeFrame, bool backtest = false)
        {
            try
            {
                TelegramParameters telegramParameters = new TelegramParameters()
                {
                    id = Guid.NewGuid().ToString(),
                    TimeFrame = timeFrame,
                    Channel = channelUrl,
                    BrokerDescription = new BrokerDescription(Brokers.OANDA, BrokerType.margin),
                    TakeProfit = true,
                    StopLoss = true,
                    TrailingStop = false,
                    LockProfits = false,
                };
                TelegramBot bot = new TelegramBot(telegramParameters, new CustomChannel(channelUrl), backtest);
                return bot;
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
                telegramParameters.BrokerDescription.BrokerId = Brokers.OANDA;
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

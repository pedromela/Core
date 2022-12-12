using BotEngine;
using BotEngine.Bot;
using BotLib.Models;
using System;
using UtilsLib.Utils;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using BacktesterLib.Models;
using static BrokerLib.BrokerLib;
using BrokerLib.Models;
using BrokerLib.Brokers;
using System.Threading;
using TelegramLib.Models;
using TelegramEngine.Telegram;
using TelegramEngine.Telegram.Channels;
using SignalsEngine.Indicators;
using BrokerLib.Market;
using BacktesterLib.Lib;
using Microsoft.EntityFrameworkCore;

namespace BacktesterEngine
{
    public class BacktesterEngine : TradingEngine
    {
        private readonly BacktesterDBContext _backtesterContext = null;
        protected readonly TelegramDBContext _telegramContext;

        private DateTime _fromDate;
        private DateTime _toDate;

        public BacktesterEngine(DateTime fromDate, DateTime toDate)
        : base()
        {
            _backtesterContext = BacktesterDBContext.newDBContext();
            _telegramContext = TelegramDBContext.newDBContext();
            _fromDate = fromDate;
            _toDate = toDate;
            BotLib.BotLib.Backtest = true;
            BotDBContext.InitProviders();
            BrokerDBContext.InitProviders();
            //Run();
        }

        public override void Run()
        {
            try
            {
                AutoResetEvent autoEvent = new AutoResetEvent(false);

                Init();
                WaitForFirstCandles();
                Started = true;
                UpdateCycle();

                autoEvent.WaitOne();
            }
            catch (Exception e)
            {
                BacktesterEngine.DebugMessage(e);
            }
        }

        protected virtual Dictionary<BrokerDescription, List<MarketInfo>> GetActiveBrokerMarketsDictForTelegramBots()
        {
            try
            {
                Dictionary<BrokerDescription, List<MarketInfo>> activeBrokerMarketsDict = new Dictionary<BrokerDescription, List<MarketInfo>>();
                //foreach (Brokers brokerType in Enum.GetValues(typeof(Brokers)))
                //{
                //    if (activeBrokerMarketsDict.ContainsKey(brokerType))
                //    {
                //        continue;
                //    }
                //    Broker broker = Broker.DecideBroker(brokerType);
                //    activeBrokerMarketsDict.Add(brokerType, broker.GetMarketInfos());
                //}

                OANDA broker = new OANDA();
                activeBrokerMarketsDict.Add(new BrokerDescription(Brokers.OANDA, BrokerType.margin), broker.GetMarketInfos());
                
                return activeBrokerMarketsDict;
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
            return null;
        }

        public void BacktestAllTelegramBots()
        {
            try
            {
                List<TelegramParameters> telegramParameters = _telegramContext.GetBotsFromDB();
                foreach (TelegramParameters bot in telegramParameters)
                {
                    BacktestTelegramBot(bot.id);
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        public void CleanBotBacktestingData(string botId) 
        {
            try
            {
                List<BacktesterTransaction> backtesterTransactions = _backtesterContext.BacktesterTransactions.Where(m => m.BotId == botId).ToList();
                _backtesterContext.BacktesterTransactions.RemoveRange(backtesterTransactions);

                List<BacktesterScore> backtesterScores = _backtesterContext.BacktesterScores.Where(m => m.BotParametersId == botId).ToList();
                _backtesterContext.BacktesterScores.RemoveRange(backtesterScores);

                _backtesterContext.SaveChanges();
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        protected void InitBot(BotParameters botParameters, DateTime fromDate, DateTime toDate)
        {
            try
            {
                foreach (var provider in BrokerDBContext.providers)
                {
                    using (BrokerDBContext brokerContext = (BrokerDBContext)provider.GetDBContext())
                    {
                        if (brokerContext.GetAccessPointsCountFromDB() == 0)
                        {
                            AccessPoint ap = new AccessPoint();
                            ap.Account = "001-004-5079852-001";
                            ap.BearerToken = "9ed78c473d719960490b47ba40737394-82f905244392e3832ccb565ba07d90c1";
                            ap.id = "1";
                            ap.UserId = "9cd94fe9-d439-4883-b508-058e02758e5d";
                            ap.BrokerId = 1;
                            //ap.id = Guid.NewGuid().ToString();
                            brokerContext.AccessPoints.Add(ap);
                            brokerContext.SaveChanges();
                        }
                    }
                }
                List<BotParameters> botsParametersList = new List<BotParameters>();
                botsParametersList.Add(botParameters);
                botsParametersList[0].BrokerDescription = new BrokerDescription(botsParametersList[0].BrokerId, botsParametersList[0].BrokerType);
                

                Dictionary<BrokerDescription, List<string>> activeBrokerMarketStringsDict = GetActiveBrokerMarketStringsDict(botsParametersList);
                Broker.InitBrokers(activeBrokerMarketStringsDict, true);
                Dictionary<Broker, List<MarketInfo>> activeBrokerMarketsDict = GetActiveBrokerMarketsDict(botsParametersList);
                IndicatorsSharedData.InitInstance(activeBrokerMarketsDict, true);

                foreach (var pair in activeBrokerMarketsDict)
                {
                    Broker broker = pair.Key;
                    IndicatorsEngine signalsEngine = null;
                    foreach (MarketInfo marketInfo in pair.Value)
                    {
                        string signalsEngineId = IndicatorsEngine.DecideSignalsEngineId(broker.GetBrokerId(), marketInfo.GetMarket());
                        if (_signalsEngineDict.ContainsKey(signalsEngineId))
                        {
                            continue;
                        }
                        signalsEngine = new IndicatorsEngine(broker.GetBrokerId(), marketInfo, fromDate, toDate, botParameters.TimeFrame);
                        _signalsEngineDict.Add(signalsEngineId, signalsEngine);
                    }
                }

                WaitForFirstCandles();

                if (botsParametersList.Count > 0)
                {
                    Broker broker = Broker.DecideBroker(botParameters.BrokerDescription);
                    MarketDescription marketDescription = new MarketDescription(botParameters.Market, broker.GetMarketType(), broker.GetBrokerType());
                    MarketInfo marketInfo = IndicatorsSharedData.Instance.GetMarketInfo(marketDescription, botParameters.TimeFrame);
                    if (marketInfo == null || !botParameters.ValidStart(marketInfo))
                    {
                        DebugMessage("BacktesterEngine::Init() : bot " + botParameters.id + " start validation failed.");
                        return;
                    }

                    BotBase bot = BotBase.GenerateBotFromParameters(botParameters, true);
                    if (!_botDict.ContainsKey(botParameters.TimeFrame))
                    {
                        if (Enum.IsDefined(typeof(TimeFrames), bot._botParameters.TimeFrame))
                        {
                            _botDict.Add(botParameters.TimeFrame, new Dictionary<string, BotBase>());
                        }
                        else
                        {
                            DebugMessage("BacktesterEngine::Init() : bot " + botParameters.id + " TimeFrame is invalid.");

                        }
                    }

                    _botDict[botParameters.TimeFrame].Add(botParameters.id, bot);
                    DebugMessage(String.Format("BacktesterEngine::Init() : Adding bot {1}/{0}", botParameters.id, botParameters.BotName));
                }
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                Configuration = builder.Build();

            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        public void BacktestBot(string BotId, DateTime fromDate, DateTime toDate, IObserver<BacktestData> observer)
        {
            try
            {
                BotParameters botParameters;
                botParameters = BotDBContext.Execute(botContext =>
                {
                    return botContext.BotsParameters.Find(BotId);
                });
                CleanBotBacktestingData(BotId);
                var complete = new BacktestData();
                var score = new BacktesterScore();
                score.Positions = -2;
                score.Successes = -2;
                if (botParameters == null)
                {
                    complete.Update(score, DateTime.Now, null, null, null, null, BacktestingState.error);
                    observer?.OnNext(complete);
                    return;
                }
                InitBot(botParameters, fromDate, toDate);
                BotBase bot = _botDict[botParameters.TimeFrame][botParameters.id];
                IndicatorsEngine indicatorsEngine = _signalsEngineDict[bot._signalsEngineId];
                long count = 1;
                bool first = true;
                while(!indicatorsEngine.CandlesEnd(bot._botParameters.TimeFrame))
                {

                    //signalsEngine.indicatorsEngine.UpdateCycle();
                    indicatorsEngine.UpdateIndicators(bot._botParameters.TimeFrame);
                    bot.UpdateSignals(indicatorsEngine);
                    bot.ProcessTransactions();
                    bot.BacktestDataUpdate(indicatorsEngine.GetCurrentCandle(bot._botParameters.TimeFrame).Timestamp);
                    if (count % 100 == 0)
                    {
                        bot._backtestData.State = first ? BacktestingState.first : BacktestingState.running;
                        first = false;
                        observer?.OnNext(bot._backtestData);
                        bot.BacktestDataReset();
                    }
                    count++;
                }
                BacktesterEngine.DebugMessage($"Backtest {bot._botParameters.id} completed!");
                score.Positions = -2;
                score.Successes = -2;
                complete.Update(score, DateTime.Now, null, null, null, null, BacktestingState.completed);
                observer?.OnNext(complete);
            }
            catch (Exception e)
            {
                BacktesterEngine.DebugMessage(e);
            }
        }

        public void BacktestInvertedTelegramBot(string BotId) 
        {
            try
            {
                TelegramParameters telegramParameters = _telegramContext.TelegramParameters.Find(BotId);

                List<TelegramTransaction> telegramTransactionsHistoric = _telegramContext.TelegramTransactions.Where(m => m.Channel == Channel.DecideChannelName(telegramParameters.Channel)).ToList();
                if (telegramTransactionsHistoric.Count > 0)
                {
                    telegramTransactionsHistoric = TelegramTransaction.InvertTelegramTransactions(telegramTransactionsHistoric);
                    BacktestTelegramBot(BotId, telegramTransactionsHistoric);
                }
            }
            catch (Exception e)
            {
                BacktesterEngine.DebugMessage(e);
            }
        }
        public void BacktestTelegramBot(string BotId, List<TelegramTransaction> telegramTransactionsHistoric = null)
        {
            try
            {
                TelegramParameters telegramParameters = _telegramContext.TelegramParameters.Find(BotId);

                if (telegramTransactionsHistoric == null)
                {
                   telegramTransactionsHistoric = _telegramContext.TelegramTransactions.Where(m => m.Channel == Channel.DecideChannelName(telegramParameters.Channel)).ToList();
                }

                CleanBotBacktestingData(BotId);
                
                _signalsEngineDict.Clear();

                
                if (telegramTransactionsHistoric.Count > 0)
                {
                    DateTime fromDate = _telegramContext.TelegramTransactions.Select(m => m.Timestamp).Min();
                    DateTime toDate = _telegramContext.TelegramTransactions.Select(m => m.Timestamp).Max();

                    TelegramBot bot = new TelegramBot(telegramParameters);
                    bot._backtest = true;

                    fromDate = DateTimeExtensions.Normalize(fromDate, (int)bot._botParameters.TimeFrame);
                    toDate = DateTimeExtensions.Normalize(toDate, (int)bot._botParameters.TimeFrame);

                    Dictionary<BrokerDescription, List<MarketInfo>> activeBrokerMarketsDict = GetActiveBrokerMarketsDictForTelegramBots();

                    foreach (var pair in activeBrokerMarketsDict)
                    {
                        Broker broker = Broker.DecideBroker(pair.Key);
                        IndicatorsEngine signalsEngine = null;
                        foreach (MarketInfo marketInfo in pair.Value)
                        {
                            signalsEngine = new IndicatorsEngine(pair.Key.BrokerId, marketInfo, fromDate, toDate, bot._botParameters.TimeFrame);
                            _signalsEngineDict.Add(IndicatorsEngine.DecideSignalsEngineId(pair.Key.BrokerId, marketInfo.GetMarket()), signalsEngine);
                        }
                    }

                    foreach (TelegramTransaction telegramTransaction in telegramTransactionsHistoric)
                    {
                        if (!telegramTransaction.IsConsistent())
                        {
                            DebugMessage(String.Format("BacktestTelegramBot({0}) : telegramTransaction {1} not consistent...", bot._botParameters.id, telegramTransaction.id));
                            continue;
                        }
                        //find signals engine for this market
                        IndicatorsEngine indicatorsEngine = _signalsEngineDict[IndicatorsEngine.DecideSignalsEngineId(Brokers.OANDA, telegramTransaction.Market)];
                        //process indicators at the moment of the transaction
                        indicatorsEngine.ProcessIndicatorsAtDate(bot._botParameters.TimeFrame, telegramTransaction.Timestamp);
                        //Process transaction
                        bot.UpdateSignals(indicatorsEngine);
                        List<Transaction> botTransactions = bot.GetTransactionsFromTelegramTransaction(telegramTransaction);
                        List<BacktesterTransaction> backtesterBotTransactions = new List<BacktesterTransaction>();
                        foreach (Transaction t in botTransactions)
                        {
                            BacktesterTransaction backtesterTransaction = new BacktesterTransaction(t);
                            backtesterBotTransactions.Add(backtesterTransaction);
                            backtesterTransaction.Store();
                        }

                        while (!indicatorsEngine.CandlesEnd(bot._botParameters.TimeFrame) && !bot.TelegramTransactionsProcessed(backtesterBotTransactions))
                        {

                            //signalsEngine.indicatorsEngine.UpdateCycle();
                            indicatorsEngine.UpdateIndicators(bot._botParameters.TimeFrame);
                            bot.UpdateSignals(indicatorsEngine);
                            //bot.ProcessTransactions();
                            foreach (BacktesterTransaction t in backtesterBotTransactions)
                            {
                                bot.ProcessTransaction(t, indicatorsEngine.GetCurrentCandle(bot._botParameters.TimeFrame));
                            }
                        }
                    }

                }


            }
            catch (Exception e)
            {
                BacktesterEngine.DebugMessage(e);
            }
        }

        public override void UpdateCycle()
        {
            try
            {
                DebugMessage("############################################################");
                DebugMessage("Backtesting Bots...");
                MyTaskScheduler.Instance.ScheduleTaskByMilisecond(BacktestBotsM1,     "BacktestBotsM1",   100);
                MyTaskScheduler.Instance.ScheduleTaskByMilisecond(BacktestBotsM5,     "BacktestBotsM5",   100);
                MyTaskScheduler.Instance.ScheduleTaskByMilisecond(BacktestBotsM15,    "BacktestBotsM15",  100);
                MyTaskScheduler.Instance.ScheduleTaskByMilisecond(BacktestBotsM30,    "BacktestBotsM30",  100);
                MyTaskScheduler.Instance.ScheduleTaskByMilisecond(BacktestBotsH1,     "BacktestBotsH1",   100);
                DebugMessage("############################################################");
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private void BacktestBotsM1()
        {
            try
            {
                if (Started)
                {
                    BacktestByTimeFrame(TimeFrames.M1);
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }
        private void BacktestBotsM5()
        {
            try
            {
                if (Started)
                {
                    BacktestByTimeFrame(TimeFrames.M5);
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private void BacktestBotsM15()
        {
            try
            {
                if (Started)
                {
                    BacktestByTimeFrame(TimeFrames.M15);
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private void BacktestBotsM30()
        {
            try
            {
                if (Started)
                {
                    BacktestByTimeFrame(TimeFrames.M30);
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private void BacktestBotsH1()
        {
            try
            {
                if (Started)
                {
                    BacktestByTimeFrame(TimeFrames.H1);
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private void UpdateCycle(IndicatorsEngine indicatorsEngine)
        {
            try
            {
                foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                {
                    indicatorsEngine.UpdateIndicators(timeFrame);
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private void BacktestByTimeFrame(TimeFrames timeFrame)
        {
            try
            {
                foreach (var bot in _botDict[timeFrame].Values)
                {
                    if (bot._botParameters.TimeFrame == timeFrame)
                    {
                        if (!_signalsEngineDict.ContainsKey(bot._signalsEngineId))
                        {
                            DebugMessage("BacktestByTimeFrame(TimeFrames): " + bot._signalsEngineId + " is not present in _signalsEngineDict");
                            continue;
                        }
                        UpdateCycle(_signalsEngineDict[bot._signalsEngineId]);
                        bot.UpdateSignals(_signalsEngineDict[bot._signalsEngineId]);
                        bot.ProcessTransactions();
                    }
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        protected override void CleanUnusedData()
        {
            try
            {
                DebugMessage("############################################################");
                DebugMessage("Cleaning Database...");

                List<BacktesterScore> scores = _backtesterContext.BacktesterScores.ToList();
                List<BacktesterTransaction> transactions = _backtesterContext.BacktesterTransactions.ToList();

                List<BacktesterScore> scoresToRemove = new List<BacktesterScore>();
                List<BacktesterTransaction> transactionsToRemove = new List<BacktesterTransaction>();

                BotDBContext.Execute(botContext =>
                {
                    foreach (BacktesterScore score in scores)
                    {
                        if (!botContext.BotsParameters.AsNoTracking().Any(m => m.id == score.BotParametersId))
                        {
                            scoresToRemove.Add(score);
                        }
                    }
                    return 0;
                });

                _backtesterContext.BacktesterScores.RemoveRange(scoresToRemove);

                BotDBContext.Execute(botContext =>
                {
                    foreach (BacktesterTransaction transaction in transactions)
                    {
                        if (!botContext.BotsParameters.AsNoTracking().Any(m => m.id == transaction.BotId))
                        {
                            transactionsToRemove.Add(transaction);
                        }
                    }
                    return 0;
                });

                _backtesterContext.BacktesterTransactions.RemoveRange(transactionsToRemove);
                DebugMessage("Clean done.");
                DebugMessage("############################################################");


            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }



    }
}

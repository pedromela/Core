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
using SignalsEngine;
using OpenQA.Selenium;
using NLog.Targets;

namespace BacktesterEngine
{
    public class BacktesterEngine : TradingEngine
    {
        private readonly BacktesterDBContext _backtesterContext = null;
        protected readonly TelegramDBContext _telegramContext;

        public BacktesterEngine()
        : base()
        {
            _backtesterContext = BacktesterDBContext.newDBContext();
            _telegramContext = TelegramDBContext.newDBContext();
            BotLib.BotLib.Backtest = true;
            BotDBContext.InitProviders();
            BrokerDBContext.InitProviders();
        }

        public override void Run()
        {
            try
            {
                AutoResetEvent autoEvent = new AutoResetEvent(false);

                Init();
                WaitForFirstCandles(_signalsEngineDict.Values.ToList());
                Started = true;
                UpdateCycle();

                autoEvent.WaitOne();
            }
            catch (Exception e)
            {
                BacktesterEngine.DebugMessage(e);
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
                List<BotParameters> botsParametersList = new List<BotParameters>
                {
                    botParameters
                };
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

                WaitForFirstCandles(_signalsEngineDict.Values.ToList());

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
                    bot._score.Update();
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

        public void BacktestTelegramChannel(string channelUrl)
        {
            TelegramChromeController controller = new TelegramChromeController(channelUrl);
            controller.LoadPage();
            controller.GoToTopOfPage();
            var messages = controller.GetAllMessages();
            Channel[] channels = new Channel[]
            {
                new CustomChannel(channelUrl)
            };
            List<TelegramTransaction> telegramTransactionsHistoric = new List<TelegramTransaction>();
            foreach (var message in messages)
            {
                try
                {
                    IWebElement dateElement = message.FindElement(By.XPath(".//a[@class='tgme_widget_message_date']/time[@class='time']"));
                    DateTime date = DateTime.Parse(dateElement.GetAttribute("datetime"));
                    IWebElement messageElement = message.FindElement(By.XPath(".//div[@class='tgme_widget_message_text js-message_text before_footer']"));
                    foreach (var channel in channels)
                    {
                        TelegramTransaction result = channel.Parse(messageElement.Text);
                        
                        if (result != null && result.IsConsistent())
                        {
                            result.Timestamp = date;
                            telegramTransactionsHistoric.Add(result);
                        }
                    }
                }
                catch (Exception e)
                {
                    BacktesterEngine.DebugMessage(e);
                }
            }
            controller.browser.Quit();

            TelegramParameters telegramParameters = new TelegramParameters()
            {
                id = Guid.NewGuid().ToString(),
                TimeFrame = TimeFrames.M1,
                Channel = channelUrl,
                BrokerDescription = new BrokerDescription(Brokers.OANDA, BrokerType.margin),
                TakeProfit = true,
                StopLoss = true,
                TrailingStop = false,
                LockProfits = false,
            };
            telegramParameters.Store();
            TelegramBot bot = TelegramBot.GenerateTelegramBotFromParameters(telegramParameters, true);
            telegramTransactionsHistoric.ForEach(t => t.id = Guid.NewGuid().ToString());
            BacktestTelegramBot(bot, telegramTransactionsHistoric);
        }

        public void BacktestTelegramBot(TelegramBot bot, List<TelegramTransaction> telegramTransactionsHistoric = null, bool invertedTransactions = false)
        {
            try
            {
                if (telegramTransactionsHistoric == null)
                {
                   telegramTransactionsHistoric = _telegramContext.TelegramTransactions.Where(m => m.Channel == Channel.DecideChannelName(bot._botParameters.Channel)).ToList();
                }

                CleanBotBacktestingData(bot._botParameters.id);
                
                _signalsEngineDict.Clear();

                if (invertedTransactions)
                {
                    telegramTransactionsHistoric = TelegramTransaction.InvertTelegramTransactions(telegramTransactionsHistoric);
                }

                
                if (telegramTransactionsHistoric.Count > 0)
                {
                    OANDA broker = new OANDA();
                    Dictionary<Broker, List<MarketInfo>> activeBrokerMarketsDict = new Dictionary<Broker, List<MarketInfo>>
                    {
                        { broker, new List<MarketInfo>() }
                    };
                    IndicatorsSharedData.InitInstance(activeBrokerMarketsDict, true);

                    //group transactions by market

                    var transactionGroupedByMarket = telegramTransactionsHistoric.SkipWhile(t => !t.IsConsistent()).GroupBy(t => t.Market);
                    foreach (var group in transactionGroupedByMarket)
                    {
                        //create signals engine if not created before
                        if (!_signalsEngineDict.ContainsKey(IndicatorsEngine.DecideSignalsEngineId(broker.GetBrokerId(), group.First().Market)))
                        {
                            DateTime fromDate = group.Select(m => m.Timestamp).Min();
                            DateTime toDate = group.Select(m => m.Timestamp).Max();

                            fromDate = DateTimeExtensions.Normalize(fromDate, (int)bot._botParameters.TimeFrame);
                            toDate = DateTimeExtensions.Normalize(toDate, (int)bot._botParameters.TimeFrame);

                            MarketInfo marketInfo = new MarketInfo(group.First().Market, broker);
                            if (!activeBrokerMarketsDict[broker].Any(marketInfo => marketInfo.GetMarket() == group.First().Market))
                            {
                                activeBrokerMarketsDict[broker].Add(marketInfo);
                            }
                            IndicatorsEngine signalsEngine = new IndicatorsEngine(broker.GetBrokerId(), marketInfo, fromDate, toDate, bot._botParameters.TimeFrame);
                            IndicatorsSharedData.Instance.AddMarkets(activeBrokerMarketsDict);
                            _signalsEngineDict.Add(IndicatorsEngine.DecideSignalsEngineId(broker.GetBrokerId(), marketInfo.GetMarket()), signalsEngine);
                        }
                        //find signals engine for this market
                        IndicatorsEngine indicatorsEngine = _signalsEngineDict[IndicatorsEngine.DecideSignalsEngineId(Brokers.OANDA, group.First().Market)];
                        //process indicators at the moment of the transaction
                        indicatorsEngine.ProcessIndicatorsAtDate(bot._botParameters.TimeFrame, group.First().Timestamp);
                        bot.UpdateSignals(indicatorsEngine);
                        //Process transaction
                        List<Transaction> botTransactions = bot.GetTransactionsFromTelegramTransactions(group.ToList());
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
                            indicatorsEngine.UpdateCandleData(bot._botParameters.TimeFrame);
                            bot.UpdateSignals(indicatorsEngine);
                            //bot.ProcessTransactions();
                            foreach (BacktesterTransaction t in backtesterBotTransactions)
                            {
                                bot.ProcessTransaction(t, indicatorsEngine.GetCurrentCandle(bot._botParameters.TimeFrame));
                            }
                        }
                    }
                    bot._score.Store();
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

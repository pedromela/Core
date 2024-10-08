﻿using BotLib.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using UtilsLib.Utils;
using Newtonsoft.Json;
using System.Linq;
using static BrokerLib.BrokerLib;
using BrokerLib.Models;
using BrokerLib.Brokers;
using SignalsEngine.Indicators;
using System.Threading.Tasks;
using BrokerLib.Market;
using BotEngine.Bot;
using Microsoft.EntityFrameworkCore;

namespace BotEngine
{
    public record Pair 
    {
        public bool Ready { get; set; }
        public object Lock { get; set; }

    }
    public class BotEngine : TradingEngine
    {

        private Dictionary<TimeFrames, Pair> ready = new Dictionary<TimeFrames, Pair>(); 
        public int iteration = 0;
        public int iterationPrint = 0;

        public BotEngine(bool async = false)
        : base()
        {
            InitReadyList();

            if (async)
            {
            }
            else
            {
                Init().Subscribe((loading) => {
                    if (loading == 100)
                    {
                        UpdateCycle();
                        Evolutions();
                    }
                });
                Started = true;
                Run();
            }
        }

        public void Evolutions() 
        {
            _geneticAlgorithm.Evolutions();
        }

        public IEnumerable<BotParameters> GetBotParametersList() 
        {
            List<BotParameters> list = new List<BotParameters>();
            foreach (var item in _botDict.Values)
            {
                list.AddRange(item.Values.Select(m => m._botParameters).ToList());
            }
            return list;
        }

        public IEnumerable<Transaction> GetTransactionErrors() 
        {
            List<Transaction> list = new List<Transaction>();
            foreach (var item in _botDict.Values)
            {
                List<BotBase> botList = item.Values.ToList();
                foreach (var bot in botList)
                {
                    list.AddRange(bot.transactionErrors);
                }
            }
            return list;
        }


        public IEnumerable<Trade> GetTradeErrors()
        {
            List<Trade> list = new List<Trade>();
            foreach (var item in _botDict.Values)
            {
                List<BotBase> botList = item.Values.ToList();
                foreach (var bot in botList)
                {
                    list.AddRange(bot.tradeErrors);
                }
            }
            return list;
        }

        protected void InitReadyList() 
        {
            foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
            {
                ready.Add(timeFrame, new Pair() { Ready = true, Lock = new object() });
            }
        }

        public override void Run()
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-PT");

                AutoResetEvent autoEvent = new AutoResetEvent(false);

                autoEvent.WaitOne();
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        public override void UpdateCycle()
        {
            try
            {
                DebugMessage("############################################################");
                DebugMessage("Starting Bots...");
                MyTaskScheduler.Instance.ScheduleTaskByInitOfMinute<BotEngine>(UpdateBotsM1, "UpdateBotsM1", this, 1, 5, 0);
                MyTaskScheduler.Instance.ScheduleTaskByInitOfMinute<BotEngine>(UpdateBotsM5, "UpdateBotsM5", this, 5, 5, 1);
                MyTaskScheduler.Instance.ScheduleTaskByInitOfMinute<BotEngine>(UpdateBotsM15, "UpdateBotsM15", this, 15, 5, 1);
                MyTaskScheduler.Instance.ScheduleTaskByInitOfMinute<BotEngine>(UpdateBotsM30, "UpdateBotsM30", this, 30, 5, 1);
                MyTaskScheduler.Instance.ScheduleTaskByInitOfMinute<BotEngine>(UpdateBotsH1, "UpdateBotsH1", this, 60, 5, 1);
                MyTaskScheduler.Instance.ScheduleTaskByInitOfMinute<BotEngine>(PrintMeasures, "PrintMeasuresBots", this, 1, 55, 0);
                MyTaskScheduler.Instance.ScheduleTaskByInitOfMinute<BotEngine>(UpdateUserBots, "UpdateUserBots", this, 1, 50, 0);
                DebugMessage("############################################################");
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private static void UpdateBotsM1(BotEngine botEngine)
        {
            try
            {
                botEngine.UpdateByTimeFrame(TimeFrames.M1);
                botEngine.iteration++;
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }
        private static void UpdateBotsM5(BotEngine botEngine)
        {
            try
            {
                botEngine.UpdateByTimeFrame(TimeFrames.M5);
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private static void UpdateBotsM15(BotEngine botEngine)
        {
            try
            {
                botEngine.UpdateByTimeFrame(TimeFrames.M15);
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private static void UpdateBotsM30(BotEngine botEngine)
        {
            try
            {
                botEngine.UpdateByTimeFrame(TimeFrames.M30);
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private static void UpdateBotsH1(BotEngine botEngine)
        {
            try
            {
                botEngine.UpdateByTimeFrame(TimeFrames.H1);
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private void UpdateByTimeFrame(TimeFrames timeFrame)
        {
            try
            {
                if (!Started)
                {
                    DebugMessage(String.Format("BotEngine::UpdateByTimeFrame({0}): Not started.", timeFrame));

                    return;
                }

                if (!_botDict.ContainsKey(timeFrame))
                {
                    DebugMessage(String.Format("BotEngine::UpdateByTimeFrame({0}): TimeFrame is not present in _signalsEngineDict", timeFrame));
                    return;
                }

                lock (ready[timeFrame].Lock)
                {
                    if (!ready[timeFrame].Ready)
                    {
                        DebugMessage(String.Format("BotEngine::UpdateByTimeFrame({0}): Not ready. Last iteration still in progess!", timeFrame));
                        return;
                    }

                    ready[timeFrame].Ready = false;
                }

                List<BotBase> botsList = new List<BotBase>();

                if (timeFrame == TimeFrames.M1)
                {
                    try
                    {
                        foreach (var pair in _botDict.Values)
                        {
                            botsList.AddRange(pair.Values);
                        }
                    }
                    catch (Exception e)
                    {
                        int idebug = 0;
                    }
                }
                else
                {
                    botsList.AddRange(_botDict[timeFrame].Values.Where(bot => bot._botParameters.TimeFrame == timeFrame));
                }
                Task[] loadingTaks = new Task[botsList.Count()];
                int i = 0;
                ConcurrentHashSet<string> processedIndicatorsEngines = new ConcurrentHashSet<string>();
                //ConcurrentHashSet<Profit> profits = new ConcurrentHashSet<Profit>();

                foreach (var bot in botsList)
                {
                    if (bot._botParameters.TimeFrame != timeFrame && timeFrame != TimeFrames.M1)
                    {
                        continue;
                    }
                    loadingTaks[i++] = Task.Run(() =>
                    {
                        if (!_signalsEngineDict.ContainsKey(bot._signalsEngineId))
                        {
                            DebugMessage(String.Format("UpdateByTimeFrame({0}): {1} is not present in _signalsEngineDict", timeFrame.ToString(), bot._signalsEngineId));
                            return;
                        }

                        if (_signalsEngineDict[bot._signalsEngineId].IsMarketClosed())
                        {
                            DebugMessage(String.Format("UpdateByTimeFrame({0}): BotId : {1} specified market is closed.", timeFrame.ToString(), bot._signalsEngineId));
                            return;
                        }
                        lock (_signalsEngineDict[bot._signalsEngineId].Lock)
                        {
                            if (!processedIndicatorsEngines.Contains(bot._signalsEngineId + ":" + timeFrame))
                            {
                                _signalsEngineDict[bot._signalsEngineId].UpdateIndicators(timeFrame);
                                //DebugMessage(String.Format("UpdateByTimeFrame({0}): {1} UPDADE COMPLETE {2}!", timeFrame, bot._signalsEngineId, iteration));
                                processedIndicatorsEngines.Add(bot._signalsEngineId + ":" + timeFrame);
                            }
                        }

                        bot.UpdateSignals(_signalsEngineDict[bot._signalsEngineId]);
                        if (bot._botParameters.TimeFrame == timeFrame)
                        {
                            bot.ProcessTransactions();
                        }
                        else
                        {
                            DateTime now = DateTimeExtensions.Normalize(DateTime.Now, (int) TimeFrames.M1);
                            if (BotLib.BotLib.Backtest)
                            {
                                var lastCandle = _signalsEngineDict[bot._signalsEngineId].GetCurrentCandle(TimeFrames.M1);
                                now = lastCandle.Timestamp;
                            }
                            int left = now.Minute % (int)bot._botParameters.TimeFrame;
                            if (left != 1)
                            {
                                bot.ProcessTransactions(false);
                            }
                        }

                        Profit profit = new Profit();
                        profit.BotId = bot._botParameters.id;
                        profit.ProfitValue = bot._score.AmountGained;
                        profit.DrawBack = bot._score.CurrentProfit < 0 ? bot._score.CurrentProfit : 0;
                        profit.Timestamp = DateTime.Now;
                        _ = profit.StoreAsync();
                        //profits.Add(profit);
                    });

                    if (loadingTaks[i - 1] == null)
                    {
                        int iDebug = 0;
                    }
                }

                loadingTaks = loadingTaks.Where(t => t != null).ToArray();
                Task.WaitAll(loadingTaks);

                foreach (var task in loadingTaks)
                {
                    task.Dispose();
                }

                if (timeFrame == TimeFrames.M1)
                {
                    SaveFailedChanges();
                }

                lock (ready[timeFrame].Lock)
                {
                    ready[timeFrame].Ready = true;
                }

                DebugMessage(String.Format("UpdateByTimeFrame({0}): COMPLETE {1}!", timeFrame, iteration));
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private void SaveFailedChanges() 
        {
            BotDBContext.Execute((botContext) =>
            {
                try
                {
                    var botList = GetBots();
                    botContext.Scores.UpdateRange(botList.Select(bot => bot._score));
                    var name = nameof(Profit);
                    if (botContext.RetryDict.ContainsKey(name))
                    {
                        var profits = (Profit[])botContext.RetryDict[name].ToArray();
                        botContext.Profits.AddRange(profits);
                        botContext.RetryDict[name].Clear();
                    }
                    botContext.SaveChanges();
                }
                catch (Exception)
                {
                    int idebug = 0;
                }
                return 0;
            }, true);
            BrokerDBContext.Execute((brokerContext) =>
            {
                try
                {
                    var name = nameof(Transaction);
                    if (brokerContext.RetryDict.ContainsKey(name))
                    {
                        var transactions = (Transaction[])brokerContext.RetryDict[name].ToArray();
                        brokerContext.Transactions.AddRange(transactions);
                        brokerContext.RetryDict[name].Clear();
                    }
                    name = nameof(Trade);
                    if (brokerContext.RetryDict.ContainsKey(name))
                    {
                        var trades = (Trade[])brokerContext.RetryDict[name].ToArray();
                        brokerContext.Trades.AddRange(trades);
                        brokerContext.RetryDict[name].Clear();
                    }
                    brokerContext.SaveChanges();
                }
                catch (Exception)
                {
                    int idebug = 0;
                }
                return 0;
            }, true);
        }

        public Dictionary<string, List<Candle>> GetAllDataToInitVWAP()
        {
            try
            {
                Dictionary<string, List<Candle>> timeFrameCandlesList = new Dictionary<string, List<Candle>>();
                string symbol = "BTCUSD";
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-PT");
                DateTime today = DateTime.Today;//.ToUniversalTime();

                string[] timeFrames = new string[] { "M1", "M5", "M15", "M30", "H1" };
                foreach (string timeFrame in timeFrames)
                {
                    string date = String.Format("{0} {1}", today.ToShortDateString().Replace("/", "-"), today.ToShortTimeString());
                    string url = String.Format("https://api.hitbtc.com/api/2/public/candles/{0}?period={1}&from={2}&limit={3}", symbol, timeFrame, date, 1000);
                    string result = Request.Get(url);
                    List<Candle> candles = JsonConvert.DeserializeObject<List<Candle>>(result);
                    timeFrameCandlesList.Add(timeFrame, candles);
                }
                return timeFrameCandlesList;
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
            return null;
        }

        private void CreateBot(BotParameters botParameters, BotParametersChanges botParameterschanges, Broker broker) 
        {
            try
            {
                BotBase bot = BotBase.GenerateBotFromParameters(botParameters);

                if (string.IsNullOrEmpty(bot._signalsEngineId))
                {
                    DebugMessage("CreateBot() : BotId._signalsEngineId is empty!");
                    return;
                }
                if (!_botDict.ContainsKey(bot._botParameters.TimeFrame) && Enum.IsDefined(typeof(TimeFrames), bot._botParameters.TimeFrame))
                {
                    _botDict.Add(bot._botParameters.TimeFrame, new Dictionary<string, BotBase>());
                }

                foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                {
                    if (_botDict.ContainsKey(timeFrame) && _botDict[timeFrame].ContainsKey(bot._botParameters.id))
                    {
                        DebugMessage($"CreateBot() : A bot with botId {bot._botParameters.id} already exists in the botDict with TimeFrame {timeFrame}!");
                        return;
                    }
                }

                if (_botDict[bot._botParameters.TimeFrame].ContainsKey(bot._botParameters.id))
                {
                    _botDict[bot._botParameters.TimeFrame][bot._botParameters.id] = bot;
                }
                else
                {
                    _botDict[bot._botParameters.TimeFrame].Add(bot._botParameters.id, bot);
                }

                //SignalsEngine.SignalsEngine _signalsEngine = _signalsEngineDict[bot._signalsEngineId];
                UserBotRelation userBotRelation = new UserBotRelation(bot._botParameters.id, botParameterschanges);

                AccessPoint ap = BrokerDBContext.Execute((brokerContext) => brokerContext.AccessPoints.Find(userBotRelation.AccessPointId));

                if (ap == null)
                {
                    userBotRelation.Store();
                    return;
                }

                //0.-TODO:get balance from api and and generate Equity
                //max value for this user-bot can be set here
                //compare intended value with value from api, if not sufficient throw an error

                Equity equity = Equity.Initialize(broker, ap, bot._botParameters.Market);
                userBotRelation.EquityId = equity.id;
                userBotRelation.Store();
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private void InitBot(BotParameters botParameters) 
        {
            List<BotParameters> botsParametersList = new List<BotParameters>() 
            {
                botParameters
            };
            Dictionary<BrokerDescription, List<string>> activeBrokerMarketStringsDict = GetActiveBrokerMarketStringsDict(botsParametersList);

            List<ActiveMarket> markets = new List<ActiveMarket>();
            int id = 0;
            foreach (var pair in activeBrokerMarketStringsDict)
            {
                Broker.AddBroker(pair.Key, activeBrokerMarketStringsDict);
                foreach (string market in pair.Value)
                {
                    ActiveMarket activeMarket = new ActiveMarket(market, pair.Key);
                    activeMarket.id = id++;
                    if (markets.Contains(activeMarket))
                    {
                        continue;
                    }

                    markets.Add(activeMarket);
                    activeMarket.Store();
                }
            }

            Dictionary<Broker, List<MarketInfo>> activeBrokerMarketsDict = GetActiveBrokerMarketsDict(botsParametersList);
            IndicatorsSharedData.Instance.AddMarkets(activeBrokerMarketsDict);
            //MarketDataClient client

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
                    signalsEngine = new IndicatorsEngine(broker.GetBrokerId(), marketInfo);
                    _signalsEngineDict.Add(signalsEngineId, signalsEngine);
                }
            }

            WaitForFirstCandles(_signalsEngineDict.Values.ToList());
        }

        private static void UpdateUserBots(BotEngine botEngine) 
        {
            try
            {
                List<BotParametersChanges> botParametersChanges = BotDBContext.Execute(contextClient => {
                    return contextClient.BotParametersChanges.ToList();
                });

                foreach (BotParametersChanges botParameters in botParametersChanges)
                {
                    if (botParameters.RecentlyCreated)
                    {
                        BotParameters botparameters = new BotParameters(botParameters);
                        botparameters.Store();
                        botParameters.RecentlyCreated = false;
                        botParameters.Update();

                        Broker broker = Broker.DecideBroker(botparameters.BrokerDescription);
                        if (broker == null)
                        {
                            botEngine.InitBot(botparameters);
                            broker = Broker.DecideBroker(botparameters.BrokerDescription);
                        }

                        MarketDescription marketDescription = new MarketDescription(botparameters.Market, broker.GetMarketType(), broker.GetBrokerType());
                        MarketInfo marketInfo = IndicatorsSharedData.Instance.GetMarketInfo(marketDescription, botparameters.TimeFrame);

                        if (marketInfo == null)
                        {
                            marketInfo = IndicatorsSharedData.Instance.GetMarketInfo(marketDescription, botparameters.TimeFrame);
                        }

                        if (!botParameters.ValidStart(marketInfo) || !botparameters.ValidStart(marketInfo))
                        {
                            DebugMessage(String.Format("BotEngine::UpdateBots() : botParameters {0} invalid start!", botparameters.id));
                            continue;
                        }

                        if (botParameters.BotId != null && botEngine._botDict[botParameters.TimeFrame].ContainsKey(botParameters.BotId))
                        {
                            DebugMessage("BotEngine::UpdateBots() : RecentlyCreated flag used but Bot was already created!");
                            botEngine._botDict[botParameters.TimeFrame][botparameters.id].ChangeParameters(botParameters);
                        }
                        else
                        {
                            botEngine.CreateBot(botparameters, botParameters, broker);
                        }
                    }
                    else if (botParameters.RecentlyModified)
                    {
                        BotParameters botparameters = BotDBContext.Execute(context => {
                            return context.BotsParameters.Find(botParameters.BotId);
                        });

                        if (botparameters == null)
                        {
                            botparameters = new BotParameters(botParameters);
                        }
                        else
                        {
                            botparameters.ChangeParameters(botParameters);
                        }

                        botparameters.Update();
                        botParameters.RecentlyModified = false;
                        botParameters.Update();

                        Broker broker = Broker.DecideBroker(botparameters.BrokerDescription);
                        MarketDescription marketDescription = new MarketDescription(botparameters.Market, broker.GetMarketType(), broker.GetBrokerType());
                        MarketInfo marketInfo = IndicatorsSharedData.Instance.GetMarketInfo(marketDescription, botparameters.TimeFrame);

                        if (!botParameters.ValidStart(marketInfo) || !botparameters.ValidStart(marketInfo))
                        {
                            DebugMessage(String.Format("BotEngine::UpdateBots() : botParameters {0} invalid start!", botparameters.id));
                            continue;
                        }

                        if (botParameters.BotId != null && botEngine._botDict[botParameters.TimeFrame].ContainsKey(botParameters.BotId))
                        {
                            botEngine._botDict[botParameters.TimeFrame][botParameters.BotId].ChangeParameters(botParameters);
                        }
                        else
                        {
                            DebugMessage("UpdateBots() : BotId " + botParameters.BotId + " modify error. The bot does not exist.");
                            DebugMessage("UpdateBots() : BotId " + botParameters.BotId + " modify error. Creating new bot.");
                            botEngine.CreateBot(botparameters, botParameters, broker);
                        }

                    }
                    else if (botParameters.RecentlyDeleted)
                    {
                        BotParameters botparameters = BotDBContext.Execute(context => {
                            return context.BotsParameters.Find(botParameters.id);
                        });

                        if (botparameters == null)
                        {
                            DebugMessage("UpdateBots() : BotId " + botParameters.BotId + " delete error. The bot does not exist in the Database.");
                            return;
                        }

                        botEngine._botDict[botParameters.TimeFrame].Remove(botParameters.BotId);
                        botparameters.Delete();
                        botParameters.RecentlyDeleted = false;
                        botParameters.Update();
                    }
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private static void PrintMeasures(BotEngine botEngine)
        {
            try
            {
                DebugMessage("############################################################");
                //foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                //{
                //    foreach (var bot in botEngine._botDict[timeFrame])
                //    {
                //        int timeFrameNumber = (int)bot.Value._botParameters.TimeFrame;
                //        if (timeFrameNumber == 0)
                //        {
                //            DebugMessage(String.Format("BotEngine::PrintMeasures() : BotId {0}, timeFrame is 0!", bot.Value._botParameters.id));
                //            continue;
                //        }
                //        if (DateTime.UtcNow.Minute % timeFrameNumber == 0)
                //        {
                //            bot.Value.DebugMeasures();
                //        }
                //    }
                //}
                //foreach (var signalsEngine in botEngine._signalsEngineDict.Values)
                //{
                //    if (signalsEngine.IsMarketClosed())
                //    {
                //        continue;
                //    }
                //    signalsEngine.PrintIndicators();

                //    DebugMessage("LastCandlePrice: " + signalsEngine.GetCurrentCandle(TimeFrames.M1).Close);
                //    DebugMessage("LastCandleTimestamp: " + signalsEngine.GetCurrentCandle(TimeFrames.M1).Timestamp);
                //    DebugMessage("TimestampNow: " + DateTime.UtcNow);

                //}

                DebugMessage(String.Format("Print iteration: {0} / {1}", botEngine.iterationPrint++, DateTime.UtcNow));

                DebugMessage("############################################################");
                //_botContext.SaveChanges();

                //_botContext.ChangeTracker.AcceptAllChanges();
            }
            catch (Exception e)
            {
                //_botContext.SaveChanges();
                //_botContext.ChangeTracker.Entries.
                //var entries = _botContext.ChangeTracker.Entries();
                //if (entries != null)
                //{
                //    foreach (var item in entries)
                //    {
                //        if (item.State == EntityState.Unchanged)
                //        {
                //            var state = item.State;
                //            item.State = EntityState.Modified;
                //        }
                //    }
                //    _botContext.SaveChanges();
                //}

                DebugMessage(e);
            }
        }

        protected override void CleanUnusedData() 
        {
            try
            {
                DebugMessage("############################################################");
                DebugMessage("Cleaning Database...");
                BotDBContext.Execute((botContext) => 
                {
                    return BrokerDBContext.Execute((brokerContext) =>
                    {
                        List<Score> scores = botContext.Scores.AsNoTracking().ToList();
                        List<Equity> equities = brokerContext.Equitys.AsNoTracking().ToList();
                        List<BotParameters> botsParameters = botContext.BotsParameters.AsNoTracking().ToList();
                        List<UserBotRelation> userBotRelations = botContext.UserBotRelations.AsNoTracking().ToList();

                        List<Score> scoresToRemove = new List<Score>();
                        List<Equity> equitiesToRemove = new List<Equity>();
                        List<BotParameters> botsParametersToRemove = new List<BotParameters>();
                        List<UserBotRelation> userBotRelationsToRemove = new List<UserBotRelation>();
                        List<Transaction> transactionsToRemove = new List<Transaction>();

                        foreach (Score score in scores)
                        {
                            if (!botContext.BotsParameters.AsNoTracking().Any(m => m.id == score.BotParametersId))
                            {
                                scoresToRemove.Add(score);
                            }
                        }
                        botContext.Scores.RemoveRange(scoresToRemove);

                        foreach (Equity equity in equities)
                        {
                            if (!botContext.UserBotRelations.AsNoTracking().Any(m => m.EquityId == equity.id))
                            {
                                equitiesToRemove.Add(equity);
                            }
                        }
                        brokerContext.Equitys.RemoveRange(equitiesToRemove);

                        //foreach (Transaction transaction in transactions)
                        //{
                        //    if (!_botContext.BotsParameters.Any(m => m.id == transaction.BotId))
                        //    {
                        //        transactionsToRemove.Add(transaction);
                        //    }
                        //}
                        //_brokerContext.Transactions.RemoveRange(transactionsToRemove);

                        foreach (UserBotRelation userBotRelation in userBotRelations)
                        {
                            if (string.IsNullOrEmpty(userBotRelation.BotId) ||
                                (userBotRelation.IsVirtual == false && string.IsNullOrEmpty(userBotRelation.AccessPointId)) ||
                                botContext.BotsParameters.Find(userBotRelation.BotId) == null)
                            {
                                userBotRelationsToRemove.Add(userBotRelation);
                            }
                        }
                        botContext.UserBotRelations.RemoveRange(userBotRelationsToRemove);

                        return botContext.SaveChanges() + brokerContext.SaveChanges();
                    }, true);
                }, true);
                DebugMessage("Clean done.");
                DebugMessage("############################################################");


            }
            catch (Exception e)
            {
                DebugMessage(e.StackTrace);
            }
        }

        private void ResetDatabase()
        {
            try
            {
                //List<Transaction> transactions = _botContext.Transactions.Where(t => t.BotId != 0).ToList();
                //_botContext.Transactions.RemoveRange(transactions);
                //List<Score> scores = _botContext.Scores.Where(t => t.BotParametersId != 0).ToList();
                //foreach (var score in scores)
                //{
                //    score.reset();
                //}
                //_botContext.Scores.UpdateRange(scores);
                //List<Equity> equities = _botContext.Equitys.ToList();
                //foreach (var equity in equities)
                //{
                //    equity.Amount = 115;
                //    equity.EquityValue = 115;
                //}
                //_botContext.Equitys.UpdateRange(equities);
                //_botContext.SaveChanges();
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }
    }
}

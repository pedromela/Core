using BotEngine.Bot;
using BotLib.Models;
using BrokerLib.Brokers;
using BrokerLib.Market;
using BrokerLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SignalsEngine.Indicators;
using SignalsEngine.Strategys;
using SignalsEngine.Strategys.ExampleStrategys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace BotEngine
{
    public class TradingEngine : Engine
    {
        protected IConfigurationRoot Configuration { get; set; }

        protected readonly int _initNEmpty = 50;
        protected Dictionary<TimeFrames, Dictionary<string, BotBase>> _botDict = null;
        protected Dictionary<string, IndicatorsEngine> _signalsEngineDict = null;
        protected GeneticAlgorithm _geneticAlgorithm;
        protected BotRanking _botRanking = null;

        public TradingEngine()
        {
            _botDict = new Dictionary<TimeFrames, Dictionary<string, BotBase>>();
            _signalsEngineDict = new Dictionary<string, IndicatorsEngine>();
            _botRanking = new BotRanking();
        }

        public override IObservable<int> Init()
        {
            try
            {
                return Observable.Create<int>((o) => {
                    o.OnNext(0);
                    BotDBContext.InitProviders();
                    BrokerDBContext.InitProviders();
                    BrokerDBContext.Execute((brokerContext) => 
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
                            return brokerContext.SaveChanges();
                        }
                        return 0;
                    }, true);

                    CleanUnusedData();

                    o.OnNext(10);

                    List<BotParameters> botsParametersList;
                    List<BotParameters> botsParametersListNotValid;

                    botsParametersList = BotDBContext.Execute(botContext => {
                        return botContext.GetBotsFromDB();
                    });
                    botsParametersListNotValid = new List<BotParameters>();

                    foreach (var botParameters1 in botsParametersList)
                    {
                        if (!botParameters1.ValidStart2())
                        {
                            DebugMessage(String.Format("TradingEngine::Init() : Bot {1}/{0} is not valid!", botParameters1.id, botParameters1.BotName));

                            botsParametersListNotValid.Add(botParameters1);
                        }
                    }
                    foreach (var botParameters2 in botsParametersListNotValid)
                    {
                        botsParametersList.Remove(botParameters2);
                        botParameters2.Delete();
                    }

                    if (botsParametersList.Count == 0)
                    {
                        ClearDatabase();
                        InitExempleStrategies();
                        GenerateBotsForeachMarketTimeFrame();

                        botsParametersList = BotDBContext.Execute(botContext => {
                            return botContext.GetBotsFromDB();
                        });
                    }
                    else
                    {
                        //BotParameters botParameters = _botRanking.KillWorstBot();
                        //if (botParameters != null)
                        //{
                        //    botsParametersList.Remove(botsParametersList.Find(bot => bot.id == botParameters.id));
                        //}
                        _botRanking.CalculateBestRankings();
                    }

                    o.OnNext(20);

                    for (int i = 0; i < botsParametersList.Count; i++)
                    {
                        botsParametersList[i].BrokerDescription = new BrokerDescription(botsParametersList[i].BrokerId, botsParametersList[i].BrokerType);
                    }

                    o.OnNext(30);

                    Dictionary<BrokerDescription, List<string>> activeBrokerMarketStringsDict = GetActiveBrokerMarketStringsDict(botsParametersList);
                    Broker.InitBrokers(activeBrokerMarketStringsDict);
                    BrokerDBContext.Execute((brokerContext) => 
                    {
                        if (brokerContext.GetActiveMarketsCountFromDB() > 0)
                        {
                            brokerContext.DeleteActiveMarketsFromDB();
                        }
                        return 0;
                    }, true);


                    o.OnNext(40);

                    Dictionary<Broker, List<MarketInfo>> activeBrokerMarketsDict = GetActiveBrokerMarketsDict(botsParametersList);
                    IndicatorsSharedData.InitInstance(activeBrokerMarketsDict);
                    //MarketDataClient client

                    o.OnNext(50);

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

                    o.OnNext(60);

                    List<ActiveMarket> markets = new List<ActiveMarket>();
                    int id = 0;
                    foreach (var pair in activeBrokerMarketsDict)
                    {
                        foreach (var market in pair.Key.GetAvailableMarketInfos())
                        {
                            ActiveMarket activeMarket = new ActiveMarket(market.GetMarketUnderscore(), pair.Key.GetBrokerDescription());
                            activeMarket.id = id++;
                            if (markets.Contains(activeMarket))
                            {
                                continue;
                            }

                            markets.Add(activeMarket);
                            activeMarket.Store();
                        }
                    }

                    o.OnNext(80);

                    WaitForFirstCandles(_signalsEngineDict.Values.ToList());

                    o.OnNext(80);

                    if (botsParametersList.Count > 0)
                    {
                        List<Transaction> activeTransactions = null;
                        List<Trade> activeTrades = null;

                        (activeTransactions, activeTrades) = BrokerDBContext.Execute(brokerContext =>
                        {
                            var activeTransactions = brokerContext.Transactions.AsNoTracking().Where(m => m.Type == TransactionType.buy || m.Type == TransactionType.sell).ToList();
                            var activeTrades = brokerContext.Trades.AsNoTracking().Where(m => m.Type == TransactionType.buy || m.Type == TransactionType.sell).ToList();
                            return (activeTransactions, activeTrades);
                        });

                        Task[] loadingTaks = new Task[botsParametersList.Count()];
                        int i = 0;

                        foreach (BotParameters botParameters in botsParametersList)
                        {
                            loadingTaks[i++] = Task.Run(() =>
                            {
                                Broker broker = Broker.DecideBroker(botParameters.BrokerDescription);
                                MarketDescription marketDescription = new MarketDescription(botParameters.Market, broker.GetMarketType(), broker.GetBrokerType());
                                MarketInfo marketInfo = IndicatorsSharedData.Instance.GetMarketInfo(marketDescription, botParameters.TimeFrame);
                                if (marketInfo == null || !botParameters.ValidStart(marketInfo))
                                {
                                    DebugMessage("TradingEngine::Init() : bot " + botParameters.id + " start validation failed.");
                                    return;
                                }

                                BotBase bot = BotBase.GenerateBotFromParameters(botParameters);
                                var botTransactions = activeTransactions.Where(m => m.BotId == bot._botParameters.id);
                                bot.InitTransactionsToBeProcessed(botTransactions);
                                var botTransactionIds = botTransactions.Select(m => m.id);
                                var botTrades = activeTrades.Where(m => botTransactionIds.Contains(m.TransactionId)).ToList();
                                bot.InitTradesToBeProcessed(botTrades);
                                lock (_botDict)
                                {
                                    if (!_botDict.ContainsKey(botParameters.TimeFrame))
                                    {
                                        if (Enum.IsDefined(typeof(TimeFrames), bot._botParameters.TimeFrame))
                                        {
                                            _botDict.Add(botParameters.TimeFrame, new Dictionary<string, BotBase>());
                                        }
                                        else
                                        {
                                            DebugMessage("TradingEngine::Init() : bot " + botParameters.id + " TimeFrame is invalid.");
                                        }
                                    }
                                    _botDict[botParameters.TimeFrame].Add(botParameters.id, bot);
                                }

                                DebugMessage(String.Format("TradingEngine::Init() : Adding bot {1}/{0}", botParameters.id, botParameters.BotName));
                            });
                        }

                        Task.WaitAll(loadingTaks);

                        foreach (var task in loadingTaks)
                        {
                            task.Dispose();
                        }
                    }

                    o.OnNext(90);


                    _geneticAlgorithm = BotDBContext.Execute(botContext => {
                        return new GeneticAlgorithm(botContext.BotsParameters.ToList().Count, this);
                    });

                    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    Configuration = builder.Build();

                    o.OnNext(100);
                    o.OnCompleted();

                    return () => { };
                });

            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
            return Observable.Empty<int>();
        }

        protected virtual void InitExempleStrategies() 
        {
            try
            {
                List<ConditionStrategy> conditionStrategies = ExampleStrategy.GetAllExampleStrategies();
                foreach (ConditionStrategy strategy in conditionStrategies)
                {
                    strategy.Save();
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        } 

        protected virtual void GenerateBotsForeachMarketTimeFrame()
        {
            try
            {
                List<Broker> brokers = Broker.GetAllBrokers();
                foreach (var broker in brokers)
                {
                    List<string> markets = broker.GetMarkets();
                    foreach (var market in markets)
                    {
                        foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                MarketInfo marketInfo = new MarketInfo(market, broker, true);//IndicatorsSharedData.Instance.GetCandleData(marketDescription, timeFrame).GetMarketInfo();//MarketInfo.Construct(brokerId, market, true);
                                string conditionStrategyId = ConditionStrategyData.GetRandomStrategyId();
                                Bot.Bot.GenerateRandomBotParameters(timeFrame, market, broker.GetBrokerDescription(), conditionStrategyId);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        protected virtual void WaitForFirstCandles(List<IndicatorsEngine> signalEngines)
        {
            try
            {
                Task[] loadingTasks = new Task[signalEngines.Count];
                int i = 0;
                foreach (var signalsEngine in signalEngines)
                {
                    loadingTasks[i++] = Task.Run(() =>
                    {
                        Waiter waiter = new Waiter(1);

                        signalsEngine.Start(waiter);
                    });
                }
                Task.WaitAll(loadingTasks);

                DebugMessage("WaitForFirstCandles done!");

            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        protected virtual void ClearDatabase()
        {
            try
            {
                foreach (var provider in BrokerDBContext.providers)
                {
                    using (BrokerDBContext brokerContext = (BrokerDBContext) provider.GetDBContext())
                    {
                        brokerContext.Transactions.RemoveRange(brokerContext.Transactions);
                        brokerContext.Trades.RemoveRange(brokerContext.Trades);
                        brokerContext.Equitys.RemoveRange(brokerContext.Equitys);
                        //brokerContext.Points.RemoveRange(brokerContext.Points);
                        //brokerContext.Candles.RemoveRange(brokerContext.Candles);
                        brokerContext.SaveChanges();
                    }
                }

                foreach (var provider in BotDBContext.providers)
                {
                    using (BotDBContext botContext = (BotDBContext)provider.GetDBContext())
                    {
                        botContext.Scores.RemoveRange(botContext.Scores);
                        botContext.BotsParameters.RemoveRange(botContext.BotsParameters);
                        botContext.BotParametersRankings.RemoveRange(botContext.BotParametersRankings);
                        botContext.BotParametersChanges.RemoveRange(botContext.BotParametersChanges);
                        botContext.ConditionStrategiesData.RemoveRange(botContext.ConditionStrategiesData);
                        botContext.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        public virtual void UpdateCycle()
        {
            try
            {

            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        protected virtual Dictionary<BrokerDescription, List<string>> GetActiveBrokerMarketStringsDict(List<BotParameters> botParametersList)
        {
            try
            {
                Dictionary<BrokerDescription, List<string>> activeBrokerMarketStringsDict = new Dictionary<BrokerDescription, List<string>>();
                foreach (var botParameters in botParametersList)
                {
                    if (activeBrokerMarketStringsDict.ContainsKey(botParameters.BrokerDescription))
                    {
                        if (!activeBrokerMarketStringsDict[botParameters.BrokerDescription].Contains(botParameters.Market))
                        {
                            activeBrokerMarketStringsDict[botParameters.BrokerDescription].Add(botParameters.Market);
                        }
                    }
                    else
                    {
                        var list = new List<string>
                        {
                            botParameters.Market
                        };
                        activeBrokerMarketStringsDict.Add(botParameters.BrokerDescription, list);
                    }
                }
                return activeBrokerMarketStringsDict;
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
            return null;
        }

        protected virtual Dictionary<Broker, List<MarketInfo>> GetActiveBrokerMarketsDict(List<BotParameters> botParametersList)
        {
            try
            {
                List<Brokers> activeBrokers = new List<Brokers>();
                Dictionary<Broker, List<MarketInfo>> activeBrokerMarketsDict = new Dictionary<Broker, List<MarketInfo>>();
                foreach (var botParameters in botParametersList)
                {
                    Broker broker = Broker.DecideBroker(botParameters.BrokerDescription);
                    if (activeBrokerMarketsDict.ContainsKey(broker) || activeBrokers.Contains(broker.GetBrokerId()))
                    {
                        continue;
                    }
                    activeBrokers.Add(broker.GetBrokerId());
                    activeBrokerMarketsDict.Add(broker, broker.GetMarketInfos());
                }
                return activeBrokerMarketsDict;
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
            return null;
        }

        protected virtual void CleanUnusedData()
        {
            try
            {

            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        public List<BotBase> GetBots() 
        {
            try
            {
                var botList = new List<BotBase>();
                foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                {
                    if (_botDict.ContainsKey(timeFrame))
                    {
                        botList.AddRange(_botDict[timeFrame].Values);
                    }
                }
                return botList;
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
            return null;
        }
    }
}
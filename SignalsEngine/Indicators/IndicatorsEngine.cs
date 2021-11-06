using BrokerLib.Market;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Indicators
{
    public class IndicatorsEngine : Engine
    {
        private readonly Brokers _brokerId;

        private bool _backtest = false;
        private DateTime _fromDate;
        private DateTime _toDate;
        private TimeFrames _timeFrame;
        private MarketInfo _marketInfo = null;
        private bool _marketInfoClosed = false;

        public readonly object Lock = new object();

        public bool FirstCandle = true;


        public IndicatorsEngine(Brokers broker, MarketInfo marketInfo)
        {
            _brokerId = broker;
            _marketInfo = marketInfo;
        }

        public IndicatorsEngine(Brokers broker, MarketInfo marketInfo, DateTime fromDate, DateTime toDate, TimeFrames timeFrame) 
        {
            _brokerId = broker;
            _marketInfo = marketInfo;
            _backtest = true;
            _fromDate = fromDate;
            _toDate = toDate;
            _timeFrame = timeFrame;
        }

        public Brokers GetBrokerId() 
        {
            return _brokerId;
        }

        public MarketInfo GetMarketInfo() 
        {
            return _marketInfo;
        }
        public bool IsMarketClosed()
        {
            try
            {
                return _marketInfoClosed;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return true;
        }

        public void Start(Waiter autoEvent)
        {
            try
            {
                StartTimeSeries();

                if (_marketInfoClosed)
                {
                    return;
                }

                StartIndicators();

                if (_backtest)
                {
                    autoEvent.Set();
                    return;
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public Candle GetCurrentCandle(TimeFrames timeFrame, string line = "middle")
        {
            return GetCurrentCandleNode(timeFrame).Value[line];
        }

        public LinkedListNode<Dictionary<string, Candle>> GetCurrentCandleNode(TimeFrames timeFrame)
        {
            try
            {
                if (!IndicatorsSharedData.Instance.ContainsCandleData(_marketInfo.GetMarketDescription(), timeFrame))
                {
                    DebugMessage("GetCurrentCandleNode(): timeSeries not present ->" + timeFrame);
                    return null;
                }
                LinkedListNode<Dictionary<string, Candle>> candleNode;
                if (_backtest)
                {
                    candleNode = IndicatorsSharedData.Instance.GetCandleData(_marketInfo.GetMarketDescription(), timeFrame).GetSelectedTimeSeries().GetLastValueNode();
                }
                else
                {
                    candleNode = IndicatorsSharedData.Instance.GetCandleData(_marketInfo.GetMarketDescription(), timeFrame).GetCurrentCandleNode();
                }

                return candleNode;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public bool CandlesEnd(TimeFrames timeFrame) 
        {
            try
            {
                if (!IndicatorsSharedData.Instance.ContainsCandleData(_marketInfo.GetMarketDescription(), timeFrame))
                {
                    DebugMessage("CandlesEnd(): timeSeries not present ->" + timeFrame);
                    return true;
                }

                LinkedListNode<Dictionary<string, Candle>> candleNode   = GetCurrentCandleNode(timeFrame);
                PriceData candleData = IndicatorsSharedData.Instance.GetCandleData(_marketInfo.GetMarketDescription(), timeFrame);
                LinkedListNode<Dictionary<string, Candle>> lastNode     = candleData.GetLastValueNode();
                SignalsEngine.DebugMessage(String.Format("Backtesting {0}", candleNode.Value["middle"].Timestamp));
                if (candleNode.Value["middle"].IsEqual(lastNode.Value["middle"]))
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return true;
        }

        public List<Candle> GetLastCandles(TimeFrames timeFrame) 
        {
            try
            {
                List<Candle> candlesListShared = IndicatorsSharedData.Instance.GetLastCandles(_marketInfo.GetMarketDescription(), timeFrame);
                return candlesListShared;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public LinkedListNode<Dictionary<string, Candle>> GetNextCandleNode(TimeFrames timeFrame)
        {
            try
            {
                if (!IndicatorsSharedData.Instance.ContainsCandleData(_marketInfo.GetMarketDescription(), timeFrame))
                {
                    DebugMessage("GetNextCandleNode(): timeSeries not present ->" + timeFrame);
                    return null;
                }
                LinkedListNode<Dictionary<string, Candle>> candleNode = IndicatorsSharedData.Instance.GetCandleData(_marketInfo.GetMarketDescription(), timeFrame).GetNextCandleNode();
                return candleNode;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }
        public Indicator GetIndicator(string name, TimeFrames timeFrame) 
        {
            return IndicatorsSharedData.Instance.GetIndicator(_marketInfo.GetMarketDescription(), name, timeFrame);
        }

        public void ProcessAtDate(TimeFrames timeFrame, DateTime date) 
        {
            try
            {
                date = DateTimeExtensions.Normalize(date, (int)timeFrame);
                StartTimeSeries(timeFrame, date);
                StartIndicators(timeFrame);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void StartTimeSeries(TimeFrames timeFrame, DateTime date)
        {
            try
            {
                List<Candle> candles = null;
                List<Candle> candlesToInitDay = null;
                PriceData candleData = IndicatorsSharedData.Instance.GetCandleData(_marketInfo.GetMarketDescription(), timeFrame);//new Candles(_brokerId, _marketInfo, timeFrame);
                
                DebugMessage(String.Format("Starting TimeSeries at {0} / {1} / {2}...", DateTime.UtcNow, _marketInfo.GetMarket(), timeFrame.ToString()));

                if (_backtest)
                {
                    candles = candleData.GetHistoricalData(date, _toDate.AddMinutes(-((int) timeFrame)), timeFrame, 200);
                    //candlesToInitDay = candleData.GetHistoricalDataToInitDayByTimeFrame(timeFrame);
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        candlesToInitDay = candleData.GetDataToInitDayByTimeFrame();
                        if (candlesToInitDay != null && candlesToInitDay.Count > 0)
                        {
                            break;
                        }
                    }
                    candles = candleData.Init(timeFrame, 200);

                    if ((candlesToInitDay == null || candlesToInitDay.Count == 0) && timeFrame == TimeFrames.M1)
                    {
                        DebugMessage("Market closed : " + DecideSignalsEngineId(candleData.GetBrokerName(), candleData.GetMarket()));
                        _marketInfoClosed = true;
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void StartTimeSeries()
        {
            try
            {
                if (_backtest)
                {
                    StartTimeSeries(_timeFrame, _fromDate);
                }
                else
                {
                    var timeFrames = Enum.GetValues(typeof(TimeFrames));
                    Task[] loadingTaks = new Task[timeFrames.Length];
                    int i = 0;
                    HashSet<string> processedIndicatorsEngines = new HashSet<string>();

                    foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                    {
                        loadingTaks[i++] = Task.Run(() =>
                        {
                            StartTimeSeries(timeFrame, _fromDate);
                        });
                    }

                    Task.WaitAll(loadingTaks);

                    foreach (var task in loadingTaks)
                    {
                        task.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }
        public void AddIndicators(TimeFrames timeFrame, Dictionary<string, Indicator> indicators) 
        {
            try
            {
                IndicatorsSharedData.Instance.AddIndicators(_marketInfo.GetMarketDescription(), timeFrame, indicators);
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        public void StartIndicators(TimeFrames timeFrame)
        {
            try
            {
                if (!IndicatorsSharedData.Instance.ContainsCandleData(_marketInfo.GetMarketDescription(), timeFrame))
                {
                    return;
                }

                Dictionary<string, Indicator> indicators = GetInitializationIndicators(timeFrame, _marketInfo);
                Dictionary<string, Indicator> specialIndicators = new Dictionary<string, Indicator> ();
                PriceData candleData = IndicatorsSharedData.Instance.GetCandleData(_marketInfo.GetMarketDescription(), timeFrame);

                foreach (var i in indicators)
                {
                    if (_backtest)
                    {
                        i.Value.Store = false;
                        //if (i.Value is IndicatorDayData)
                        //{
                        //    i.Value.Init(candleData.GetTimeSeriesDay());
                        //}
                        //else
                        //{
                            if (i.Value.InputName.Equals(PriceData.NAME))
                            {
                                i.Value.Init(candleData.SelectTimeSeries(0, 200));
                            }
                            else
                            {
                                specialIndicators.Add(i.Key, i.Value);
                            }
                        //}
                    }
                    else
                    {
                        bool store = false;
                        if (i.Value.Store)
                        {
                            i.Value.Store = false;
                            store = true;
                        }
                        if (i.Value is IndicatorDayData)
                        {
                            i.Value.Init(candleData.GetTimeSeriesDay());
                        }
                        else
                        {
                            if (i.Value.InputName.Equals(PriceData.NAME))
                            {
                                i.Value.Init(candleData.GetTimeSeries());
                            }
                            else
                            {
                                specialIndicators.Add(i.Key, i.Value);
                            }
                        }
                        if (store)
                        {
                            i.Value.Store = true;
                        }
                    }
                }

                foreach (var pair in specialIndicators)
                {
                    indicators.Remove(pair.Key);
                }

                AddIndicators(timeFrame, indicators);

                foreach (var indicator in specialIndicators)
                {
                    bool store = false;
                    Indicator i = IndicatorsSharedData.Instance.GetIndicator(_marketInfo.GetMarketDescription(), indicator.Value.InputName, timeFrame);
                    if (i.Store)
                    {
                        i.Store = false;
                        store = true;
                    }
                    i.Store = !i.Store;
                    indicator.Value.Init(i);
                    if (store)
                    {
                        i.Store = true;
                    }
                }

                AddIndicators(timeFrame, specialIndicators);

            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        public void StartIndicators() 
        {
            try
            {
                DebugMessage("Starting Indicators for Broker " + _brokerId + " and market " + _marketInfo.GetMarket() + "...");
                DebugMessage("Waiting for first M1 candle...");

                if (_backtest)
                {
                    StartIndicators(_timeFrame);
                }
                else
                {
                    var timeFrames = Enum.GetValues(typeof(TimeFrames));
                    Task[] loadingTaks = new Task[timeFrames.Length];
                    int i = 0;
                    HashSet<string> processedIndicatorsEngines = new HashSet<string>();

                    foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                    {
                        loadingTaks[i++] = Task.Run(() =>
                        {
                            StartIndicators(timeFrame);
                        });
                    }

                    Task.WaitAll(loadingTaks);

                    foreach (var task in loadingTaks)
                    {
                        task.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        public void IncludeNextCandleInSelection(TimeFrames timeFrame) 
        {
            try
            {
                IndicatorsSharedData.Instance.GetCandleData(_marketInfo.GetMarketDescription(), timeFrame).IncludeNextCandleInSelection();
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        public void UpdateIndicators(TimeFrames timeFrame) 
        {
            try
            {
                if (IndicatorsSharedData.Instance.ContainsCandleData(_marketInfo.GetMarketDescription(), timeFrame) && IndicatorsSharedData.Instance.ContainsIndicators(_marketInfo.GetMarketDescription(), timeFrame))
                {
                    if (timeFrame == TimeFrames.M1)
                    {
                        DateTime date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, 0);
                        if (!_backtest && date.Equals(DateTime.Today.ToUniversalTime()))
                        {
                            ResetDailyIndicators();
                        }
                    }
                    Price timeSeries = null;
                    if (_backtest)
                    {
                        IncludeNextCandleInSelection(timeFrame);
                        timeSeries = IndicatorsSharedData.Instance.GetCandleData(_marketInfo.GetMarketDescription(), timeFrame).GetSelectedTimeSeries();
                    }
                    else
                    {
                        timeSeries = IndicatorsSharedData.Instance.GetCandleData(_marketInfo.GetMarketDescription(), timeFrame);
                    }

                    int retries = 3;
                    bool success = false;
                    for (int retry = 0; retry < retries; retry++)
                    {
                        if (timeSeries.CalculateNext(null))
                        {
                            success = true;
                            break;
                        }
                        else
                        {
                            DebugMessage(String.Format("UpdateIndicators({0}, {1}): Getting last candle failed! Retry {2}", _brokerId.ToString(), _marketInfo.GetMarket(), retry + 1));
                            Thread.Sleep(1000);
                        }
                    }

                    if (success)
                    {
                        Dictionary<string, Indicator> indicators = IndicatorsSharedData.Instance.GetIndicators(_marketInfo.GetMarketDescription(), timeFrame);

                        Task[] loadingTaks = new Task[indicators.Count];
                        int i = 0;

                        foreach (var pair in indicators)
                        {
                            loadingTaks[i++] = Task.Run(() =>
                            {
                                if (_backtest)
                                {
                                    pair.Value.Store = false;
                                }
                                if (pair.Value.ShortName.Equals(PriceData.NAME))
                                {
                                    return; ;
                                    //i.Value.CalculateNext(null);
                                }
                                if (pair.Value.InputName.Equals(PriceData.NAME))
                                {
                                    pair.Value.CalculateNext(timeSeries);
                                }
                                else
                                {
                                    Indicator indicator = IndicatorsSharedData.Instance.GetIndicator(_marketInfo.GetMarketDescription(), pair.Value.InputName, timeFrame);
                                    pair.Value.CalculateNext(indicator);
                                }
                            });
                        }

                        Task.WaitAll(loadingTaks);

                        foreach (var task in loadingTaks)
                        {
                            task.Dispose();
                        }

                        BrokerBulkStore.Instance.SaveChanges();
                    }
                }
                else
                {
                    DebugMessage("UpdateIndicators(" + _brokerId.ToString() + ", " + _marketInfo.GetMarket() + "): something is missing!");
                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        private void ResetDailyIndicators() 
        {
            try
            {
                DebugMessage("Reseting daily indicators!");

                foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                {
                    if (!IndicatorsSharedData.Instance.ContainsCandleData(_marketInfo.GetMarketDescription(), timeFrame))
                    {
                        continue;
                    }
                    PriceData candleData = IndicatorsSharedData.Instance.GetCandleData(_marketInfo.GetMarketDescription(), timeFrame);
                    Dictionary<string, Indicator> indicators = IndicatorsSharedData.Instance.GetIndicators(_marketInfo.GetMarketDescription(), timeFrame);
                    Price timeSeriesDay = candleData.GetTimeSeriesDay(true);
                    foreach (var indicator in indicators.Values)
                    {
                        if (indicator is IndicatorDayData)
                        {
                            indicator.Init(timeSeriesDay);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        public void PrintIndicators() 
        {
            try
            {
                foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                {
                    DebugMessage("############################################################");
                    DebugMessage("Broker: " + _brokerId.ToString());
                    DebugMessage("TimeFrame: " + timeFrame.ToString());
                    DebugMessage("Market: " + _marketInfo.GetMarket());
                    MarketDescription marketDescription = _marketInfo.GetMarketDescription();
                    if (marketDescription.Market == "ETHUSD")
                    {
                        var indicators = IndicatorsSharedData.Instance.GetIndicators(marketDescription, timeFrame);
                        foreach (var indicator in indicators)
                        {
                            var lines = indicator.Value.GetLines();
                            foreach (string line in lines.Keys)
                            {
                                DebugMessage(indicator.Value.ShortName + "_" + line + ": " + indicator.Value.GetLastClose(line));
                            }
                        }
                    }
                    Candle lastCandle = GetCurrentCandle(timeFrame);
                    DebugMessage("LastCandleTimestamp: " + lastCandle.Timestamp);
                    DebugMessage("############################################################");

                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }
        public void ProcessIndicatorsAtDate(TimeFrames timeframe, DateTime date)
        {
            try
            {
                ProcessAtDate(timeframe, date);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }


        ///////////////////////////////////////////////////////////////////
        //////////////////////// STATIC FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////
        public static string DecideSignalsEngineId(Brokers BrokerId, string Market)
        {
            try
            {
                //if (BrokerId == Brokers.HitBTCMargin)
                //{
                //    BrokerId = Brokers.HitBTC;
                //}
                if (Market.Contains("_"))
                {
                    Market = Market.Replace("_", "");
                }
                return BrokerId.ToString() + ":" + Market;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }
        public static Dictionary<string, Indicator> GetIndicatorsForAPI(TimeFrames timeFrame)
        {
            try
            {
                Dictionary<string, Indicator> indicators = new Dictionary<string, Indicator>();


                Indicator vwap = new VWAP(3, timeFrame, null);
                indicators.Add(vwap.ShortName, vwap);

                Indicator sma = new SMA(12, timeFrame, null);
                indicators.Add(sma.ShortName, sma);

                Indicator smma = new SMMA(12, timeFrame, null);
                indicators.Add(smma.ShortName, sma);

                Indicator ema = new EMA(12, timeFrame, null);
                indicators.Add(ema.ShortName, ema);

                Indicator macd = new MACD(9, 12, 26, timeFrame, null);
                indicators.Add(macd.ShortName, macd);

                Indicator bears = new BearsPower(13, timeFrame, null);
                indicators.Add(bears.ShortName, bears);

                Indicator bulls = new BullsPower(13, timeFrame, null);
                indicators.Add(bulls.ShortName, bulls);

                Indicator bollingerbands20 = new BB(20, 2, timeFrame, null);
                indicators.Add(bollingerbands20.ShortName, bollingerbands20);

                Indicator avgtruerange14 = new ATR(14, timeFrame, null);
                indicators.Add(avgtruerange14.ShortName, avgtruerange14);

                Indicator bullishbearish = new BullishBearish(200, timeFrame, null);
                indicators.Add(bullishbearish.ShortName, bullishbearish);

                Indicator momentum12 = new Momentum(12, timeFrame, null);
                indicators.Add(momentum12.ShortName, momentum12);

                Indicator psar200 = new ParabolicSAR(200, 0.02f, 0.02f, 0.2f, timeFrame, null);
                psar200.Store = true;
                indicators.Add(psar200.ShortName, psar200);

                Indicator adx14 = new ADX(14, timeFrame, null);
                adx14.Store = true;
                indicators.Add(adx14.ShortName, adx14);


                return indicators;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public static Dictionary<string, Indicator> GetInitializationIndicators(TimeFrames timeFrame, MarketInfo marketInfo) 
        {
            try
            {
                Dictionary<string, Indicator> indicators = new Dictionary<string, Indicator>();


                Indicator vwap = new VWAP(3, timeFrame, marketInfo);
                vwap.Store = true;
                indicators.Add(vwap.ShortName, vwap);

                Indicator sma = new SMA(12, timeFrame, marketInfo);
                sma.Store = true;
                indicators.Add(sma.ShortName, sma);
                sma = new SMA(26, timeFrame, marketInfo);
                sma.Store = true;
                indicators.Add(sma.ShortName, sma);
                sma = new SMA(20, timeFrame, marketInfo);
                sma.Store = true;
                indicators.Add(sma.ShortName, sma);
                sma = new SMA(200, timeFrame, marketInfo);
                sma.Store = true;
                indicators.Add(sma.ShortName, sma);

                Indicator smma = new SMMA(12, timeFrame, marketInfo);
                smma.Store = true;
                indicators.Add(smma.ShortName, smma);
                smma = new SMMA(5, timeFrame, marketInfo);
                smma.Store = true;
                indicators.Add(smma.ShortName, smma);
                smma = new SMMA(8, timeFrame, marketInfo);
                smma.Store = true;
                indicators.Add(smma.ShortName, smma);
                smma = new SMMA(26, timeFrame, marketInfo);
                smma.Store = true;
                indicators.Add(smma.ShortName, smma);
                smma = new SMMA(20, timeFrame, marketInfo);
                smma.Store = true;
                indicators.Add(smma.ShortName, smma);
                smma = new SMMA(200, timeFrame, marketInfo);
                smma.Store = true;
                indicators.Add(smma.ShortName, smma);

                Indicator ema = new EMA(12, timeFrame, marketInfo);
                ema.Store = true;
                indicators.Add(ema.ShortName, ema);
                ema = new EMA(26, timeFrame, marketInfo);
                ema.Store = true;
                indicators.Add(ema.ShortName, ema);
                ema = new EMA(20, timeFrame, marketInfo);
                ema.Store = true;
                indicators.Add(ema.ShortName, ema);
                ema = new EMA(200, timeFrame, marketInfo);
                ema.Store = true;
                indicators.Add(ema.ShortName, ema);

                Indicator macd = new MACD(9, 12, 26, timeFrame, marketInfo);
                macd.Store = true;
                indicators.Add(macd.ShortName, macd);

                Indicator macd2 = new MACD(26, 26, 72, timeFrame, marketInfo);
                macd2.Store = true;
                indicators.Add(macd2.ShortName, macd2);

                Indicator bears = new BearsPower(13, timeFrame, marketInfo);
                bears.Store = true;
                indicators.Add(bears.ShortName, bears);

                Indicator bulls = new BullsPower(13, timeFrame, marketInfo);
                bulls.Store = true;
                indicators.Add(bulls.ShortName, bulls);

                Indicator bollingerbands20 = new BB(20, 2, timeFrame, marketInfo);
                bollingerbands20.Store = true;
                indicators.Add(bollingerbands20.ShortName, bollingerbands20);

                Indicator bollingerbands200 = new BB(200, 2, timeFrame, marketInfo);
                bollingerbands200.Store = true;
                indicators.Add(bollingerbands200.ShortName, bollingerbands200);

                Indicator bollingerbands100 = new BB(100, 3, timeFrame, marketInfo);
                bollingerbands100.Store = true;
                indicators.Add(bollingerbands100.ShortName, bollingerbands100);

                Indicator avgtruerange14 = new ATR(14, timeFrame, marketInfo);
                avgtruerange14.Store = true;
                indicators.Add(avgtruerange14.ShortName, avgtruerange14);

                Indicator bullishbearish = new BullishBearish(200, timeFrame, marketInfo);
                bullishbearish.Store = true;
                indicators.Add(bullishbearish.ShortName, bullishbearish);

                Indicator momentum12 = new Momentum(12, timeFrame, marketInfo);
                momentum12.Store = true;
                indicators.Add(momentum12.ShortName, momentum12);

                Indicator momentum120 = new Momentum(120, timeFrame, marketInfo);
                momentum120.Store = true;
                indicators.Add(momentum120.ShortName, momentum120);

                Indicator mommom1 = new Momentum(1, timeFrame, marketInfo, "i_MOM:12");
                mommom1.Store = true;
                indicators.Add(mommom1.ShortName, mommom1);

                Indicator psar200 = new ParabolicSAR(200, 0.02f, 0.02f, 0.2f, timeFrame, marketInfo);
                psar200.Store = true;
                indicators.Add(psar200.ShortName, psar200);

                Indicator adx14 = new ADX(14, timeFrame, marketInfo);
                adx14.Store = true;
                indicators.Add(adx14.ShortName, adx14);

                //Indicator price = new Price("price", 200, timeFrame);
                //indicators.Add(price.ShortName, price);

                return indicators;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }
    }


}

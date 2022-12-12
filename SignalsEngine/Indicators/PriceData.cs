using BrokerLib.Brokers;
using BrokerLib.Market;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Indicators
{
    public class PriceData : Price
    {
        public const string NAME = "i_price:200"; 
        private MarketInfo _marketInfo = null;
        private TimeFrames _timeFrame = TimeFrames.H1;
        private Price _timeSeriesDay = null;

        private Price _selectedTimeSeries = null;
        private int _startSelectionIdx = 0;
        private int _endSelectionIdx = 0;


        private int _currentIdx = 0;
        private LinkedListNode<Dictionary<string, Candle>> _currentCandle = null;
        private Broker _broker = null;

        public PriceData(int Period, TimeFrames TimeFrame, Broker broker, MarketInfo marketInfo)
        : base("price", Period, TimeFrame, marketInfo)
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
            _broker = broker;
            _marketInfo = marketInfo;
            _timeSeriesDay = new Price("priceDay", Period, TimeFrame, marketInfo);
            _selectedTimeSeries = new Price("priceSelected", Period, TimeFrame, marketInfo);
        }

        public override List<string> CreateLines()
        {
            try
            {
                return base.CreateLines();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public override void Clear()
        {
            try
            {
                base.Clear();
                _timeSeriesDay.Clear();
                _selectedTimeSeries.Clear();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public List<Candle> Init(TimeFrames timeFrame, int count)
        {
            try
            {
                bool first = true;
                List<Candle> candlesListShared = GetLastCandles(timeFrame, count);
                foreach (Candle lastCandle in candlesListShared)
                {
                    if (!first) 
                    {
                        Candle _lastCandle = GetLastValue("middle");

                        if (_lastCandle != null && lastCandle.Timestamp <= _lastCandle.Timestamp)
                        {
                            SignalsEngine.DebugMessage(String.Format("Indicator::Init({0},{1},{2}) : lastCandle {3} is lesser or iqual than last saved candle {4}.",
                                                        string.IsNullOrEmpty(_lastCandle.Symbol) ? (string.IsNullOrEmpty(lastCandle.Symbol) ? MarketInfo.GetMarket() : lastCandle.Symbol) : _lastCandle.Symbol,
                                                        _lastCandle.TimeFrame,
                                                        this.ShortName,
                                                        lastCandle.Timestamp,
                                                        _lastCandle.Timestamp));
                            return null;
                        }
                    }
                    first = false;
                    AddLastValues(lastCandle);
                    //lastCandle.Store();

                }
                _currentCandle = GetLastValueNode();
                _currentIdx = Count();
                //_timeSeriesDay.Init(this);
                return candlesListShared;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public override void Init(Indicator indicator)
        {
            try
            {
                base.Init(indicator);
                _timeSeriesDay.Init(indicator);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public override bool CalculateNext(Indicator indicator)
        {
            List<Candle> candlesListShared = GetLastCandles(TimeFrame, 1);
            if (candlesListShared == null || candlesListShared.Count == 0)
            {
                SignalsEngine.DebugMessage(String.Format("PriceData::CalculateNext({0}) : candlesListShared is null or empty!", ShortName));
                return false;
            }
            foreach (Candle lastCandle in candlesListShared)
            {
                Candle _lastCandle = GetLastValue("middle");

                if (_lastCandle != null && lastCandle.Timestamp <= _lastCandle.Timestamp)
                {
                    SignalsEngine.DebugMessage(String.Format("PriceData::CalculateNext({0},{1},{2}) : lastCandle {3} is lesser or iqual than last saved candle {4}.",
                                                string.IsNullOrEmpty(_lastCandle.Symbol) ? (string.IsNullOrEmpty(lastCandle.Symbol) ? MarketInfo.GetMarket() : lastCandle.Symbol) : _lastCandle.Symbol,
                                                _lastCandle.TimeFrame,
                                                ShortName,
                                                lastCandle.Timestamp,
                                                _lastCandle.Timestamp));
                    return false;
                }
                AddLastValues(lastCandle);
                lastCandle.Store();

            }
            _currentCandle = GetLastValueNode();
            _currentIdx = Count();

            _timeSeriesDay.CalculateNext(this);

            return true;
        }

        public void AddLastValues(Candle lastCandle)
        {
            try
            {
                Dictionary<string, Candle> valueList = new Dictionary<string, Candle>();
                Candle candle = new Candle();
                candle.Close = lastCandle.Close;
                candle.Open = lastCandle.Open;
                candle.Min = lastCandle.Min;
                candle.Max = lastCandle.Max;
                candle.TimeFrame = lastCandle.TimeFrame;
                candle.Volume = lastCandle.Volume;
                candle.VolumeQuote = lastCandle.VolumeQuote;
                candle.Symbol = lastCandle.Symbol;
                candle.Timestamp = lastCandle.Timestamp;
                valueList.Add("middle", candle);

                candle = new Candle();
                candle.Close = lastCandle.Min;
                candle.Timestamp = lastCandle.Timestamp;
                valueList.Add("min", candle);

                candle = new Candle();
                candle.Close = lastCandle.Max;
                candle.Timestamp = lastCandle.Timestamp;
                valueList.Add("max", candle);

                candle = new Candle();
                candle.Close = lastCandle.Open;
                candle.Timestamp = lastCandle.Timestamp;
                valueList.Add("open", candle);

                AddLastValue(valueList);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        ///////////////////////////////////////////////////////
        ////////////////// CANDLES CODE //////////////////////
        //////////////////////////////////////////////////////

        public MarketInfo GetMarketInfo()
        {
            return _marketInfo;
        }
        public string GetMarket()
        {
            return _marketInfo.GetMarket();
        }

        public Broker GetBroker()
        {
            return _broker;
        }

        public Brokers GetBrokerName()
        {
            return _broker.GetBrokerId();
        }

        public Price GetTimeSeries()
        {
            try
            {
                return this;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Price GetTimeSeriesDay(bool reset = false)
        {
            try
            {
                if (reset)
                {
                    _timeSeriesDay.Clear();
                    _timeSeriesDay.AddLastValue(BotLib.BotLib.Backtest ? _selectedTimeSeries.GetLastValue() : GetLastValue());
                }
                return _timeSeriesDay;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Price GetSelectedTimeSeriesDay()
        {
            try
            {
                //FIXME
                return _timeSeriesDay;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public override Candle GetLastValue(string name = "middle")
        {
            try
            {
                if (BotLib.BotLib.Backtest)
                {
                    return _selectedTimeSeries.GetLastValue("middle");
                }
                return base.GetLastValue("middle");
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Price GetCandlesDay()
        {
            try
            {
                return _timeSeriesDay;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Price GetSelectedTimeSeries()
        {
            try
            {
                return _selectedTimeSeries;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public void IncludeNextCandleInSelection()
        {
            try
            {
                if (_endSelectionIdx >= Count() - 1)
                {
                    SignalsEngine.DebugMessage("Price::IncludeNextCandleInSelection() : Done.");
                    return;
                }
                _selectedTimeSeries.AddLastValue(ValueAt(++_endSelectionIdx, "middle"));
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public Price SelectTimeSeries(int endCount)
        {
            try
            {
                if (endCount > Count())
                {
                    SignalsEngine.DebugMessage("Price::SelectPrice() : startIndex + lenght exceeds timeSeries length.");
                }

                if (_startSelectionIdx == Count() - endCount && _endSelectionIdx == endCount)
                {
                    return _selectedTimeSeries;
                }

                _selectedTimeSeries.Clear();

                int i = 0;
                List<Candle> candles = new List<Candle>();
                for (i = Count() - endCount; i < Count(); i++)
                {
                    _selectedTimeSeries.AddLastValue(ValueAt(i));
                }

                _startSelectionIdx = Count() - endCount;
                _endSelectionIdx = i;

                return _selectedTimeSeries;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Price SelectTimeSeries(int startIndex, int length)
        {
            try
            {
                if (startIndex < 0)
                {
                    SignalsEngine.DebugMessage("Price::SelectPrice() : startIndex is negative interger.");
                }
                else if (startIndex > Count())
                {
                    SignalsEngine.DebugMessage("Price::SelectPrice() : startIndex exceeds timeSeries length.");
                }
                else if (startIndex + length > Count())
                {
                    length = Count() - startIndex;
                    //SignalsEngine.DebugMessage("Price::SelectPrice() : startIndex + lenght exceeds timeSeries length.");
                }

                if (_startSelectionIdx == startIndex && _endSelectionIdx == startIndex + length)
                {
                    return _selectedTimeSeries;
                }

                _selectedTimeSeries.Clear();
                int i = 0;
                List<Candle> candles = new List<Candle>();
                for (i = 0; i < Count() && i >= startIndex && length > 0; i++)
                {
                    _selectedTimeSeries.AddLastValue(ValueAt(i));
                    length--;
                }

                _startSelectionIdx = startIndex;
                _endSelectionIdx = startIndex + i;

                return _selectedTimeSeries;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }
        public List<MissingCandleInterval> GetMissingIntervals(List<Candle> candles, DateTime fromDate, DateTime toDate, TimeFrames timeFrame)
        {
            try
            {
                List<MissingCandleInterval> missingIntervals = new List<MissingCandleInterval>();

                if (candles == null || candles.Count == 0)
                {
                    missingIntervals.Add(new MissingCandleInterval(fromDate, toDate, 0));
                    return missingIntervals;
                }

                if (candles.Count == 1)
                {
                    missingIntervals.Add(new MissingCandleInterval(fromDate, toDate, 1));
                    return missingIntervals;
                }

                int timeFrameInt = (int)timeFrame;
                Candle lastCandle = candles[0];
                int i = 0;

                if (lastCandle.Timestamp > fromDate)
                {
                    missingIntervals.Add(new MissingCandleInterval(fromDate, lastCandle.Timestamp.AddMinutes(-timeFrameInt), i));
                }
                else if (lastCandle.Timestamp < fromDate)
                {
                    SignalsEngine.DebugMessage("GetMissingIntervals() : first candle is before than fromDate");
                }

                for (i = 1; i < candles.Count; i++)
                {
                    if (lastCandle.Timestamp < candles[i].Timestamp.AddMinutes(-timeFrameInt))
                    {
                        missingIntervals.Add(new MissingCandleInterval(lastCandle.Timestamp.AddMinutes(timeFrameInt), candles[i].Timestamp.AddMinutes(-timeFrameInt), i - 1));
                    }
                    else if (lastCandle.Timestamp > candles[i].Timestamp.AddMinutes(-timeFrameInt))
                    {
                        SignalsEngine.DebugMessage("GetMissingIntervals() : previous lastCandle candle is after than lastCandle");
                    }
                    lastCandle = candles[i];
                }

                if (lastCandle.Timestamp < toDate)
                {
                    missingIntervals.Add(new MissingCandleInterval(lastCandle.Timestamp.AddMinutes(timeFrameInt), toDate, i - 1));
                }
                else if (lastCandle.Timestamp > toDate)
                {
                    SignalsEngine.DebugMessage("GetMissingIntervals() : last candle is after than fromDate");
                }

                if (_marketInfo.GetMarketType() == MarketTypes.Forex || _marketInfo.GetMarketType() == MarketTypes.Stocks)
                {
                    List<MissingCandleInterval> missingIntervalsToRemove = new List<MissingCandleInterval>();

                    foreach (var missingInterval in missingIntervals)
                    {
                        if (!_marketInfo._watch.IsOpen(missingInterval._fromDate) || !_marketInfo._watch.IsOpen(missingInterval._toDate))
                        {
                            missingIntervalsToRemove.Add(missingInterval);
                        }
                    }
                    foreach (var missingInterval in missingIntervalsToRemove)
                    {
                        missingIntervals.Remove(missingInterval);
                    }
                }

                return missingIntervals;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public List<Candle> GetHistoricalData(DateTime fromDate, DateTime toDate, TimeFrames timeFrame, int startCount)
        {
            try
            {
                DateTime currentCandleDate = fromDate;
                if (_marketInfo.GetMarketType().Equals(MarketTypes.Crypto))
                {
                    fromDate = fromDate.AddMinutes(-200 * (int)timeFrame);
                }
                else if (_marketInfo.GetMarketType().Equals(MarketTypes.Forex))
                {
                    fromDate = fromDate.AddMinutes(-800 * (int)timeFrame);
                }
                else
                {
                    fromDate = fromDate.AddMinutes(-800 * (int)timeFrame);
                }
                //first search in database
                //if not found request from api
                SignalsEngine.DebugMessage(String.Format("GetHistoricalData(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Getting historical data...", _marketInfo.GetMarket(), fromDate, toDate, timeFrame));

                //long expectedPeriod = (toDate.Ticks - fromDate.Ticks) / (60000 * (long)timeFrames);
                List<Candle> candles = null;

                //candles = _brokerContext.GetCandlesFromDB(timeFrame, _marketInfo._market, fromDate, toDate);

                //if (!Price.IsConsistent(candles))
                //{
                //    SignalsEngine.DebugMessage(String.Format("GetHistoricalData(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Inconsistent candles after GetCandlesFromDB!", _marketInfo._market, fromDate, toDate, timeFrame));
                //}

                candles = GetCandlesFromBroker(candles, fromDate, toDate, timeFrame);
                Period = candles.Count;
                Lines.Period = Period;
                if (!PriceData.IsConsistent(candles))
                {
                    SignalsEngine.DebugMessage(String.Format("GetHistoricalData(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Inconsistent candles after GetCandlesFromBroker!", _marketInfo.GetMarket(), fromDate, toDate, timeFrame));
                }

                //candles = NormalizeCandles(candles, fromDate, toDate, timeFrame);

                //if (!Price.IsConsistent(candles))
                //{
                //    SignalsEngine.DebugMessage(String.Format("GetHistoricalData(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Inconsistent candles after normalization!", _marketInfo._market, fromDate, toDate, timeFrame));
                //}

                Clear();

                for (int i = 0; i < candles.Count; i++)
                {
                    Candle candle = candles[i];
                    AddLastValue(candle);
                }

                _currentCandle = GetLastValueNode();

                return candles;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public List<Candle> GetCandlesFromBroker(List<Candle> candles, DateTime fromDate, DateTime toDate, TimeFrames timeFrame)
        {
            try
            {
                fromDate = DateTimeExtensions.Normalize(fromDate, (int) timeFrame);
                toDate = DateTimeExtensions.Normalize(toDate, (int) timeFrame);

                Dictionary<int, List<Candle>> candlesToInsert = new Dictionary<int, List<Candle>>();
                CandleDictionary candleDictionary = new CandleDictionary();
                List<MissingCandleInterval> missingIntervals = GetMissingIntervals(candles, fromDate, toDate, timeFrame);

                if (candles != null)
                {
                    candleDictionary.AddCandles(candles);
                }

                foreach (var missingCandleInterval in missingIntervals)
                {
                    List<Candle> candleList = GetDataByTimeFrame(timeFrame, missingCandleInterval._fromDate, missingCandleInterval._toDate);

                    long expectedPeriod = ((missingCandleInterval._toDate.Ticks - missingCandleInterval._fromDate.Ticks) / (600000000 * (long)timeFrame)) + 1;
                    //long expectedPeriod = (long)(missingCandleInterval._toDate - missingCandleInterval._fromDate).TotalMinutes / (long)timeFrame;

                    if (candleList != null && candleList.Count > 0)
                    {
                        //candleList = NormalizeCandles(candleList, missingCandleInterval._fromDate, missingCandleInterval._toDate, timeFrame);
                        List<Candle> candlesToSave = new List<Candle>();

                        candlesToSave.AddRange(candleList);
                        
                        if (!BotLib.BotLib.Backtest) 
                        {
                            candlesToSave = Candle.SaveCandlesInDB(candlesToSave);
                        }

                        candlesToInsert.Add(missingCandleInterval._index, candlesToSave);
                        //candles.InsertRange(missingCandleInterval._index, candlesToSave);

                    }
                }

                foreach (var pair in candlesToInsert)
                {

                    if (IsConsistent(pair.Value))
                    {
                        candleDictionary.AddCandles(pair.Value);
                        //candles.InsertRange(pair.Key, pair.Value);
                    }
                    else
                    {
                        SignalsEngine.DebugMessage("GetCandlesFromBroker() : candlesToInsert are inconsistent!");
                    }
                }

                candles = candleDictionary.ToList();

                if (!PriceData.IsConsistent(candles))
                {
                    SignalsEngine.DebugMessage(String.Format("GetCandlesFromBroker(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Inconsistent candles after normalization!", _marketInfo.GetMarket(), fromDate, toDate, timeFrame));
                }

                return candles;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public List<Candle> NormalizeCandles(List<Candle> candles, DateTime fromDate, DateTime toDate, TimeFrames timeFrame)
        {
            try
            {

                Dictionary<int, List<Candle>> candlesToInsert = new Dictionary<int, List<Candle>>();
                CandleDictionary candleDictionary = new CandleDictionary();
                List<MissingCandleInterval> missingIntervals = GetMissingIntervals(candles, fromDate, toDate, timeFrame);

                candleDictionary.AddCandles(candles);

                foreach (var missingCandleInterval in missingIntervals)
                {
                    long expectedPeriod = ((missingCandleInterval._toDate.Ticks - missingCandleInterval._fromDate.Ticks) / (600000000 * (long)timeFrame)) + 1;

                    Candle lastCandle = null;
                    Candle nextCandle = null;
                    try
                    {
                        if (missingCandleInterval._index + 1 >= candles.Count)
                        {
                            lastCandle = ValueAt(missingCandleInterval._index - 1, "middle");
                            nextCandle = ValueAt(missingCandleInterval._index, "middle");
                        }
                        else
                        {
                            lastCandle = ValueAt(missingCandleInterval._index, "middle");
                            nextCandle = ValueAt(missingCandleInterval._index + 1, "middle");
                        }
                    }
                    catch (Exception e)
                    {
                        SignalsEngine.DebugMessage(e);
                    }
                    float difference = nextCandle.Close - lastCandle.Close;
                    float differenceIncrement = difference / expectedPeriod;

                    List<Candle> normalizedCandles = new List<Candle>();

                    for (int i = 0; i < expectedPeriod; i++)
                    {
                        Candle newCandle = new Candle(lastCandle.TimeFrame,
                                                            lastCandle.Symbol,
                                                            missingCandleInterval._fromDate.AddMinutes((i++) * (int)timeFrame),
                                                            lastCandle.Min,
                                                            lastCandle.Max,
                                                            lastCandle.Close + (i + 1) * differenceIncrement,
                                                            lastCandle.Open + (i + 1) * differenceIncrement,
                                                            lastCandle.Volume,
                                                            lastCandle.VolumeQuote);

                        normalizedCandles.Add(newCandle);
                    }
                    if (normalizedCandles.Count > 0)
                    {
                        candlesToInsert.Add(missingCandleInterval._index, normalizedCandles);
                        //candles.InsertRange(missingCandleInterval._index, normalizedCandles);
                        Candle.SaveCandlesInDB(normalizedCandles);
                    }

                }

                foreach (var pair in candlesToInsert)
                {
                    if (IsConsistent(pair.Value))
                    {
                        candleDictionary.AddCandles(pair.Value);
                        //candles.InsertRange(pair.Key, pair.Value);
                    }
                    else
                    {
                        SignalsEngine.DebugMessage("NormalizeCandles() : candlesToInsert are inconsistent!");
                    }
                }

                candles = candleDictionary.ToList();

                if (!PriceData.IsConsistent(candles))
                {
                    SignalsEngine.DebugMessage(String.Format("NormalizeCandles(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Inconsistent candles after normalization!", _marketInfo.GetMarket(), fromDate, toDate, timeFrame));
                }

                return candles;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public List<Candle> GetLastCandles(TimeFrames timeFrame, int count)
        {
            try
            {
                List<Candle> candles = _broker.GetLastCandles(_marketInfo.GetMarket(), timeFrame, count);
                if (candles == null)
                {
                    return null;
                }
                return candles;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public LinkedListNode<Dictionary<string, Candle>> GetNextCandleNode(/*int count*/)
        {
            try
            {
                //Candle candle = null;
                //if (_currentIdx < _timeSeries.Count - 1)
                //{
                //    _currentCandle = _currentCandle.Next;
                //    candle = _timeSeries.ValueAt(++_currentIdx);
                //}
                //return candle;

                if (_currentCandle != null && _currentCandle.Next != null)
                {
                    _currentCandle = _currentCandle.Next;
                    _currentIdx++;
                    return _currentCandle;
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return _currentCandle;
        }

        public LinkedListNode<Dictionary<string, Candle>> GetCurrentCandleNode()
        {
            try
            {
                //Candle candle = null;
                //if (_currentIdx < _timeSeries.Count - 1)
                //{
                //    candle = _timeSeries.ValueAt(_currentIdx);
                //}
                //return candle;

                if (_currentCandle != null)
                {
                    return _currentCandle;
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return _currentCandle;
        }

        public List<Candle> GetDataByTimeFrame(TimeFrames timeFrame, DateTime fromDate, DateTime toDate)
        {
            try
            {
                SignalsEngine.DebugMessage(String.Format("GetDataByTimeFrame({0}, {1}, {2}, {3}): Getting candles...", _marketInfo.GetMarket(), timeFrame, fromDate, toDate));
                List<Candle> candlesList = _broker.GetCandles(_marketInfo.GetMarket(), timeFrame, fromDate, toDate);
                //_timeSeriesDay = new Price(_timeFrame, candlesList);
                return candlesList;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public List<Candle> GetDataToInitDayByTimeFrame()
        {
            try
            {
                List<Candle> candlesList = _broker.GetCandlesToInitDay(_marketInfo.GetMarket(), _timeFrame);
                foreach (Candle candle in candlesList)
                {
                    _timeSeriesDay.AddLastValue(candle);
                }
                return candlesList;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public static bool IsConsistent(List<Candle> candles)
        {
            try
            {
                if (candles != null && candles.Count >= 0)
                {
                    for (int i = 1; i < candles.Count; i++)
                    {
                        Candle candle = candles[i - 1];
                        Candle nextCandle = candles[i];

                        if (candle != null && nextCandle != null &&
                            (nextCandle.Timestamp <= candle.Timestamp || candle.IsAbnormalCandle() || nextCandle.IsAbnormalCandle()))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }
    }
}

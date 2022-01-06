using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static BrokerLib.BrokerLib;
using BrokerLib.Models;
using BrokerLib.Lib;
using BrokerLib.Brokers;
using BrokerLib.Market;
using Microsoft.EntityFrameworkCore;

namespace BrokerLib.Lib
{

    public class MissingCandleInterval 
    {
        public DateTime _fromDate;
        public DateTime _toDate;
        public int      _index;

        public MissingCandleInterval(DateTime fromDate, DateTime toDate, int index) 
        {
            _fromDate   = fromDate;
            _toDate     = toDate;
            _index      = index;
        }
    }
    public class Candles
    {
        private readonly BrokerDBContext _brokerContext;
        private MarketInfo _marketInfo = null;
        private TimeFrames _timeFrame = TimeFrames.H1;
        private LinkedList<Candle> _timeSeries = null;
        private LinkedList<Candle> _timeSeriesDay = null;

        private LinkedList<Candle> _selectedTimeSeries = null;
        private int _startSelectionIdx = 0;
        private int _endSelectionIdx = 0;


        private int _currentIdx = 0;
        private LinkedListNode<Candle> _currentCandle = null;
        private Broker _broker = null;
        public Candles(Broker broker, MarketInfo marketInfo, TimeFrames timeFrame)
        {
            _marketInfo = marketInfo;
            _timeFrame = timeFrame;
            _timeSeries = new LinkedList<Candle>();
            _timeSeriesDay = new LinkedList<Candle>();
            _selectedTimeSeries = new LinkedList<Candle>();

            //_candles = new List<Candle>();
            _brokerContext = BrokerDBContext.newDBContext();
            _broker = broker;
        }

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

        public BrokerLib.Brokers GetBrokerName()
        {
            return _broker.GetBrokerId();
        }

        public LinkedList<Candle> GetTimeSeries() 
        {
            try
            {
                return _timeSeries;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public LinkedList<Candle> GetCandles()
        {
            try
            {
                return _timeSeries;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public LinkedList<Candle> GetSelectedTimeSeriesDay()
        {
            try
            {
                //FIXME
                return _timeSeriesDay;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public LinkedList<Candle> GetCandlesDay()
        {
            try
            {
                return _timeSeriesDay;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public LinkedList<Candle> GetSelectedTimeSeries() 
        {
            try
            {
                return _selectedTimeSeries;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public void IncludeNextCandleInSelection() 
        {
            try
            {
                if (_endSelectionIdx >= _timeSeries.Count - 1)
                {
                    BrokerLib.DebugMessage("Candles::IncludeNextCandleInSelection() : Done.");
                    return;
                }
                _selectedTimeSeries.AddLast(_timeSeries.ElementAt(++_endSelectionIdx));
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public LinkedList<Candle> SelectTimeSeries(int endCount)
        {
            try
            {
                if (endCount > _timeSeries.Count)
                {
                    BrokerLib.DebugMessage("Candles::SelectLinkedList<Candle>() : startIndex + lenght exceeds timeSeries length.");
                }

                if (_startSelectionIdx == _timeSeries.Count - endCount && _endSelectionIdx == endCount)
                {
                    return _selectedTimeSeries;
                }

                _selectedTimeSeries.Clear();

                int i = 0;
                List<Candle> candles = new List<Candle>();
                for (i = _timeSeries.Count - endCount; i < _timeSeries.Count; i++)
                {
                    _selectedTimeSeries.AddLast(_timeSeries.ElementAt(i));
                }

                _startSelectionIdx = _timeSeries.Count - endCount;
                _endSelectionIdx = i;

                return _selectedTimeSeries;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public LinkedList<Candle> SelectTimeSeries(int startIndex, int length)
        {
            try
            {
                if (startIndex < 0)
                {
                    BrokerLib.DebugMessage("Candles::SelectLinkedList<Candle>() : startIndex is negative interger.");
                }
                else if (startIndex > _timeSeries.Count)
                {
                    BrokerLib.DebugMessage("Candles::SelectLinkedList<Candle>() : startIndex exceeds timeSeries length.");
                }
                else if (startIndex + length > _timeSeries.Count)
                {
                    length = _timeSeries.Count - startIndex;
                    //BrokerLib.DebugMessage("Candles::SelectLinkedList<Candle>() : startIndex + lenght exceeds timeSeries length.");
                }

                if (_startSelectionIdx == startIndex && _endSelectionIdx == startIndex + length)
                {
                    return _selectedTimeSeries;
                }

                _selectedTimeSeries.Clear();
                int i = 0;
                List<Candle> candles = new List<Candle>();
                for (i = 0; i < _timeSeries.Count && i >= startIndex && length > 0; i++)
                {
                    _selectedTimeSeries.AddLast(_timeSeries.ElementAt(i));
                    length--;
                }

                _startSelectionIdx = startIndex;
                _endSelectionIdx = startIndex + i;

                return _selectedTimeSeries;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
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
                    BrokerLib.DebugMessage("GetMissingIntervals() : first candle is before than fromDate");
                }

                for (i = 1; i < candles.Count; i++)
                {
                    if (lastCandle.Timestamp < candles[i].Timestamp.AddMinutes(-timeFrameInt))
                    {
                        missingIntervals.Add(new MissingCandleInterval(lastCandle.Timestamp.AddMinutes(timeFrameInt), candles[i].Timestamp.AddMinutes(-timeFrameInt), i - 1));
                    }
                    else if (lastCandle.Timestamp > candles[i].Timestamp.AddMinutes(-timeFrameInt))
                    {
                        BrokerLib.DebugMessage("GetMissingIntervals() : previous lastCandle candle is after than lastCandle");
                    }
                    lastCandle = candles[i];
                }

                if (lastCandle.Timestamp < toDate)
                {
                    missingIntervals.Add(new MissingCandleInterval(lastCandle.Timestamp.AddMinutes(timeFrameInt), toDate, i - 1));
                }
                else if (lastCandle.Timestamp > toDate)
                {
                    BrokerLib.DebugMessage("GetMissingIntervals() : last candle is after than fromDate");
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
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public bool CandlesInDB(DateTime fromDate, DateTime toDate, TimeFrames timeFrame) 
        {
            try
            {
                using (BrokerDBContext dataContext = BrokerDBContext.newDBContext())
                {
                    if (dataContext.Candles.AsNoTracking().Any(m => m.TimeFrame.Equals(timeFrame) &&
                                             m.Timestamp >= fromDate &&
                                             m.Timestamp <= toDate))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return false;
        }

        public bool CandlesInLocalDBContext(DateTime fromDate, DateTime toDate, TimeFrames timeFrame)
        {
            try
            {
                if (_brokerContext.Candles.Local.Any(m => m.TimeFrame.Equals(timeFrame) &&
                                             m.Timestamp >= fromDate &&
                                             m.Timestamp <= toDate))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return false;
        }
        public List<Candle> GetCandlesFromDB(DateTime fromDate, DateTime toDate, TimeFrames timeFrame) 
        {
            try
            {
                using (BrokerDBContext dataContext = BrokerDBContext.newDBContext())
                {
                    List<Candle> candlesFromDB = dataContext.Candles.AsNoTracking().Where(m => m.TimeFrame.Equals(timeFrame) &&
                                                          m.Timestamp >= fromDate &&
                                                          m.Timestamp <= toDate).ToList();
                }
 
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public List<Candle> GetCandlesFromLocalDBContext(DateTime fromDate, DateTime toDate, TimeFrames timeFrame)
        {
            try
            {
                List<Candle> candlesFromDB = _brokerContext.Candles.Local.Where(m => m.TimeFrame.Equals(timeFrame) &&
                                                                        m.Timestamp >= fromDate &&
                                                                        m.Timestamp <= toDate).ToList();
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public List<Candle> GetHistoricalData(DateTime fromDate, DateTime toDate, TimeFrames timeFrame, int lastCount = -1) 
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
                BrokerLib.DebugMessage(String.Format("GetHistoricalData(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Getting historical data...", _marketInfo.GetMarket(), fromDate, toDate, timeFrame));

                //long expectedPeriod = (toDate.Ticks - fromDate.Ticks) / (60000 * (long)timeFrames);
                List<Candle> candles = null;

                //candles = _brokerContext.GetCandlesFromDB(timeFrame, _marketInfo._market, fromDate, toDate);

                //if (!Candles.IsConsistent(candles))
                //{
                //    BrokerLib.DebugMessage(String.Format("GetHistoricalData(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Inconsistent candles after GetCandlesFromDB!", _marketInfo._market, fromDate, toDate, timeFrame));
                //}

                candles = GetCandlesFromBroker(candles, fromDate, toDate, timeFrame);

                if (!Candles.IsConsistent(candles))
                {
                    BrokerLib.DebugMessage(String.Format("GetHistoricalData(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Inconsistent candles after GetCandlesFromBroker!", _marketInfo.GetMarket(), fromDate, toDate, timeFrame));
                }

                //candles = NormalizeCandles(candles, fromDate, toDate, timeFrame);

                //if (!Candles.IsConsistent(candles))
                //{
                //    BrokerLib.DebugMessage(String.Format("GetHistoricalData(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Inconsistent candles after normalization!", _marketInfo._market, fromDate, toDate, timeFrame));
                //}

                _currentIdx = lastCount > 0 ? lastCount : _currentIdx;

                _timeSeries.Clear();
                _timeSeriesDay.Clear();
                _selectedTimeSeries.Clear();

                int auxLastCount = lastCount;

                if (lastCount > 0)
                {
                    int currentIdx = 0;
                    for (int i = 0; i < candles.Count; i++)
                    {
                        if (candles[i].Timestamp == currentCandleDate)
                        {
                            currentIdx = i;
                            break;
                        }
                    }
                    int idx = lastCount > currentIdx ? 0 : currentIdx - lastCount;
                    for (int i = idx; i < candles.Count; i++)
                    {
                        Candle candle = candles[i];
                        _timeSeries.AddLast(candle);
                        if (auxLastCount > 0)
                        {
                            _currentCandle = _timeSeries.Last;//#1
                            auxLastCount--;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < candles.Count; i++)
                    {
                        Candle candle = candles[i];
                        _timeSeries.AddLast(candle);
                    }
                }

                if (auxLastCount > 0)
                {
                    lastCount -= auxLastCount;
                }

                if (lastCount > 0)
                {
                    return _timeSeries.Take(lastCount).ToList();//#1 returns here
                }

                _currentCandle = _timeSeries.Last;

                return candles;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public List<Candle> GetCandlesFromBroker(List<Candle> candles, DateTime fromDate, DateTime toDate, TimeFrames timeFrame)
        {
            try
            {

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

                        candlesToSave = Candle.SaveCandlesInDB(candlesToSave);

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
                        BrokerLib.DebugMessage("GetCandlesFromBroker() : candlesToInsert are inconsistent!");
                    }
                }

                candles = candleDictionary.ToList();

                if (!Candles.IsConsistent(candles))
                {
                    BrokerLib.DebugMessage(String.Format("GetCandlesFromBroker(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Inconsistent candles after normalization!", _marketInfo.GetMarket(), fromDate, toDate, timeFrame));
                }

                return candles;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
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
                            lastCandle = candles.ElementAt(missingCandleInterval._index - 1);
                            nextCandle = candles.ElementAt(missingCandleInterval._index);
                        }
                        else
                        {
                            lastCandle = candles.ElementAt(missingCandleInterval._index);
                            nextCandle = candles.ElementAt(missingCandleInterval._index + 1);
                        }
                    }
                    catch (Exception e)
                    {
                        BrokerLib.DebugMessage(e);
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
                        BrokerLib.DebugMessage("NormalizeCandles() : candlesToInsert are inconsistent!");
                    }
                }

                candles = candleDictionary.ToList();
                
                if (!Candles.IsConsistent(candles))
                {
                    BrokerLib.DebugMessage(String.Format("NormalizeCandles(market : {0}, from : {1}, to : {2}, timeFrame : {3}) : Inconsistent candles after normalization!", _marketInfo.GetMarket(), fromDate, toDate, timeFrame));
                }
                
                return candles;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
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
                foreach (Candle candle in candles)
                {
                    _timeSeries.AddLast(candle);
                }
                _currentCandle = _timeSeries.Last;
                _currentIdx = _timeSeries.Count;
                return candles;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public LinkedListNode<Candle> GetNextCandleNode(/*int count*/)
        {
            try
            {
                //Candle candle = null;
                //if (_currentIdx < _timeSeries.Count - 1)
                //{
                //    _currentCandle = _currentCandle.Next;
                //    candle = _timeSeries.ElementAt(++_currentIdx);
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
                BrokerLib.DebugMessage(e);
            }
            return _currentCandle;
        }

        public LinkedListNode<Candle> GetCurrentCandleNode()
        {
            try
            {
                //Candle candle = null;
                //if (_currentIdx < _timeSeries.Count - 1)
                //{
                //    candle = _timeSeries.ElementAt(_currentIdx);
                //}
                //return candle;

                if (_currentCandle != null)
                {
                    return _currentCandle;
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return _currentCandle;
        }

        public List<Candle> GetDataByTimeFrame(TimeFrames timeFrame, DateTime fromDate, DateTime toDate)
        {
            try
            {
                BrokerLib.DebugMessage(String.Format("GetDataByTimeFrame({0}, {1}, {2}, {3}): Getting candles...", _marketInfo.GetMarket(), timeFrame, fromDate, toDate));
                List<Candle> candlesList = _broker.GetCandles(_marketInfo.GetMarket(), timeFrame, fromDate, toDate);
                //_timeSeriesDay = new LinkedList<Candle>(_timeFrame, candlesList);
                return candlesList;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
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
                    _timeSeriesDay.AddLast(candle);
                }
                return candlesList;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public static List<Candle> FixConsistency(List<Candle> candles)
        {
            try
            {
                if (candles != null && candles.Count >= 0)
                {
                    List<Candle> candlesToRemove = new List<Candle>();
                    candles.Sort(new CandleComparer());
                    for (int i = 1; i < candles.Count; i++)
                    {
                        Candle candle = candles[i - 1];
                        Candle nextCandle = candles[i];

                        if (candle.Timestamp == nextCandle.Timestamp || nextCandle.IsAbnormalCandle())
                        {
                            candlesToRemove.Add(nextCandle);
                        }
                    }
                    foreach (Candle candle in candlesToRemove)
                    {
                        candles.Remove(candle);
                    }
                    return candles;
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
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
                    BrokerLib.DebugMessage(e);
                }
                return false;
            }

    }
}

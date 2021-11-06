// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Indicator.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   The base class for all indicators.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System;
using SignalsEngine.Conditions;
using static BrokerLib.BrokerLib;
using BrokerLib.Models;
using BrokerLib.Market;

namespace SignalsEngine.Indicators
{
    /// <summary>
    /// The base class for all indicators.
    /// </summary>
    public abstract class Indicator
    {
        /// <summary>
        /// Gets or sets the indicator name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the indicator short name.
        /// </summary>
        public string ShortName { get; protected set; }
        public string ShorDescriptionName { get; protected set; }
        public bool Special { get; protected set; }

        public string InputName { get; set; }

        public int Period { get; protected set; }
        public bool Store { get; set; }

        public TimeFrames TimeFrame { get; protected set; }

        public IndicatorLines Lines { get; protected set; }

        public MarketInfo MarketInfo { get; set; }

        public List<string> Arguments { get; protected set; }
        public Indicator(string ShortName, int Period, TimeFrames TimeFrame, MarketInfo marketInfo, string Name = null, bool AllowInconsistentData = false, bool Store = false, bool Special = false) 
        {
            this.ShortName = TextCondition.INDICATOR_PREFIX + ShortName;
            this.Name = Name;
            this.Period = Period;
            this.TimeFrame = TimeFrame;
            this.Lines = new IndicatorLines(Period, CreateLines(), AllowInconsistentData);
            this.Arguments = new List<string>();
            this.InputName = PriceData.NAME;
            this.MarketInfo = marketInfo;
            this.Store = Store;
            this.Special = Special;
        }

        protected string GetShorDescriptionName() 
        {
            try
            {
                string shortDescription = "";

                if (ShortName.Contains(":"))
                {
                    var tokens = ShortName.Split(":");
                    if (tokens.Length > 0)
                    {
                        shortDescription += tokens[0];
                    }
                    else
                    {
                        SignalsEngine.DebugMessage("Indicator::GetShorDescriptionName() : Split result has 0 elements.");
                        return null;
                    }
                }
                else
                {
                    shortDescription += ShortName;
                }
                foreach (var arg in Arguments)
                {
                    shortDescription += ":" + arg; 
                }
                return shortDescription;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Dictionary<string, int> GetLines()
        {
            try
            {
                return Lines.GetLines();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public virtual List<string> CreateLines()
        {
            try
            {
                List<string> lines = new List<string>();
                lines.Add("middle");
                return lines;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public void AddArgument(string argument) 
        {
            try
            {
                Arguments.Add(argument);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void AddValues(LinkedList<Dictionary<string, Candle>> values)
        {
            try
            {
                Lines.AddValues(values);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void AddLastValue(Dictionary<string, Candle> value) 
        {
            try
            {
                Lines.AddLastValue(value);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void AddCurrentValue(Candle value, string line = "middle")
        {
            try
            {
                Lines.AddCurrentValue(value, line);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void AddLastValue(Candle value, string line = "middle")
        {
            try
            {
                Lines.AddLastValue(value, line);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void AddCurrentClose(float close, DateTime timestamp, string line = "middle")
        {
            try
            {
                Candle candle = new Candle();
                candle.Close = close;
                candle.TimeFrame = this.TimeFrame;
                candle.Timestamp = timestamp;
                Lines.AddCurrentValue(candle, line);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void AddLastClose(float close, DateTime timestamp, string line = "middle")
        {
            try
            {
                Candle candle = new Candle();
                candle.Close = close;
                candle.TimeFrame = this.TimeFrame;
                candle.Timestamp = timestamp;
                Lines.AddLastValue(candle, line);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public Candle ValueAt(int idx, string name = "middle")
        {
            try
            {
                return Lines.ValueAt(idx, name);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Dictionary<string, Candle> ValueAt(int idx) 
        {
            try
            {
                return Lines.ValueAt(idx);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public int Count() 
        {
            try
            {
                return Lines.Count();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return 0;
        }

        public LinkedListNode<Dictionary<string, Candle>> GetFirstValueNode()
        {
            try
            {
                return Lines.GetFirstValueNode();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public LinkedListNode<Dictionary<string, Candle>> GetLastValueNode()
        {
            try
            {
                return Lines.GetLastValueNode();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Dictionary<string, Candle> GetLastValue()
        {
            try
            {
                return Lines.GetLastValue();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Candle GetLastValue(string name = "middle")
        {
            try
            {
                return Lines.GetLastValue(name);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public float GetLastClose(string name = "middle")
        {
            try
            {
                return Lines.GetLastClose(name);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return 0;
        }

        public DateTime GetLastTimestamp(string name = "middle")
        {
            try
            {
                return Lines.GetLastValue(name).Timestamp;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return DateTime.MinValue;
        }

        public float GetClose(int countFromLast, string name = "middle")
        {
            return Lines.GetValue(countFromLast, name).Close;
        }

        public Candle GetValue(int countFromLast, string name = "middle") 
        {
            return Lines.GetValue(countFromLast, name);
        }

        public void RemoveFirst() 
        {
            try
            {
                Lines.RemoveFirst();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void RemoveLast()
        {
            try
            {
                Lines.RemoveLast();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public LinkedList<Dictionary<string, Candle>> GetValues()
        {
            return Lines.GetValues();
        }

        public virtual void Clear()
        {
            Lines.Clear();
        }

        public virtual void Init(Indicator indicator)
        {
            this.Init(indicator, "middle", "middle");
        }

        public virtual void Init(Indicator indicator, string inLine = "middle", string outLine = "middle")
        {

        }

        public virtual bool CalculateNext(Indicator indicator)
        {
            return this.Validate(indicator, "middle", "middle");
        }

        public bool Validate(Indicator indicator, string inLine = "middle", string outLine = "middle", bool store = true)
        {
            try
            {
                Candle lastCandle = indicator.GetLastValue(inLine);
                Candle _lastCandle = GetLastValue(outLine);

                if (lastCandle == null)
                {
                    SignalsEngine.DebugMessage(String.Format("Indicator::CalculateNext({0},{1},{2}) : lastCandle is empty.",
                                                string.IsNullOrEmpty(_lastCandle.Symbol) ? (string.IsNullOrEmpty(lastCandle.Symbol) ? MarketInfo.GetMarket() : lastCandle.Symbol) : _lastCandle.Symbol,
                                                _lastCandle.TimeFrame,
                                                this.ShortName));
                    return false;
                }
                if (_lastCandle != null && lastCandle.Timestamp <= _lastCandle.Timestamp)
                {
                    SignalsEngine.DebugMessage(String.Format("Indicator::CalculateNext({0},{1},{2}) : lastCandle {3} is lesser or iqual than last saved candle {4}.",
                                                string.IsNullOrEmpty(_lastCandle.Symbol) ? (string.IsNullOrEmpty(lastCandle.Symbol) ? MarketInfo.GetMarket() : lastCandle.Symbol) : _lastCandle.Symbol,
                                                _lastCandle.TimeFrame,
                                                this.ShortName,
                                                lastCandle.Timestamp,
                                                _lastCandle.Timestamp));
                    return false;
                }
                if (_lastCandle == null)
                {
                    SignalsEngine.DebugMessage(String.Format("Indicator::CalculateNext({0},{1},{2}) : _lastCandle is empty.",
                            string.IsNullOrEmpty(lastCandle.Symbol) ? MarketInfo.GetMarket() : lastCandle.Symbol,
                            lastCandle.TimeFrame,
                            this.ShortName));
                    return false;
                }
                if (store)
                {
                    StoreLinesCurrentValues(lastCandle);
                }
                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }

        public void StoreLinesCurrentValues(Candle lastCandle)
        {
            if (Store)
            {
                Candle _lastCandle;
                foreach (var line in Lines.Lines)
                {
                    _lastCandle = GetLastValue(line.Key);
                    _lastCandle.TimeFrame = lastCandle.TimeFrame;
                    _lastCandle.Timestamp = lastCandle.Timestamp;
                    _lastCandle.Symbol = MarketInfo.GetMarket();
                    StoreLastValue(_lastCandle, line.Key);
                }
            }
        }

        public virtual float Slope(string name = "middle") 
        {
            try
            {
                return Lines.Slope(name);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return 0;
        }

        public virtual void StoreLastValue(Candle candle, string line = "middle")
        {
            try
            {
                if (!Store)
                {
                    return;
                }
                if (candle == null)
                {
                    return;
                }
                Point point = new Point();
                point.TimeFrame = candle.TimeFrame;
                point.Timestamp = candle.Timestamp;//.AddMinutes((int)candle.TimeFrame);
                point.Value = candle.Close;
                point.Line = this.ShortName + "_" + line;
                point.TimeFrame = this.TimeFrame;
                point.Symbol = MarketInfo.GetMarket();
                //BrokerBulkStore.Instance.Store(point);
                point.Store();
                return;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return;
        }

    }
}

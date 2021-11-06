using BrokerLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalsEngine.Indicators
{
    public class CandleDictionary
    {
        public Dictionary<DateTime, Candle> _candleDictionary = null;
        public CandleDictionary() 
        {
            _candleDictionary = new Dictionary<DateTime, Candle>();
        }

        public void AddCandle(Candle candle)
        {
            try
            {
                if (candle == null)
                {
                    return;
                }
                if (_candleDictionary.ContainsKey(candle.Timestamp))
                {
                    SignalsEngine.DebugMessage(String.Format("CandleDictionay::AddCandle({0}) candle already present in dictionary.", candle.Timestamp));
                }
                else
                {
                    _candleDictionary.Add(candle.Timestamp, candle);
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void AddCandles(List<Candle> candles) 
        {
            try
            {
                foreach (Candle candle in candles)
                {
                    AddCandle(candle);
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public List<Candle> ToList()
        {
            try
            {
                return _candleDictionary.Values.ToList();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }
    }
}

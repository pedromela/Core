using BrokerLib.Lib;
using BrokerLib.Market;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace BrokerLib.Market
{
    public class MarketData
    {
        public Dictionary<TimeFrames, Candles> TimeFrame2Candles { get; set; }
        private MarketInfo _marketInfo = null;
        public MarketData(MarketInfo marketInfo) 
        {
            _marketInfo = marketInfo;
            TimeFrame2Candles = new Dictionary<TimeFrames, Candles>();
            InitCandles();
        }

        public void InitCandles() 
        {
            try
            {
                foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                {
                    if (TimeFrame2Candles.ContainsKey(timeFrame))
                    {
                        continue;
                    }
                    TimeFrame2Candles.Add(timeFrame, new Candles(_marketInfo._broker, _marketInfo, timeFrame));
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public MarketInfo GetMarketInfo() 
        {
            return _marketInfo;
        }
    }
}

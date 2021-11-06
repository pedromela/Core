using BrokerLib.Brokers;
using BrokerLib.Market;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Indicators
{
    public class MissingCandleInterval
    {
        public DateTime _fromDate;
        public DateTime _toDate;
        public int _index;

        public MissingCandleInterval(DateTime fromDate, DateTime toDate, int index)
        {
            _fromDate = fromDate;
            _toDate = toDate;
            _index = index;
        }
    }
    public class Price : Indicator
    {
        public Price(string ShortName, int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base(ShortName + ":" + Period, Period, TimeFrame, marketInfo, "Price")
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
        }


        public override List<string> CreateLines()
        {
            try
            {
                List<string> lines = base.CreateLines();
                return lines;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public void AddLastCandle(Candle candle) 
        {
            try
            {
                AddLastValue(candle);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }
        public override void Init(Indicator indicator)
        {
            try
            {
                AddValues(indicator.GetValues());
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public override bool CalculateNext(Indicator indicator)
        {
            try
            {
                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }

    }
}

using BrokerLib.Market;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Indicators
{
    class StdDevIndicator : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BB"/> class. 
        /// </summary>
        /// 

        protected MSD msd20;
        protected float _StdDevFactor;

        public StdDevIndicator(string ShortName, int Period, float StdDevFactor, MarketInfo marketInfo, TimeFrames TimeFrame, string Name = null)
        : base(ShortName + ":" + Period + ":" + StdDevFactor, Period, TimeFrame, marketInfo, Name)
        {
            AddArgument("Period");
            AddArgument("StdDevFactor");
            this.ShorDescriptionName = GetShorDescriptionName();
            _StdDevFactor = StdDevFactor;
            msd20 = new MSD(Period, TimeFrame, marketInfo);
        }

        public override List<string> CreateLines()
        {
            try
            {
                List<string> lines = base.CreateLines();
                lines.Add("lower");
                lines.Add("upper");
                return lines;
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
                msd20.Init(indicator);

                float msd = msd20.GetLastClose();
                float sma = msd20.sma20.GetLastClose();

                AddLastValues(sma, msd, indicator.GetLastTimestamp());
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
                if (!base.CalculateNext(indicator))
                {
                    return false;
                }

                msd20.CalculateNext(indicator);

                float msd = msd20.GetLastClose();
                float sma = msd20.sma20.GetLastClose();

                AddLastValues(sma, msd, indicator.GetLastTimestamp());

                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false ;
        }

        public void AddLastValues(float value, float msd, DateTime timestamp)
        {
            try
            {
                Dictionary<string, Candle> valueList = new Dictionary<string, Candle>();
                Candle candle = new Candle();
                candle.Close = value;
                candle.Timestamp = timestamp;
                valueList.Add("middle", candle);

                candle = new Candle();
                candle.Close = value - _StdDevFactor * msd;
                candle.Timestamp = timestamp;
                valueList.Add("lower", candle);

                candle = new Candle();
                candle.Close = value + _StdDevFactor * msd;
                candle.Timestamp = timestamp;
                valueList.Add("upper", candle);

                RemoveLast();
                AddLastValue(valueList);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }
    }
}

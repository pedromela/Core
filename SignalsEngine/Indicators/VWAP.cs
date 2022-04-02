using BrokerLib.Market;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Indicators
{
    class VWAP : IndicatorDayData
    {

        private float CumulativePriceVolume = 0;
        private float CumulativeVolume = 0;
        protected MSD msd20;
        protected float _StdDevFactor;
        /// <summary>
        /// Initializes a new instance of the <see cref="VWAP"/> class. 
        /// </summary>
        public VWAP(float StdDevFactor, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("VWAP", "Volume Weighted Average Price", TimeFrame, marketInfo, true)
        {
            AddArgument("Period");
            AddArgument("StdDevFactor");
            this.ShorDescriptionName = GetShorDescriptionName();
            _StdDevFactor = StdDevFactor;
            Period = 1440 / (int)TimeFrame;
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
            base.Init(indicator);
            var values = indicator.GetValues();
            var lines = indicator.GetLines();
            float VWAP = 0;
            CumulativePriceVolume = 0;
            CumulativeVolume = 0;
            int i = 0;
            Candle candle;
            foreach (var value in values)
            {
                candle = value["middle"];
                float TPV = (candle.Close + candle.Max + candle.Min) / 3;
                CumulativePriceVolume += TPV * candle.Volume;
                CumulativeVolume += candle.Volume;
                VWAP = CumulativeVolume > 0 ? (CumulativePriceVolume / CumulativeVolume) : 0;
                i++;
            }
            AddLastClose(VWAP, indicator.GetLastTimestamp());
            msd20.Init(this);
            float msd = msd20.GetLastClose();

            AddLastValues(VWAP, msd, indicator.GetLastTimestamp());
        }

        public override bool CalculateNext(Indicator indicator)
        {
            try
            {
                if (!base.CalculateNext(indicator))
                {
                    return false;
                }

                Candle candle = indicator.GetLastValue("middle");

                float TPV = (candle.Close + candle.Max + candle.Min) / 3;

                CumulativePriceVolume += TPV * candle.Volume;
                CumulativeVolume += candle.Volume;

                float VWAP = CumulativeVolume > 0 ? (CumulativePriceVolume / CumulativeVolume) : 0;
                AddLastClose(VWAP, indicator.GetLastTimestamp());
                msd20.CalculateNext(this);
                float msd = msd20.GetLastClose();

                AddLastValues(VWAP, msd, indicator.GetLastTimestamp());
                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;

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

        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static float[] Calculate(float[] price, int period)
        {
            var lwma = new float[price.Length];
            float avgsum = 0.0f;
            float sum = 0.0f;
            for (int i = 0; i < period - 1; i++)
            {
                avgsum += price[i] * (i + 1);
                sum += price[i];
            }

            var divider = period * (period + 1) / 2;
            for (int i = period - 1; i < price.Length; i++)
            {
                avgsum += price[i] * period;
                sum += price[i];
                lwma[i] = avgsum / divider;
                avgsum -= sum;
                sum -= price[i - period + 1];
            }

            return lwma;
        }
    }
}

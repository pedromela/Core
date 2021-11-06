// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ATR.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Average True Range Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Linq;
using BrokerLib.Market;
using BrokerLib.Models;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{
    /// <summary>
    /// Average True Range Indicator.
    /// </summary>
    public class ATR : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATR"/> class. 
        /// </summary>
        /// 
        private Lowest low;
        private Highest high;
        //private SMA sma;
        public ATR(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("ATR:" + Period, Period, TimeFrame, marketInfo, "Welles Wilder's Average True Range")
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
            high = new Highest(TimeFrame, marketInfo);
            low = new Lowest(TimeFrame, marketInfo);
            //sma = new SMA(Period);
        }

        public override void Init(Indicator indicator)
        {
            try
            {
                float sum = 0;
                var values = indicator.GetValues();
                var lines = indicator.GetLines();

                high.Init(indicator);
                low.Init(indicator);

                var candle = values.Last;
                int idx = lines["middle"];
                for (int i = 0; i < values.Count && i < Period; i++)
                {
                    var valueList = candle.Value;
                    sum += GetMax(valueList["middle"].Close);
                    candle = candle.Previous;
                }

                if (indicator.Count() < Period)
                {
                    Period = indicator.Count();
                }
                float ma = Period > 0 ? sum / Period : 0;
                AddLastClose(ma, candle.Value.First().Value.Timestamp);
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

                float ma = GetLastClose();

                high.CalculateNext(indicator);
                low.CalculateNext(indicator);
                var candle          = indicator.GetLastValue("middle");
                var Max             = GetMax(candle.Close);
                var MaxMinusPeriod  = GetMax(indicator.GetClose(Period), Period);

                if (indicator.Count() >= Period)
                {
                    ma = ma + (Max - MaxMinusPeriod) / Period;
                }
                else
                {
                    ma = ma + (Max - ma) / indicator.Count();
                }

                AddLastClose(ma, candle.Timestamp);

                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }

        private float GetMax(float candleClose, int lastMinus = 0) 
        {
            var diff1 = lastMinus == 0 ? Math.Abs(candleClose - high.GetLastClose()) : Math.Abs(candleClose - high.GetClose(Period));
            var diff2 = lastMinus == 0 ? Math.Abs(candleClose - low.GetLastClose()) : Math.Abs(candleClose - low.GetClose(Period));
            var diff3 = lastMinus == 0 ? high.GetLastClose() - low.GetLastClose() : high.GetClose(Period) - low.GetClose(Period);

            var max = diff1 > diff2 ? diff1 : diff2;
            var Max = max > diff3 ? max : diff3;
            return Max;
        }

        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="period">Indicator period.</param>
        /// <param name="timeSeries">Instrument <c>ohlc</c> time series.</param>
        /// <returns>Calculated indicator series.</returns>
        //public static float[] Calculate(int period, TimeSeries timeSeries)
        //{
        //    var temp = new float[timeSeries.Close.Length];
        //    temp[0] = 0;

        //    for (var i = 1; i < timeSeries.Close.Length; i++)
        //    {
        //        var diff1 = Math.Abs(timeSeries.Close[i - 1] - timeSeries.High[i]);
        //        var diff2 = Math.Abs(timeSeries.Close[i - 1] - timeSeries.Low[i]);
        //        var diff3 = timeSeries.High[i] - timeSeries.Low[i];

        //        var max = diff1 > diff2 ? diff1 : diff2;
        //        temp[i] = max > diff3 ? max : diff3;
        //    }

        //    var atr = SMA.Calculate(temp, period);

        //    return atr;
        //}
    }
}

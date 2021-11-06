// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BearsPower.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Bears Power Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BotLib.Models;
using BrokerLib.Lib;
using BrokerLib.Market;
using global::SignalsEngine;
using SignalsEngine.Indicators;
using System;
using System.Linq;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{

    /// <summary>
    /// Bears Power Indicator.
    /// </summary>
    public class BearsPower : IndicatorDayData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BearsPower"/> class. 
        /// </summary>
        /// 
        private Lowest low;
        private EMA ema13;

        public BearsPower(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("BearsPower:"+ Period, "Dr. Alexander Elder's Bulls/Bears Power (Elder-Rays)", TimeFrame, marketInfo)
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
            ema13 = new EMA(Period, TimeFrame, marketInfo);
            low = new Lowest(TimeFrame, marketInfo);
        }

        public override void Init(Indicator indicator)
        {
            try
            {
                ema13.Init(indicator);
                low.Init(indicator);
                AddLastClose(low.GetLastClose() - ema13.GetLastClose(), indicator.GetLastTimestamp());
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

                ema13.CalculateNext(indicator);
                low.CalculateNext(indicator);
                AddLastClose(low.GetLastClose() - ema13.GetLastClose(), indicator.GetLastTimestamp());

                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <param name="timeSeries">Instrument <c>ohlc</c> time series.</param>
        /// <returns>Calculated indicator series.</returns>
        //public static float[] Calculate(float[] price, int period, TimeSeries timeSeries)
        //{

        //    var bears = new float[price.Length];

        //    var ema = EMA.Calculate(price, period);
        //    for (var i = 0; i < price.Length; i++)
        //    {
        //        bears[i] = timeSeries.Low[i] - ema[i];
        //    }

        //    return bears;
        //}
    }
}

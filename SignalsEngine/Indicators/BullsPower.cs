// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BullsPower.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Bulls Power Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using global::SignalsEngine;
using BotLib.Models;
using System;
using SignalsEngine.Indicators;
using BrokerLib.Lib;
using static BrokerLib.BrokerLib;
using BrokerLib.Market;

namespace SignalsEngine
{
    /// <summary>
    /// Bulls Power Indicator.
    /// </summary>
    public class BullsPower : IndicatorDayData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BullsPower"/> class. 
        /// </summary>
        /// 
        private EMA ema13;
        private Highest high;

        public BullsPower(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("BullsPower:"+ Period, "Dr. Alexander Elder's Bulls/Bears Power (Elder-Rays)", TimeFrame, marketInfo)
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
            ema13 = new EMA(Period, TimeFrame, marketInfo);
            high = new Highest(TimeFrame, marketInfo);

        }

        public override void Init(Indicator indicator)
        {
            try
            {
                ema13.Init(indicator);
                high.Init(indicator);
                AddLastClose(high.GetLastClose() - ema13.GetLastClose(), indicator.GetLastTimestamp());
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
                high.CalculateNext(indicator);
                AddLastClose(high.GetLastClose() - ema13.GetLastClose(), indicator.GetLastTimestamp());
                
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
        //    var bulls = new float[price.Length];

        //    var ema = EMA.Calculate(price, period);
        //    for (var i = 0; i < price.Length; i++)
        //    {
        //        bulls[i] = timeSeries.High[i] - ema[i];
        //    }

        //    return bulls;
        //}
    }
}

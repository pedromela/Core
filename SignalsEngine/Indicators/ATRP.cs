// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ATRP.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Average True Range Percentage Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{
    /// <summary>
    /// Average True Range Percentage Indicator.
    /// </summary>
    public class ATRP : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATRP"/> class. 
        /// </summary>
        public ATRP(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("ATRP" + Period, Period, TimeFrame, marketInfo, "Average True Range Percentage")
        {
            AddArgument("Period");
        }

        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="period">Indicator period.</param>
        /// <param name="timeSeries">Instrument <c>ohlc</c> time series.</param>
        /// <returns>Calculated indicator series.</returns>
        //public static float[] Calculate(int period, TimeSeries timeSeries)
        //{

        //    var atr = ATR.Calculate(period, timeSeries);
        //    var atrp = new float[atr.Length];
        //    for (var i = period; i < timeSeries.Close.Length; i++)
        //    {
        //        atrp[i] = atr[i] * 100.0f / timeSeries.Close[i];
        //    }

        //    return atrp;
        //}
    }
}

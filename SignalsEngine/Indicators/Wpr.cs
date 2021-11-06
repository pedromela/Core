// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Wpr.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Williams Percent Range Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{
    /// <summary>
    /// Williams Percent Range Indicator.
    /// </summary>
    public class WPR : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WPR"/> class. 
        /// </summary>
        public WPR(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("WPR" + Period, Period, TimeFrame, marketInfo, "Williams Percent Range")
        {
            AddArgument("Period");
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

        //    var wpr = new float[price.Length];

        //    for (var i = period; i < price.Length; i++)
        //    {
        //        var highest = float.MinValue;
        //        var lowest = float.MaxValue;

        //        for (int j = i - period + 1; j <= i; j++)
        //        {
        //            if (timeSeries.High[j] > highest)
        //            {
        //                highest = timeSeries.High[j];
        //            }

        //            if (timeSeries.Low[j] < lowest)
        //            {
        //                lowest = timeSeries.Low[j];
        //            }
        //        }

        //        wpr[i] = -100 * (highest - timeSeries.Close[i]) / (highest - lowest);
        //    }

        //    return wpr;
        //}
    }
}

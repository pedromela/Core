// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BOP.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Balance Of Power Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using global::SignalsEngine;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{
    /// <summary>
    /// Balance Of Power Indicator.
    /// </summary>
    public class BOP : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BOP"/> class. 
        /// </summary>
        public BOP(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("BOP" + Period, Period, TimeFrame, marketInfo, "Balance Of Power")
        {
            AddArgument("Period");
        }

        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="timeSeries">Instrument <c>ohlc</c> time series.</param>
        /// <returns>Calculated indicator series.</returns>
        //public static float[] Calculate(TimeSeries timeSeries)
        //{

        //    var bop = new float[timeSeries.Open.Length];
        //    for (var i = 0; i < timeSeries.Open.Length; i++)
        //    {
        //        if (timeSeries.High[i].AlmostEqual(timeSeries.Low[i]))
        //        {
        //            bop[i] = 0;
        //        }
        //        else
        //        {
        //            bop[i] = (timeSeries.Close[i] - timeSeries.Open[i]) / (timeSeries.High[i] - timeSeries.Low[i]);
        //        }
        //    }

        //    return bop;
        //}
    }
}

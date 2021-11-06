// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LWMA.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Linearly Weighted Moving Average Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{
    /// <summary>
    /// Linearly Weighted Moving Average Indicator.
    /// </summary>
    public class LWMA : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LWMA"/> class. 
        /// </summary>
        public LWMA(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("LWMA" + Period, Period, TimeFrame, marketInfo, "Linearly Weighted Moving Average")
        {
            AddArgument("Period");
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

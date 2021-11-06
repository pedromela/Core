// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Trix.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Triple-smoothed Exponential Moving Average Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{

    /// <summary>
    /// Triple-smoothed Exponential Moving Average Indicator.
    /// </summary>
    public class TRIX : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TRIX"/> class. 
        /// </summary>
        public TRIX(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("TRIX" + Period, Period, TimeFrame, marketInfo, "Triple-smoothed Exponential Moving Average")
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

            var trix = new float[price.Length];
            var ema1 = EMA.Calculate(price, period);
            var ema2 = EMA.Calculate(ema1, period);
            var ema3 = EMA.Calculate(ema2, period);
            trix[0] = 0.0f;
            for (int i = 1; i < price.Length; ++i)
            {
                if (ema3[i].IsAlmostZero())
                {
                    trix[i] = 0.0f;
                }
                else
                {
                    trix[i] = 100.0f * ((ema3[i] - ema3[i - 1]) / ema3[i]);
                }
            }

            return trix;
        }       
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ao.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Awesome Oscillator Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{
    /// <summary>
    /// Awesome Oscillator Indicator.
    /// </summary>
    public class AO : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AO"/> class. 
        /// </summary>
        public AO(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("AO" + Period, Period, TimeFrame, marketInfo, "Bill Williams' Awesome Oscillator")
        {
            AddArgument("Period");
        }

        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <returns>Calculated indicator series.</returns>
        public static float[] Calculate(float[] price)
        {

            var fastSma = SMA.Calculate(price, 5);
            var slowSma = SMA.Calculate(price, 34);
            var ao = new float[price.Length];

            for (int i = 0; i < price.Length; i++)
            {
                ao[i] = fastSma[i] - slowSma[i];
            }

            return ao;
        }
    }
}

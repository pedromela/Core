// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ac.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Accelerator / Decelerator Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{

    /// <summary>
    /// Accelerator / Decelerator Indicator.
    /// </summary>
    public class AC : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AC"/> class. 
        /// </summary>
        public AC(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("AC" + Period, Period, TimeFrame, marketInfo, "Bill Williams' Accelerator/Decelerator oscillator")
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

            var ao = AO.Calculate(price);
            var smaOfAo = SMA.Calculate(ao, 5);
            var ac = new float[price.Length];
            for (var i = 0; i < price.Length; ++i)
            {
                ac[i] = ao[i] - smaOfAo[i];
            }

            return ac;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIR.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Infinite Impulse Response Moving Average Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{
    /// <summary>
    /// Infinite Impulse Response Moving Average Indicator.
    /// </summary>
    public class IIR : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IIR"/> class.
        /// </summary>
        public IIR(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("IIR" + Period, Period, TimeFrame, marketInfo, "John F. Ehlers' Infinite Impulse Response Moving Average")
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

            return FIR.Calculate(price, new float[] { 2, 4, 0, 0, -1 });
        }
    }
}

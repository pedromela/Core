// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FIR.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Finite Impulse Response Moving Average Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{
    
    /// <summary>
    /// Finite Impulse Response Moving Average Indicator.
    /// Weights examples:
    /// <c>Kalman</c> velocity component applied for lag reduction:
    ///   new float[] { 2, 3, 2, 0, -1 }
    ///   new float[] { 2, 3, 4, 3, 1, 0, -1 }
    ///   new float[] { 2, 3, 4, 5, 4, 2, 1, 0, -1 }
    ///   new float[] { 2, 3, 4, 5, 6, 5, 3, 2, 1, 0, -1 }
    ///   new float[] { 2, 3, 4, 5, 6, 7, 6, 4, 3, 2, 1, 0, -1 }
    /// FIR Smoother:
    ///   new float[] { 2, 7, 9, 6, 1, -1, -3 }
    /// IIR Smoother:
    ///   new float[] { 2, 4, 0, 0, -1 }
    /// Other filters:
    ///   new float[] { 1, 2, 1 }
    ///   new float[] { 1, 2, 2, 1 }
    ///   new float[] { 1, 2, 3, 2, 1 }
    ///   new float[] { 1, 2, 3, 3, 2, 1 }
    ///   new float[] { 1, 2, 3, 4, 3, 2, 1 }
    ///   new float[] { 1, 2, 3, 4, 4, 3, 2, 1 }
    ///   new float[] { 1, 2, 3, 4, 5, 4, 3, 2, 1 }
    ///   new float[] { 1, 2, 3, 4, 5, 5, 4, 3, 2, 1 }
    ///   new float[] { 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1 }
    ///   new float[] { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1 }
    /// </summary>
    public class FIR : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FIR"/> class.
        /// </summary>
        public FIR(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("FIR" + Period, Period, TimeFrame, marketInfo, "John F. Ehlers' Finite Impulse Response Moving Average")
        {
            AddArgument("Period");
        }

        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="weights">Indicator weights.</param>
        /// <returns>Calculated indicator series.</returns>
        public static float[] Calculate(float[] price, float[] weights)
        {

            var fir = new float[price.Length];
            float divider = 0.0f;

            for (int i = 0; i < weights.Length; ++i)
            {
                fir[i] = 0;
                divider += weights[i];
            }

            for (int i = weights.Length; i < price.Length; ++i)
            {
                float sum = 0.0f;
                for (int w = 0; w < weights.Length; w++)
                {
                    sum += weights[w] * price[i - w];
                }

                fir[i] = sum / divider;
            }

            return fir;
        }
    }
}

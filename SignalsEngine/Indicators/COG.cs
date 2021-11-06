// --------------------------------------------------------------------------------------------------------------------
// <copyright file="COG.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Center Of Gravity Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{

    /// <summary>
    /// Center Of Gravity Indicator.
    /// </summary>
    public class COG : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="COG"/> class. 
        /// </summary>
        public COG(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("COG" + Period, Period, TimeFrame, marketInfo, "John Ehlers's Center of Gravity oscillator")
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

            var cog = new float[price.Length];
            for (int i = period - 1; i < price.Length; ++i)
            {
                float weightedSum = 0.0f;
                float sum = 0.0f;
                for (int j = 0; j < period; ++j)
                {
                    weightedSum += price[i - period + j + 1] * (period - j);
                    sum += price[i - period + j + 1];
                }

                cog[i] = -weightedSum / sum;
            }

            return cog;
        }
    }
}

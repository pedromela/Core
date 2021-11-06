// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Highest.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Lowest Value Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using BotLib.Models;
using BrokerLib.Market;
using BrokerLib.Models;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{

    /// <summary>
    /// Highest Value Indicator.
    /// </summary>
    public class Highest : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Highest"/> class. 
        /// </summary>
        public Highest(TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("Highest", 0, TimeFrame, marketInfo, "Highest Value Daily")
        {
        }

        public override void Init(Indicator indicator)
        {
            if (indicator != null) 
            {
                var values = indicator.GetValues();
                var lines = indicator.GetLines();
                int idx = lines["middle"];
                foreach (var valueList in values)
                {
                    Candle candle = valueList["middle"];
                    if (Count() == 0 || candle.Close > GetLastClose())
                    {
                        AddLastValue(candle);
                    }
                    else
                    {
                        AddLastValue(GetLastValue());
                    }
                }
            }

        }

        public override bool CalculateNext(Indicator indicator)
        {
            try
            {
                if (!base.CalculateNext(indicator))
                {
                    return false;
                }

                Candle last = indicator.GetLastValue("middle");
                if (last.Close > GetLastClose())
                {
                    AddLastValue(last);
                }
                else
                {
                    AddLastValue(GetLastValue());
                }

                return true;

            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;

        }

        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static float[] Calculate(float[] price, int period)
        {
            var highest = new float[price.Length];
            highest[0] = price[0];
            for (int i = 1; i < period; ++i)
            {
                if (price[i] > highest[i - 1])
                {
                    highest[i] = price[i];
                }
                else
                {
                    highest[i] = highest[i - 1];
                }
            }

            int highestIdx = 0;
            for (int i = period; i < price.Length; ++i)
            {
                float highestHigh = float.MinValue;
                var start = Math.Max(i - period + 1, highestIdx);
                for (int s = start; s <= i; ++s)
                {
                    if (price[s] > highestHigh)
                    {
                        highestHigh = price[s];
                        highestIdx = s;
                    }
                }

                highest[i] = highestHigh;
            }

            return highest;
        }
    }
}

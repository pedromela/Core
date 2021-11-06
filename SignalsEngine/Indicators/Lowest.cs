// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lowest.cs" company="Pedro Mela">
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
    /// Lowest Value Indicator.
    /// </summary>
    public class Lowest : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Lowest"/> class. 
        /// </summary>
        public Lowest(TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("Lowest", 0, TimeFrame, marketInfo, "Lowest Value Daily")
        {

        }

        public override void Init(Indicator indicator)
        {
            if (indicator != null)
            {
                var values = indicator.GetValues();
                var lines = indicator.GetLines();
                foreach (var valueList in values)
                {
                    Candle candle = valueList["middle"];
                    if (Count() == 0  || candle.Close < GetLastClose())
                    {
                        AddLastValue(candle);
                    }
                    else
                    {
                        //var lastValue = GetLastValue();
                        //foreach (var item in lastValue)
                        //{
                        //    item.Value.Timestamp.AddMinutes((int)TimeFrame);
                        //}
                        //AddLastValue(lastValue);
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
                if (last.Close < GetLastClose())
                {
                    AddLastValue(last);
                }
                else
                {
                    //var lastValue = GetLastValue();
                    //foreach (var item in lastValue)
                    //{
                    //    item.Value.Timestamp.AddMinutes((int)TimeFrame);
                    //}
                    //AddLastValue(lastValue);
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

            var lowest = new float[price.Length];
            lowest[0] = price[0];

            for (int i = 1; i < period; ++i)
            {
                if (price[i] < lowest[i - 1])
                {
                    lowest[i] = price[i];
                }
                else
                {
                    lowest[i] = lowest[i - 1];
                }
            }

            int lowestIdx = 0;
            for (int i = period; i < price.Length; ++i)
            {
                float lowestLow = float.MaxValue;
                var start = Math.Max(i - period + 1, lowestIdx);
                for (int s = start; s <= i; ++s)
                {
                    if (price[s] < lowestLow)
                    {
                        lowestLow = price[s];
                        lowestIdx = s;
                    }
                }

                lowest[i] = lowestLow;
            }

            return lowest;
        }
    }
}

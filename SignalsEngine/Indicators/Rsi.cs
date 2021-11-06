// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Rsi.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Relative Strength Index Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BotLib.Models;
using BrokerLib.Lib;
using BrokerLib.Market;
using BrokerLib.Models;
using SignalsEngine.Indicators;
using System;
using System.Linq;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{

    /// <summary>
    /// Relative Strength Index Indicator.
    /// </summary>
    public class RSI : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RSI"/> class. 
        /// </summary>
        public RSI(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("RSI" + Period, Period, TimeFrame, marketInfo, "Relative Strength Index developed by J. Welles Wilder and published in a 1978 book, New Concepts in Technical Trading Systems")
        {
            AddArgument("Period");
        }


        public override void Init(Indicator indicator)
        {


            float gain = 0.0f;
            float loss = 0.0f;
            DateTime timestamp = indicator.GetLastTimestamp().AddMinutes((int) indicator.TimeFrame * indicator.Count() * -1);
            // first RSI value
            AddLastClose(0.0f, timestamp);
            for (int i = 1; i <= Period; ++i)
            {
                Candle last = indicator.ValueAt(i, "middle");
                Candle previous = indicator.ValueAt(i-1, "middle");
                var diff = last.Close - previous.Close;
                if (diff >= 0)
                {
                    gain += diff;
                }
                else
                {
                    loss -= diff;
                }
            }

            float avrg = gain / Period;
            float avrl = loss / Period;
            float rs = gain / loss;
            float baseRs = rs;
            for (int i = Period + 1; i < indicator.Count(); ++i)
            {
                Candle last = indicator.ValueAt(i, "middle");
                Candle previous = indicator.ValueAt(i - 1, "middle");
                var diff = last.Close - previous.Close;

                if (diff >= 0)
                {
                    avrg = ((avrg * (Period - 1)) + diff) / Period;
                    avrl = (avrl * (Period - 1)) / Period;
                }
                else
                {
                    avrl = ((avrl * (Period - 1)) - diff) / Period;
                    avrg = (avrg * (Period - 1)) / Period;
                }

                rs = avrg / avrl;
                timestamp.AddMinutes((int)indicator.TimeFrame);
                AddLastClose(100 - (100 / (1 + rs)), timestamp);
            }
            timestamp.AddMinutes((int)indicator.TimeFrame);
            AddLastClose(100 - (100 / (1 + baseRs)), timestamp);
        }

        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static float[] Calculate(float[] price, int period)
        {

            var rsi = new float[price.Length];

            float gain = 0.0f;
            float loss = 0.0f;

            // first RSI value
            rsi[0] = 0.0f;
            for (int i = 1; i <= period; ++i)
            {
                var diff = price[i] - price[i - 1];
                if (diff >= 0)
                {
                    gain += diff;
                }
                else
                {
                    loss -= diff;
                }
            }

            float avrg = gain / period;
            float avrl = loss / period;
            float rs = gain / loss;
            rsi[period] = 100 - (100 / (1 + rs));

            for (int i = period + 1; i < price.Length; ++i)
            {
                var diff = price[i] - price[i - 1];

                if (diff >= 0)
                {
                    avrg = ((avrg * (period - 1)) + diff) / period;
                    avrl = (avrl * (period - 1)) / period;
                }
                else
                {
                    avrl = ((avrl * (period - 1)) - diff) / period;
                    avrg = (avrg * (period - 1)) / period;
                }

                rs = avrg / avrl;

                rsi[i] = 100 - (100 / (1 + rs));
            }

            return rsi;
        }
    }
}

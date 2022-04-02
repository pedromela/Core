// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sma.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Simple Moving Average Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using SignalsEngine.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{
    /// <summary>
    /// Simple Moving Average Indicator.
    /// </summary>
    public class SMA : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SMA"/> class. 
        /// </summary>
        public SMA(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("SMA:"+Period, Period, TimeFrame, marketInfo, "Simple Moving Average")
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
        }

        public override void Init(Indicator indicator)
        {
            try
            {
                base.Init(indicator);
                float sum = 0;
                var values = indicator.GetValues();
                var lines = indicator.GetLines();

                var value = values.Last;
                int idx = lines["middle"];
 
                for (int i = 0; i < values.Count && i < Period; i++)
                {
                    var valueList = value.Value;
                    sum += valueList["middle"].Close;
                    value = value.Previous;
                }
                int auxPeriod = Period;
                if (values.Count < auxPeriod)
                {
                    auxPeriod = values.Count;
                }
                float ma = auxPeriod > 0 ? sum / auxPeriod : 0;
                AddLastClose(ma, indicator.GetLastTimestamp());
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
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

                float ma = GetLastClose();
                if (indicator.Count() >= Period)
                {
                    float lastMinusPeriod = indicator.ValueAt(indicator.Count() - Period, "middle").Close;
                    ma = ma + (indicator.GetLastClose() - lastMinusPeriod) / Period;
                }
                else
                {
                    ma = ma + (indicator.GetLastClose() - ma) / indicator.Count();
                }

                AddLastClose(ma, indicator.GetLastTimestamp());

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
            var sma = new float[price.Length];

            float sum = 0;

            for (var i = 0; i < period; i++)
            {
                sum += price[i];
                sma[i] = sum / (i + 1);
            }

            for (var i = period; i < price.Length; i++)
            {
                sum = 0;
                for (var j = i; j > i - period; j--)
                {
                    sum += price[j];
                }

                sma[i] = sum / period;
            }

            return sma;
        }
    }
}

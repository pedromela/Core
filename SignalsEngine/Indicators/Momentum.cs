// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Momentum.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Momentum Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using BrokerLib.Models;
using SignalsEngine.Indicators;
using System;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{

    /// <summary>
    /// Momentum Indicator.
    /// </summary>
    public class Momentum : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Momentum"/> class. 
        /// </summary>
        /// 

        public Momentum(int Period, TimeFrames TimeFrame, MarketInfo marketInfo, string InputName)
        : base("MOM:" + Period + ";" + InputName, Period, TimeFrame, marketInfo, "Momentum")
        {
            this.InputName = InputName;
            AddArgument("Period");
            ShorDescriptionName = GetShorDescriptionName();
        }
        public Momentum(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("MOM:" + Period, Period, TimeFrame, marketInfo, "Momentum")
        {
            AddArgument("Period");
            ShorDescriptionName = GetShorDescriptionName();
        }

        public override void Init(Indicator indicator)
        {
            try
            {
                int idxPeriod = indicator.Count() - Period - 1;
                if (idxPeriod < 0)
                {
                    idxPeriod = 0;
                }
                Candle candleLast = indicator.GetLastValue("middle");
                Candle candlePedriod = indicator.ValueAt(idxPeriod, "middle");
                float mom = candleLast.Close - candlePedriod.Close;
                AddLastClose(mom, indicator.GetLastTimestamp());
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

                int idxPeriod = indicator.Count() - Period - 1;
                if (idxPeriod < 0)
                {
                    idxPeriod = 0;
                }
                Candle candleLast = indicator.GetLastValue("middle");
                Candle candlePedriod = indicator.ValueAt(idxPeriod, "middle");
                float mom = candleLast.Close - candlePedriod.Close;
                AddLastClose(mom, indicator.GetLastTimestamp());
                return true;
            }
            catch (System.Exception e)
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
            var momentum = new float[price.Length];
            for (int i = 0; i < period; i++)
            {
                momentum[i] = 0;
            }

            for (int i = period; i < price.Length; i++)
            {
                momentum[i] = price[i] * 100 / price[i - period];
            }

            return momentum;
        }
    }
}

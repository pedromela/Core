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

namespace SignalsEngine.Indicators
{
    /// <summary>
    /// Momentum Indicator.
    /// </summary>
    public class DailyChange : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Momentum"/> class. 
        /// </summary>
        /// 

        public DailyChange(TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("DC", 0, TimeFrame, marketInfo, "Daily Change", false, false, true)
        {
            switch (TimeFrame)
            {
                case TimeFrames.M1:
                    Period = 1440;
                    break;
                case TimeFrames.M5:
                    Period = 1440/5;
                    break;
                case TimeFrames.M15:
                    Period = 1440/15;
                    break;
                case TimeFrames.M30:
                    Period = 1440 / 30;
                    break;
                case TimeFrames.H1:
                    Period = 1440 / 60;
                    break;
                case TimeFrames.D1:
                    Period = 1;
                    break;
                default:
                    break;
            }
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
                float dailychange = (candleLast.Close - candlePedriod.Close) / candlePedriod.Close * 100;
                AddLastClose(dailychange, indicator.GetLastTimestamp());
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
                float dailychange = (candleLast.Close - candlePedriod.Close) / candlePedriod.Close * 100;
                AddLastClose(dailychange, indicator.GetLastTimestamp());
                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }
    }
}

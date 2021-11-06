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
    public class SMMA : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SMA"/> class. 
        /// </summary>
        ///
        public SMMA(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("SMMA:" + Period, Period, TimeFrame, marketInfo, "Smoothed Moving Average")
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
        }

        public override void Init(Indicator indicator)
        {
            try
            {
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
                float prevsum = ma * Period;

                if (indicator.Count() >= Period)
                {
                    float lastMinusPeriod = indicator.ValueAt(indicator.Count() - Period, "middle").Close;
                    ma = (prevsum - ma + indicator.GetLastClose()) / Period;
                }
                else
                {
                    prevsum = ma * indicator.Count();
                    ma = (prevsum - ma + indicator.GetLastClose()) / indicator.Count();
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
    }
}

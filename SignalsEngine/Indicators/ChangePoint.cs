// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangePoint.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Change Point Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;
using static UtilsLib.Utils.ChangePointBase;

namespace SignalsEngine.Indicators
{
    public class ChangePoint : Indicator
    {
        public ChangePoint(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("CP:" + Period, Period, TimeFrame, marketInfo, "Change Point Indicator")
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
        }

        public override void Init(Indicator indicator)
        {
            //var values = indicator.GetValues();
            var lastCandleNode = indicator.GetLastValueNode();
            var list = new List<TimeSeriesData>();
            for (int i = 0; i < Period; i++)
            {
                if (lastCandleNode == null || lastCandleNode.Previous == null)
                {
                    break;
                }
                list.Add(new TimeSeriesData(lastCandleNode.Value["middle"].Close));
                lastCandleNode = lastCandleNode.Previous;
            }
            list.Reverse();
            float pvalue = ChangePointBase.GetLastChangePoint(list);

            AddLastValues(pvalue, indicator.GetLastTimestamp());
        }

        public override bool CalculateNext(Indicator indicator)
        {
            try
            {
                if (!base.CalculateNext(indicator))
                {
                    return false;
                }

                var lastCandleNode = indicator.GetLastValueNode();
                var list = new List<TimeSeriesData>();
                for (int i = 0; i < Period; i++)
                {
                    if (lastCandleNode == null || lastCandleNode.Previous == null)
                    {
                        break;
                    }
                    list.Add(new TimeSeriesData(lastCandleNode.Value["middle"].Close));
                    lastCandleNode = lastCandleNode.Previous;
                }
                list.Reverse();
                float pvalue = ChangePointBase.GetLastChangePoint(list);

                AddLastValues(pvalue, indicator.GetLastTimestamp());
                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;

        }

        public void AddLastValues(float pvalue, DateTime timestamp)
        {
            try
            {
                Dictionary<string, Candle> valueList = new Dictionary<string, Candle>();
                Candle candle = new Candle();
                candle.Close = pvalue;
                candle.Timestamp = timestamp;
                valueList.Add("middle", candle);
                AddLastValue(valueList);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }
    }
}

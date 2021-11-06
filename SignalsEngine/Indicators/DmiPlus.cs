// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DmiPlus.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Directional Movement Index Plus Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using BrokerLib.Models;
using global::SignalsEngine;
using SignalsEngine.Indicators;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{

    /// <summary>
    /// Directional Movement Index Plus Indicator.
    /// </summary>
    public class DmiPlus : EMA
    {
        private Highest highest;
        private Lowest lowest;
        /// <summary>
        /// Initializes a new instance of the <see cref="DmiPlus"/> class. 
        /// </summary>
        public DmiPlus(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("DMI+" + Period, Period, TimeFrame, marketInfo, "Directional Movement Index Plus")
        {
            AddArgument("Period");
            highest = new Highest(TimeFrame, marketInfo);
            lowest = new Lowest(TimeFrame, marketInfo);
        }

        public override List<string> CreateLines()
        {
            try
            {
                List<string> lines = base.CreateLines();
                lines.Add("aux");
                return lines;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public override void Init(Indicator indicator)
        {
            try
            {
                highest.Init(indicator);
                lowest.Init(indicator);

                var highnode = highest.GetLastValueNode();
                var lownode = lowest.GetLastValueNode();
                var pricenode = indicator.GetLastValueNode();

                var prevhighnode = highnode.Previous;
                var prevlownode = lownode.Previous;
                var prevpricenode = pricenode.Previous;

                var plusDm = highnode.Value["middle"].Close - prevhighnode.Value["middle"].Close;
                var minusDm = prevlownode.Value["middle"].Close - lownode.Value["middle"].Close;

                if (plusDm < 0)
                {
                    plusDm = 0;
                }

                if (minusDm < 0)
                {
                    minusDm = 0;
                }

                if (plusDm.AlmostEqual(minusDm))
                {
                    minusDm = 0;
                }
                else if (plusDm >= minusDm)
                {
                    minusDm = 0;
                }

                var trueHigh = highnode.Value["middle"].Close > prevpricenode.Value["middle"].Close ?
                                highnode.Value["middle"].Close : prevpricenode.Value["middle"].Close;
                var trueLow = lownode.Value["middle"].Close < prevpricenode.Value["middle"].Close ?
                                lownode.Value["middle"].Close : prevpricenode.Value["middle"].Close;

                var tr = trueHigh - trueLow;
                var mdm = 0f;
                if (tr.IsAlmostZero())
                {
                    mdm = 0;
                }
                else
                {
                    mdm = 100.0f * minusDm / tr;
                }
                Dictionary<string, Candle> valueList = new Dictionary<string, Candle>();
                Candle candle = new Candle();
                candle.Close = mdm;
                candle.Timestamp = indicator.GetLastTimestamp();
                valueList.Add("aux", candle);

                candle = new Candle();
                candle.Close = mdm;
                candle.Timestamp = indicator.GetLastTimestamp();
                valueList.Add("middle", candle);

                AddLastValue(valueList);
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
                if (!base.Validate(indicator))
                {
                    return false;
                }
                lowest.CalculateNext(indicator);
                highest.CalculateNext(indicator);

                var highnode = highest.GetLastValueNode();
                var lownode = lowest.GetLastValueNode();
                var pricenode = indicator.GetLastValueNode();

                var prevhighnode = highnode.Previous;
                var prevlownode = lownode.Previous;
                var prevpricenode = pricenode.Previous;

                var plusDm = highnode.Value["middle"].Close - prevhighnode.Value["middle"].Close;
                var minusDm = prevlownode.Value["middle"].Close - lownode.Value["middle"].Close;

                if (plusDm < 0)
                {
                    plusDm = 0;
                }

                if (minusDm < 0)
                {
                    minusDm = 0;
                }

                if (plusDm.AlmostEqual(minusDm))
                {
                    minusDm = 0;
                }
                else if (plusDm >= minusDm)
                {
                    minusDm = 0;
                }

                var trueHigh = highnode.Value["middle"].Close > prevpricenode.Value["middle"].Close ?
                                highnode.Value["middle"].Close : prevpricenode.Value["middle"].Close;
                var trueLow = lownode.Value["middle"].Close < prevpricenode.Value["middle"].Close ?
                                lownode.Value["middle"].Close : prevpricenode.Value["middle"].Close;

                var tr = trueHigh - trueLow;
                var mdm = 0f;
                if (tr.IsAlmostZero())
                {
                    mdm = 0;
                }
                else
                {
                    mdm = 100.0f * minusDm / tr;
                }
                Dictionary<string, Candle> valueList = new Dictionary<string, Candle>();
                Candle candle = new Candle();
                candle.Close = mdm;
                candle.Timestamp = indicator.GetLastTimestamp();
                valueList.Add("aux", candle);

                candle = base.CalculateNext(this, "aux", "middle");
                valueList.Add("middle", candle);

                AddLastValue(valueList);

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
        /// <param name="timeSeries">Instrument <c>ohlc</c> time series.</param>
        /// <returns>Calculated indicator series.</returns>
        //public static float[] Calculate(float[] price, int period, TimeSeries timeSeries)
        //{
        //    var mdm = new float[price.Length];
        //    mdm[0] = 0.0f;

        //    for (int i = 1; i < price.Length; ++i)
        //    {
        //        var plusDm = timeSeries.High[i] - timeSeries.High[i - 1];
        //        var minusDm = timeSeries.Low[i - 1] - timeSeries.Low[i];

        //        if (plusDm < 0)
        //        {
        //            plusDm = 0;
        //        }

        //        if (minusDm < 0)
        //        {
        //            minusDm = 0;
        //        }

        //        if (plusDm.AlmostEqual(minusDm))
        //        {
        //            minusDm = 0;
        //        }
        //        else if (plusDm >= minusDm)
        //        {
        //            minusDm = 0;
        //        }

        //        var trueHigh = timeSeries.High[i] > price[i - 1] ? timeSeries.High[i] : price[i - 1];
        //        var trueLow = timeSeries.Low[i] < price[i - 1] ? timeSeries.Low[i] : price[i - 1];
        //        var tr = trueHigh - trueLow;
        //        if (tr.IsAlmostZero())
        //        {
        //            mdm[i] = 0;
        //        }
        //        else
        //        {
        //            mdm[i] = 100 * minusDm / tr;
        //        }
        //    }

        //    var dmi = EMA.Calculate(mdm, period);

        //    return dmi;
        //}
    }
}

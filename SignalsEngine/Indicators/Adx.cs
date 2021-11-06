// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Adx.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Average Directional Movement Index Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using BrokerLib.Market;
using BrokerLib.Models;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{

    /// <summary>
    /// Average Directional Movement Index Indicator.
    /// </summary>
    public class ADX : EMA
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ADX"/> class. 
        /// </summary>
        /// 
        private DmiMinus dmiminus;
        private DmiPlus dmiplus;

        public ADX(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("ADX" + Period, Period, TimeFrame, marketInfo, "Welles Wilder' Average Directional Movement Index")
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();

            dmiminus = new DmiMinus(Period, TimeFrame, marketInfo);
            dmiplus = new DmiPlus(Period, TimeFrame, marketInfo);
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
                dmiminus.Init(indicator);
                dmiplus.Init(indicator);


                var highnode = dmiplus.GetLastValueNode();
                var lownode = dmiminus.GetLastValueNode();
                var pricenode = indicator.GetLastValueNode();
                var pDi = highnode.Value["middle"].Close;
                var mDi = lownode.Value["middle"].Close;
                var diff = pDi + mDi;
                var dx = 0f;
                if (diff.IsAlmostZero())
                {
                    dx = 0;
                }
                else
                {
                    dx = 100 * (Math.Abs(pDi - mDi) / (pDi + mDi));
                }

                Dictionary<string, Candle> valueList = new Dictionary<string, Candle>();
                Candle candle = new Candle();
                candle.Close = dx;
                candle.Timestamp = indicator.GetLastTimestamp();
                valueList.Add("aux", candle);

                candle = new Candle();
                candle.Close = dx;
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
                dmiminus.CalculateNext(indicator);
                dmiplus.CalculateNext(indicator);

                var highnode = dmiplus.GetLastValueNode();
                var lownode = dmiminus.GetLastValueNode();
                var pricenode = indicator.GetLastValueNode();
                var pDi = highnode.Value["middle"].Close;
                var mDi = lownode.Value["middle"].Close;
                var diff = pDi + mDi;
                var dx = 0f;
                if (diff.IsAlmostZero())
                {
                    dx = 0;
                }
                else
                {
                    dx = 100 * (Math.Abs(pDi - mDi) / (pDi + mDi));
                }

                Dictionary<string, Candle> valueList = new Dictionary<string, Candle>();
                Candle candle = new Candle();
                candle.Close = dx;
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
        //    var dx = new float[price.Length];
        //    var pDi = DmiPlus.Calculate(price, period, timeSeries);
        //    var mDi = DmiMinus.Calculate(price, period, timeSeries);
        //    for (var i = 0; i < price.Length; ++i)
        //    {
        //        var diff = pDi[i] + mDi[i];
        //        if (diff.IsAlmostZero())
        //        {
        //            dx[i] = 0;
        //        }
        //        else
        //        {
        //            dx[i] = 100 * (Math.Abs(pDi[i] - mDi[i]) / (pDi[i] + mDi[i]));
        //        }
        //    }

        //    var adx = EMA.Calculate(dx, period);

        //    return adx;
        //}
    }
}

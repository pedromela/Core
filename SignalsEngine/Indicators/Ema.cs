// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ema.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Exponential Moving Average Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using BrokerLib.Models;
using SignalsEngine.Indicators;
using System;
using System.Linq;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{

    /// <summary>
    /// Exponential Moving Average Indicator.
    /// </summary>
    public class EMA : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EMA"/> class. 
        /// </summary>
        /// 
        public EMA(string ShortName, int Period, TimeFrames TimeFrame, MarketInfo marketInfo, string Name = null, bool AllowInconsistentData = false, bool Store = false, bool Special = false)
        : base(ShortName, Period, TimeFrame, marketInfo, Name, AllowInconsistentData, Store, Special)
        {

        }


        public EMA(int Period, string ShortName, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base(ShortName + ":" + Period, Period, TimeFrame, marketInfo, "Exponential Moving Average")
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
        }

        public EMA(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("EMA:" + Period, Period, TimeFrame, marketInfo, "Exponential Moving Average")
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
        }

        public override void Init(Indicator indicator)
        {
            try
            {
                float a = 2.0f / (Period + 1);

                int len = Period > indicator.Count() ? 0 : indicator.Count() - Period;

                float ema = indicator.ValueAt(len, "middle").Close;
                for (int i = len + 1; i < indicator.Count(); i++)
                {
                    ema += a * (indicator.ValueAt(i, "middle").Close - ema);
                }
                AddLastClose(ema, indicator.GetLastTimestamp());
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public override void Init(Indicator indicator, string inLine = "middle", string outLine = "middle")
        {
            try
            {
                float a = 2.0f / (Period + 1);
                int len = Period > indicator.Count() ? 0 : indicator.Count() - Period;

                float ema = indicator.ValueAt(len, inLine).Close;
                for (int i = len + 1; i < indicator.Count(); i++)
                {
                    ema += a * (indicator.ValueAt(i, "middle").Close - ema);
                }
                AddCurrentClose(ema, indicator.GetLastTimestamp(), outLine);
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

                float ema = GetLastClose();
                float a = 2.0f / (Period + 1);

                if (Count() >= Period)
                {
                    ema = ema + (indicator.GetLastClose() - ema) * a;
                }
                else
                {
                    int len = Period > indicator.Count() ? 0 : indicator.Count() - Period;

                    ema = indicator.ValueAt(len, "middle").Close;
                    for (int i = indicator.Count() - 1; i > len; i--)
                    {
                        ema = a * indicator.ValueAt(i, "middle").Close + (1 - a) * ema;
                    }
                }
                AddLastClose(ema, indicator.GetLastTimestamp());

                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }

        public virtual Candle CalculateNext(Indicator indicator, string inLine, string outLine)
        {
            try
            {
                //if (!base.CalculateNext(indicator, inLine, outLine))
                //{
                //    return false;
                //}

                float ema = GetLastClose(outLine);
                float a = 2.0f / (Period + 1);

                if (Count() >= Period)
                {
                    ema = ema + (indicator.GetLastClose(inLine) - ema) * a;
                }
                else
                {
                    int len = Period > indicator.Count() ? 0 : indicator.Count() - Period;

                    ema = indicator.ValueAt(len, inLine).Close;
                    for (int i = indicator.Count() - 1; i > len; i--)
                    {
                        ema = a * indicator.ValueAt(i, inLine).Close + (1 - a) * ema;
                    }
                }
                Candle candle = new Candle();
                candle.Close = ema;
                candle.Timestamp = indicator.GetLastTimestamp();

                return candle;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public void CalculatePrevious(Indicator indicator, int number)
        {
            try
            {
                float ema = 0;
                float a = 2.0f / (Period + 1);

                for (int j = number; j > 0; j--)
                {
                    if (indicator.Count() >= number * 2)
                    {
                        ema = indicator.ValueAt(indicator.Count() - number - j, "middle").Close;
                        for (int i = indicator.Count() - number - j + 1; i < indicator.Count() - j; i++)
                        {
                            ema += a * (indicator.ValueAt(i, "middle").Close - ema);
                        }
                        AddLastClose(ema, indicator.GetLastTimestamp());
                    }
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void CalculatePrevious(Indicator indicator, int number, string inLine = "middle", string outLine = "middle")
        {
            try
            {
                float ema = 0;
                float a = 2.0f / (Period + 1);

                for (int j = number; j > 0; j--)
                {
                    if (indicator.Count() >= number * 2)
                    {
                        ema = indicator.ValueAt(indicator.Count() - number - j, inLine).Close;
                        for (int i = indicator.Count() - number - j + 1; i < indicator.Count() - j; i++)
                        {
                            ema += a * (indicator.ValueAt(i, inLine).Close - ema);
                        }
                        AddLastClose(ema, indicator.GetLastTimestamp());
                    }
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static float[] Calculate(float[] price, int period)
        {
            var ema = new float[price.Length];
            float sum = price[0];
            float coeff = 2.0f / (1.0f + period);

            for (int i = 0; i < price.Length; i++)
            {
                sum += coeff * (price[i] - sum);
                ema[i] = sum;
            }

            return ema;
        }
    }
}

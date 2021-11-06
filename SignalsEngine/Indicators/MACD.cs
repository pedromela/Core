using BrokerLib.Market;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;


namespace SignalsEngine.Indicators
{
    class MACD : EMA
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SMA"/> class. 
        /// </summary>
        /// 

        public EMA ema12;
        public EMA ema26;


        public MACD(int MACDLen, int MACDEMALenBottom, int MACDEMALenUpper, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("MACD:"+MACDLen+":"+MACDEMALenBottom+":"+MACDEMALenUpper, MACDLen, TimeFrame, marketInfo, "Moving Average Convergence Divergence", true, false, true)
        {
            AddArgument("MACDLen");
            AddArgument("MACDEMALenBottom");
            AddArgument("MACDEMALenUpper");
            this.ShorDescriptionName = GetShorDescriptionName();

            ema12 = new EMA(MACDEMALenBottom, "MACDEMA12", TimeFrame, marketInfo);
            ema26 = new EMA(MACDEMALenUpper, "MACDEMA26", TimeFrame, marketInfo);

        }

        public override void Init(Indicator indicator)
        {
            try
            {
                ema12.CalculatePrevious(indicator, Period);
                ema26.CalculatePrevious(indicator, Period);
                var node12 = ema12.GetFirstValueNode();
                var node26 = ema26.GetFirstValueNode();
                Dictionary<string, Candle> valueList;
                Candle candle;
                for (; node12 != null; )
                {
                    valueList = new Dictionary<string, Candle>();
                    candle = new Candle();
                    candle.Close = node12.Value["middle"].Close - node26.Value["middle"].Close;
                    candle.Timestamp = node12.Value["middle"].Timestamp;
                    AddLastValue(candle, "middle");
                    node12 = node12.Next;
                    node26 = node26.Next;
                }

                base.Init(this, "middle", "signal");

                valueList = new Dictionary<string, Candle>();
                candle = new Candle();
                candle.Close = GetLastClose("middle") - GetLastClose("signal");
                candle.Timestamp = indicator.GetLastTimestamp();
                AddCurrentValue(candle, "histogram");

            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public override List<string> CreateLines()
        {
            try
            {
                List<string> lines = base.CreateLines();
                lines.Add("signal");
                lines.Add("histogram");
                return lines;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public override bool CalculateNext(Indicator indicator)
        {
            try
            {
                if (!Validate(indicator, "middle", "middle", false))
                {
                    return false;
                }
                ema12.CalculateNext(indicator);
                ema26.CalculateNext(indicator);

                if (Count() > Period)
                {
                    RemoveFirst();
                }
                Dictionary<string, Candle> valueList = new Dictionary<string, Candle>();
                Candle candle = new Candle();
                candle.Close = ema12.GetLastClose() - ema26.GetLastClose();
                candle.Timestamp = indicator.GetLastTimestamp();
                valueList.Add("middle", candle);

                candle = base.CalculateNext(this, "middle", "signal");
                valueList.Add("signal", candle);

                candle = new Candle();
                candle.Close = GetLastClose("middle") - GetLastClose("signal");
                candle.Timestamp = indicator.GetLastTimestamp();
                valueList.Add("histogram", candle);

                AddLastValue(valueList);
                Candle lastCandle = indicator.GetLastValue("middle");
                StoreLinesCurrentValues(lastCandle);

                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }

        //public void AddLastValues(float emadiff, DateTime timestamp)
        //{
        //    try
        //    {
        //        Dictionary<string, Candle> valueList = new Dictionary<string, Candle>();
        //        Candle candle = new Candle();
        //        candle.Close = emadiff;
        //        candle.Timestamp = timestamp;
        //        AddLastValue(candle, "middle");

        //        candle = new Candle();
        //        candle.Close = macd - signal;
        //        candle.Timestamp = timestamp;
        //        valueList.Add("histogram", candle);

        //        AddLastValue(valueList);
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //}

        //public float SignalLine()
        //{
        //    try
        //    {
        //        float a = 1 / Period;
        //        float ema = 0;
        //        if (Count() >= Period)
        //        {
        //            ema = CalculateEMAMACD(Period, a);
        //        }
        //        else
        //        {
        //            ema = CalculateEMAMACD(Count(), a);
        //        }
        //        return ema;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return 0.0f;

        //}

        //public float CalculateEMAMACD(int period, float a)
        //{
        //    try
        //    {
        //        if (Count() > 0)
        //        {
        //            float ema = ValueAt(Count() - period, "middle").Close;
        //            for (int i = Count() - 1; i > Count() - period; i--)
        //            {
        //                ema = a * ValueAt(i, "middle").Close + (1.0f - a) * ema;
        //            }
        //            return ema;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return 0.0f;

        //}
    }
}

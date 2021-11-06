using BotLib.Models;
using SignalsEngine.Indicators;
using System;
using static BrokerLib.BrokerLib;
using BrokerLib.Models;
using UtilsLib.Utils;
using BrokerLib.Market;

namespace SignalsEngine
{
    public class SignalsEngine : Engine
    {
        public readonly Brokers _broker;
        private readonly BotDBContext _context;

        public bool MyStrategy = true;
        public bool VWAPMA200Strategy = false;
        public bool VWAPMA200Strategy2 = false;
        public bool VWAPMA200InverseStrategy = false;
        public bool MACDStrategy = false;

        public bool MACrossOverStrategy = false;
        public bool WaitingForAnotherCrossOverBuy = false;
        public bool WaitingForMAGapBuy = false;
        public bool WaitingForAnotherCrossOverSell = false;
        public bool WaitingForMAGapSell = false;
        public bool EMAOnTop = false;
        public Candle CrossOverStartPriceBuy = null;
        public Candle CrossOverEndPriceBuy = null;
        public Candle CrossOverStartPriceSell = null;
        public Candle CrossOverEndPriceSell = null;

        public IndicatorsEngine indicatorsEngine;

        public SignalsEngine(BotDBContext context, Brokers broker, MarketInfo marketInfo) 
        {
            _context = context;
            _broker = broker;
            indicatorsEngine = new IndicatorsEngine(broker, marketInfo);
        }

        public SignalsEngine(BotDBContext context, Brokers broker, MarketInfo marketInfo, DateTime fromDate, DateTime toDate, TimeFrames timeFrame)
        {
            _context = context;
            _broker = broker;
            indicatorsEngine = new IndicatorsEngine(broker, marketInfo, fromDate, toDate, timeFrame);
        }
        public bool IsMarketClosed() 
        {
            try
            {
                return indicatorsEngine.IsMarketClosed();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return true;
        }

        public Candle GetCurrentCandle(TimeFrames timeFrame)
        {
            return indicatorsEngine.GetCurrentCandle(timeFrame);
        }

        public Candle GetCurrentCandle()
        {
            return indicatorsEngine.GetCurrentCandle(TimeFrames.M1);
        }

        public bool GreenCandle(Candle candle) 
        {
            if (candle.Close - candle.Open > 0)
            {
                return true;
            }
            return false;
        }

        public bool RedCandle(Candle candle)
        {
            if (candle.Close - candle.Open <= 0)
            {
                return true;
            }
            return false;
        }

        public bool ReversalCandlesBuy(Candle previous, Candle last) 
        {
            if (GreenCandle(last) && RedCandle(previous))// if last is green candle and previous is red
            {
                return true;
            }
            return false;
        }

        public bool ReversalCandlesSell(Candle previous, Candle last)
        {
            if (RedCandle(last) && GreenCandle(previous))// if last is red candle and previous is green
            {
                return true;
            }
            return false;
        }

        public bool ValueBetweenRedCandle(Candle candle, float value) 
        {
            if (GreenCandle(candle))//green candle
            {
                return false;
            }

            if (candle.Min < value && candle.Max > value)
            {
                return true;
            }

            return false;
        }

        public bool ValueBetweenGreenCandle(Candle candle, float value)
        {
            if (RedCandle(candle))//red candle
            {
                return false;
            }

            if (candle.Min > value && candle.Max < value)
            {
                return true;
            }

            return false;
        }

        public bool ValueLesserRedCandle(Candle candle, float value)
        {
            if (candle.Open > value)
            {
                return true;
            }

            return false;
        }

        public bool ValueLesserGreenCandle(Candle candle, float value)
        {
            if (candle.Close > value)
            {
                return true;
            }

            return false;
        }

        public bool ValueGreaterGreenCandle(Candle candle, float value)
        {
            if (candle.Open < value)
            {
                return true;
            }

            return false;
        }

        public bool ValueGreaterRedCandle(Candle candle, float value)
        {
            if (candle.Close < value)
            {
                return true;
            }

            return false;
        }

        //public bool DoubleBollingerBandsBuyStrategy(TimeFrames timeFrame)
        //{
        //    try
        //    {
        //        Indicator indicator20 = indicatorsEngine.GetIndicator("BB:20:2", timeFrame);
        //        Indicator indicator200 = indicatorsEngine.GetIndicator("BB:200:2", timeFrame);
        //        BB bollingerBands20 = null;
        //        BB bollingerBands200 = null;

        //        if (indicator20 is BB && indicator200 is BB)
        //        {
        //            bollingerBands20 = (BB)indicator20;
        //            bollingerBands200 = (BB)indicator200;
        //        }
        //        else
        //        {
        //            return false;
        //        }

        //        //float upperValue = bollingerBands.GetLastUpperValue();
        //        //float value = bollingerBands.GetLastValue();
        //        float bottomValue200 = bollingerBands200.GetLastBottomValue();
        //        var candlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame);
        //        var previouscandlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame).Previous;

        //        if ((ValueGreaterGreenCandle(candlenode.Value, bottomValue200) || ValueGreaterRedCandle(previouscandlenode.Value, bottomValue200)) && ReversalCandlesBuy(previouscandlenode.Value, candlenode.Value))
        //        {
        //            float bottomValue20 = bollingerBands20.GetLastBottomValue();
        //            if ((ValueGreaterGreenCandle(candlenode.Value, bottomValue20) || ValueGreaterRedCandle(previouscandlenode.Value, bottomValue20)) && ReversalCandlesBuy(previouscandlenode.Value, candlenode.Value))
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;
        //}

        //public bool DoubleBollingerBandsSellStrategy(TimeFrames timeFrame)
        //{
        //    try
        //    {
        //        Indicator indicator20 = indicatorsEngine.GetIndicator("BB:20:2", timeFrame);
        //        Indicator indicator200 = indicatorsEngine.GetIndicator("BB:200:2", timeFrame);
        //        BB bollingerBands20 = null;
        //        BB bollingerBands200 = null;

        //        if (indicator20 is BB && indicator200 is BB)
        //        {
        //            bollingerBands20 = (BB)indicator20;
        //            bollingerBands200 = (BB)indicator200;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //        float upperValue200 = bollingerBands200.GetLastUpperValue();
        //        //float value = bollingerBands.GetLastValue();
        //        //float bottomValue = bollingerBands.GetLastBottomValue();
        //        var candlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame);
        //        var previouscandlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame).Previous;
        //        if ((ValueLesserRedCandle(candlenode.Value, upperValue200) || ValueLesserGreenCandle(previouscandlenode.Value, upperValue200)) && ReversalCandlesSell(previouscandlenode.Value, candlenode.Value))
        //        {
        //            float upperValue20 = bollingerBands20.GetLastUpperValue();
        //            if ((ValueLesserRedCandle(candlenode.Value, upperValue20) || ValueLesserGreenCandle(previouscandlenode.Value, upperValue20)) && ReversalCandlesSell(previouscandlenode.Value, candlenode.Value))
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;
        //}

        //public bool BollingerBandsBuyStrategy(TimeFrames timeFrame)
        //{
        //    try
        //    {
        //        Indicator indicator200 = indicatorsEngine.GetIndicator("BB:200:2", timeFrame);
        //        BB bollingerBands200 = null;

        //        if (indicator200 is BB)
        //        {
        //            bollingerBands200 = (BB) indicator200;
        //        }
        //        else
        //        {
        //            return false;
        //        }

        //        //float upperValue = bollingerBands.GetLastUpperValue();
        //        //float value = bollingerBands.GetLastValue();
        //        float bottomValue = bollingerBands200.GetLastBottomValue();
        //        var candlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame);
        //        var previouscandlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame).Previous;

        //        if ((ValueGreaterGreenCandle(candlenode.Value, bottomValue) || ValueGreaterRedCandle(previouscandlenode.Value, bottomValue)) && ReversalCandlesBuy(previouscandlenode.Value, candlenode.Value))
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;
        //}

        //public bool BollingerBandsSellStrategy(TimeFrames timeFrame)
        //{
        //    try
        //    {
        //        Indicator indicator200 = indicatorsEngine.GetIndicator("BB:200:2", timeFrame);
        //        BB bollingerBands200 = null;

        //        if (indicator200 is BB)
        //        {
        //            bollingerBands200 = (BB)indicator200;
        //        }
        //        else
        //        {
        //            return false;
        //        }

        //        float upperValue = bollingerBands200.GetLastUpperValue();
        //        //float value = bollingerBands.GetLastValue();
        //        //float bottomValue = bollingerBands.GetLastBottomValue();
        //        var candlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame);
        //        var previouscandlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame).Previous;
        //        if ((ValueLesserRedCandle(candlenode.Value, upperValue) || ValueLesserGreenCandle(previouscandlenode.Value, upperValue)) && ReversalCandlesSell(previouscandlenode.Value, candlenode.Value))
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;
        //}


        //public float VWAPMA200BuyStrategy2(TimeFrames timeFrame, Candle last, float Percentage)
        //{
        //    try
        //    {
        //        Indicator VWAP = indicatorsEngine.GetIndicator("VWAP", TimeFrames.M1);
        //        Indicator SMA200 = indicatorsEngine.GetIndicator("SMA:200", timeFrame);

        //        float targetDifference = last.Close * Percentage;
        //        float differenceVWAP = last.Close - VWAP.GetLastValue();
        //        float differenceMA200 = last.Close - SMA200.GetLastValue();
        //        var candlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame);
        //        var previouscandlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame).Previous;
        //        float timesTargetDifference = (differenceMA200 + differenceVWAP) / (2 * targetDifference);
        //        if (timesTargetDifference < -1 && ReversalCandlesBuy(previouscandlenode.Value, candlenode.Value))
        //        {
        //            return -timesTargetDifference;
        //        }

        //        return 0;

        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return 0;

        //}

        //public bool VWAPMA200BuyStrategy(TimeFrames timeFrame, Candle last, float Percentage)
        //{
        //    try
        //    {
        //        //Indicator VWAP = indicatorsEngine.GetIndicator("VWAP", TimeFrames.M1);
        //        //Indicator SMA200 = indicatorsEngine.GetIndicator("SMA200", timeFrame);

        //        //if (last.Close * (1 + Percentage) < VWAP.GetLastValue() && last.Close < SMA200.GetLastValue())
        //        //{
        //        //    return true;
        //        //}

        //        //return false;
        //        Indicator VWAP = indicatorsEngine.GetIndicator("VWAP", TimeFrames.M1);
        //        Indicator SMA200 = indicatorsEngine.GetIndicator("SMA:200", timeFrame);

        //        float targetDifference = last.Close * Percentage;
        //        float differenceVWAP = last.Close - VWAP.GetLastValue();
        //        float differenceMA200 = last.Close - SMA200.GetLastValue();
        //        var candlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame);
        //        var previouscandlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame).Previous;
        //        float timesTargetDifference = (differenceMA200 + differenceVWAP) / (2 * targetDifference);
        //        if (timesTargetDifference < -1 && ReversalCandlesBuy(previouscandlenode.Value, candlenode.Value))
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;

        //}
        //public bool VWAPMA200SellStrategy(TimeFrames timeFrame, Candle last, float Percentage)
        //{
        //    try
        //    {
        //        Indicator VWAP = indicatorsEngine.GetIndicator("VWAP", TimeFrames.M1);
        //        Indicator SMA200 = indicatorsEngine.GetIndicator("SMA:200", timeFrame);

        //        float targetDifference = last.Close * Percentage;
        //        float differenceVWAP = -last.Close + VWAP.GetLastValue();
        //        float differenceMA200 = -last.Close + SMA200.GetLastValue();
        //        var candlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame);
        //        var previouscandlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame).Previous;
        //        float timesTargetDifference = (differenceMA200 + differenceVWAP) / (2 * targetDifference);
        //        if (timesTargetDifference < -1 && ReversalCandlesSell(previouscandlenode.Value, candlenode.Value))
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;
        //}

        //public bool VWAPMA200InverseBuyStrategy(TimeFrames timeFrame, Candle last, float Percentage)
        //{
        //    try
        //    {
        //        Indicator VWAP = indicatorsEngine.GetIndicator("VWAP", TimeFrames.M1);
        //        Indicator SMA200 = indicatorsEngine.GetIndicator("SMA:200", timeFrame);

        //        float targetDifference = last.Close * Percentage;
        //        float differenceVWAP = -last.Close + VWAP.GetLastValue();
        //        float differenceMA200 = -last.Close + SMA200.GetLastValue();
        //        var candlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame);
        //        var previouscandlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame).Previous;
        //        float timesTargetDifference = (differenceMA200 + differenceVWAP) / (2 * targetDifference);
        //        if (timesTargetDifference < -1 && ReversalCandlesSell(previouscandlenode.Value, candlenode.Value))
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;
        //}

        //public bool VWAPMA200InverseSellStrategy(TimeFrames timeFrame, Candle last, float Percentage)
        //{
        //    try
        //    {
        //        Indicator VWAP = indicatorsEngine.GetIndicator("VWAP", TimeFrames.M1);
        //        Indicator SMA200 = indicatorsEngine.GetIndicator("SMA:200", timeFrame);

        //        float targetDifference = last.Close * Percentage;
        //        float differenceVWAP = -last.Close + VWAP.GetLastValue();
        //        float differenceMA200 = -last.Close + SMA200.GetLastValue();
        //        var candlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame);
        //        var previouscandlenode = indicatorsEngine.GetCurrentCandleNode(timeFrame).Previous;
        //        float timesTargetDifference = (differenceMA200 + differenceVWAP) / (2 * targetDifference);
        //        if (timesTargetDifference < -1 && ReversalCandlesSell(previouscandlenode.Value, candlenode.Value))
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;
        //}

        //public bool MACDBuyStrategy(TimeFrames timeFrame)
        //{
        //    try
        //    {
        //        Indicator EMA12 = indicatorsEngine.GetIndicator("EMA:12", timeFrame);
        //        Indicator EMA26 = indicatorsEngine.GetIndicator("EMA:26", timeFrame);
        //        Indicator MACDEMA9 = indicatorsEngine.GetIndicator("MACD:9:12:26", timeFrame);

        //        float MACDvalue = EMA12.GetLastValue() - EMA26.GetLastValue();
        //        float SIGNALvalue = MACDEMA9.GetLastValue();
        //        if (MACDvalue - SIGNALvalue > 0)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;
        //}

        //public bool MACDSellStrategy(TimeFrames timeFrame)
        //{
        //    try
        //    {
        //        Indicator EMA12 = indicatorsEngine.GetIndicator("EMA:12", timeFrame);
        //        Indicator EMA26 = indicatorsEngine.GetIndicator("EMA:26", timeFrame);
        //        Indicator MACDEMA9 = indicatorsEngine.GetIndicator("MACD:9:12:26", timeFrame);

        //        float MACDvalue = EMA12.GetLastValue() - EMA26.GetLastValue();
        //        float SIGNALvalue = MACDEMA9.GetLastValue();
        //        if (MACDvalue - SIGNALvalue < 0)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }

        //    return false;
        //}
        //public bool MyBuyStrategy(TimeFrames timeFrame, Candle last, float Percentage, int Period)
        //{
        //    try
        //    {
        //        Candle max = indicatorsEngine.GetMaxCandle(timeFrame, Period);
        //        float difference = max.Close - last.Close;
        //        float DecreaseTarget = Percentage * max.Close;
        //        if (difference > DecreaseTarget)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        if (SignalsEngine.Verbose)
        //            Console.WriteLine(e);
        //        if (SignalsEngine.Logging)
        //            SignalsEngine.log.Error(e);
        //    }
        //    return false;
        //}

        //public bool MA20EMA20CrossOverBuyStrategy(TimeFrames timeFrame, Candle last, float Percentage)
        //{
        //    try
        //    {
        //        Indicator EMA20 = indicatorsEngine.GetIndicator("EMA:20", timeFrame);
        //        Indicator MA20 = indicatorsEngine.GetIndicator("SMA:20", timeFrame);

        //        double m = 0;
        //        if (MACrossOver(timeFrame))
        //        {
        //            if (EMA20.GetLastValue() <= MA20.GetLastValue() && !WaitingForAnotherCrossOverBuy)
        //            {
        //                CrossOverStartPriceBuy = last;
        //                WaitingForAnotherCrossOverBuy = true;
        //            }
        //            else if (EMA20.GetLastValue() > MA20.GetLastValue())
        //            {
        //                if (WaitingForAnotherCrossOverBuy)
        //                {
        //                    CrossOverEndPriceBuy = last;
        //                    m = (CrossOverEndPriceBuy.Close - CrossOverStartPriceBuy.Close) / (CrossOverEndPriceBuy.Timestamp - CrossOverStartPriceBuy.Timestamp).TotalMinutes;
        //                    if (m < -0.1)
        //                    {
        //                        WaitingForMAGapBuy = true;
        //                        CrossOverStartPriceBuy = last;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (WaitingForMAGapBuy)
        //            {
        //                if (EMA20.GetLastValue() > MA20.GetLastValue() * (1 + Percentage / 3.0))
        //                {
        //                    CrossOverEndPriceBuy = last;
        //                    m = (CrossOverEndPriceBuy.Close - CrossOverStartPriceBuy.Close) / ((CrossOverEndPriceBuy.Timestamp - CrossOverStartPriceBuy.Timestamp).TotalMinutes);
        //                    if (m > 0)
        //                    {
        //                        WaitingForMAGapBuy = false;
        //                        WaitingForAnotherCrossOverBuy = false;
        //                        return true;
        //                    }
        //                }
        //            }
        //        }

        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;
        //}

        //public bool MA20EMA20CrossOverSellStrategy(TimeFrames timeFrame, Candle last, float Percentage)
        //{
        //    try
        //    {
        //        float EMA20 = indicatorsEngine.GetIndicator("EMA:20", timeFrame).GetLastValue();
        //        float MA20 = indicatorsEngine.GetIndicator("SMA:20", timeFrame).GetLastValue();

        //        double m = 0;
        //        if (MACrossOver(timeFrame))
        //        {
        //            if (EMA20 >= MA20 && !WaitingForAnotherCrossOverSell)
        //            {
        //                CrossOverStartPriceSell = last;
        //                WaitingForAnotherCrossOverSell = true;
        //            }
        //            else if (EMA20 < MA20)
        //            {
        //                if (WaitingForAnotherCrossOverSell)
        //                {
        //                    CrossOverEndPriceSell = last;
        //                    m = (CrossOverEndPriceSell.Close - CrossOverStartPriceSell.Close) / ((CrossOverEndPriceSell.Timestamp - CrossOverStartPriceSell.Timestamp).TotalMinutes);
        //                    if (m > 0.1)
        //                    {
        //                        WaitingForMAGapSell = true;
        //                        CrossOverStartPriceSell = last;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (WaitingForMAGapSell)
        //            {
        //                if (EMA20 < MA20 * (1 - Percentage / 4.0))
        //                {
        //                    CrossOverEndPriceSell = last;
        //                    m = (CrossOverEndPriceSell.Close - CrossOverStartPriceSell.Close) / ((CrossOverEndPriceSell.Timestamp - CrossOverStartPriceSell.Timestamp).TotalMinutes);
        //                    if (m < 0)
        //                    {
        //                        WaitingForMAGapSell = false;
        //                        WaitingForAnotherCrossOverSell = false;
        //                        return true;
        //                    }
        //                }
        //            }
        //        }

        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;
        //}

        //public bool MACrossOver(TimeFrames timeFrame)
        //{
        //    try
        //    {
        //        bool result = false;
        //        Indicator MA20 = indicatorsEngine.GetIndicator("SMA:20", timeFrame);
        //        Indicator EMA20 = indicatorsEngine.GetIndicator("EMA:20", timeFrame);

        //        if (MA20.GetLastValue() > EMA20.GetLastValue())
        //        {
        //            if (EMAOnTop)
        //            {
        //                result = true;
        //            }
        //            EMAOnTop = false;
        //        }
        //        else
        //        {
        //            if (!EMAOnTop)
        //            {
        //                result = true;
        //            }
        //            EMAOnTop = true;
        //        }
        //        //if (MA20 > EMA20*(1-UpPercentage) && MA20 < EMA20*(1+UpPercentage))
        //        //{
        //        //    return true;
        //        //}
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return false;
        //}

        //public float GetIndicatorValue(string Name, TimeFrames TimeFrame)
        //{
        //    return indicatorsEngine.GetIndicator(Name, TimeFrame).GetLastValue();
        //}

        //public void ProcessIndicatorsAtDate(TimeFrames timeframe, DateTime date) 
        //{
        //    try
        //    {
        //        //indicatorsEngine.StartTimeSeries();
        //        indicatorsEngine.ProcessAtDate(timeframe, date);
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //}

        /////////////////////////////////////////////////////////////////////
        ////////////////////////// STATIC FUNCTIONS /////////////////////////
        /////////////////////////////////////////////////////////////////////
        //public static string DecideSignalsEngineId(Brokers BrokerId, string Market)
        //{
        //    try
        //    {
        //        if (BrokerId == Brokers.HitBTCMargin)
        //        {
        //            BrokerId = Brokers.HitBTC;
        //        }
        //        if (Market.Contains("_"))
        //        {
        //            Market = Market.Replace("_", "");
        //        }
        //        return BrokerId.ToString() + ":" + Market;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return null;
        //}

    }

}

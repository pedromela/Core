using BotLib;
using BotLib.Models;
using BrokerLib.Models;
using SignalsEngine.Indicators;
using System;
using System.Collections.Generic;
using System.Text;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Signals
{
    class VWAPMA200Signal : Signal
    {
        private float _buyPercentage = 0;
        private float _sellPercentage = 0;

        public VWAPMA200Signal(float buyPercentage, float sellPercentage)
        : base("VWAPMA200Signal")
        {
            _buyPercentage = buyPercentage;
            _sellPercentage = sellPercentage;
        }


        //public override SignalState ProcessNext(IndicatorsEngine indicatorsEngine, TimeFrames timeFrame, Candle last)
        //{
        //    try
        //    {
        //        Indicator VWAP = indicatorsEngine.GetIndicator("VWAP", timeFrame);
        //        Indicator SMA200 = indicatorsEngine.GetIndicator("SMA200", timeFrame);

        //        if (last.Close * (1 + _buyPercentage) < VWAP.GetLastValue() && last.Close < SMA200.GetLastValue())
        //        {
        //            return SignalState.Buy;
        //        }

        //        if (last.Close * (1 - _sellPercentage) > VWAP.GetLastValue() && last.Close > SMA200.GetLastValue())
        //        {
        //            return SignalState.Sell;
        //        }

        //        return SignalState.DoNothing;
        //    }
        //    catch (Exception e)
        //    {
        //        SignalsEngine.DebugMessage(e);
        //    }
        //    return SignalState.DoNothing;
        //}

        //public bool VWAPMA200BuyStrategy(IndicatorsEngine indicatorsEngine, TimeFrames timeFrame, Candle last, float Percentage)
        //{
        //    Indicator VWAP = indicatorsEngine.GetIndicator("VWAP", timeFrame);
        //    Indicator SMA200 = indicatorsEngine.GetIndicator("SMA200", timeFrame);

        //    if (last.Close * (1 + Percentage) < VWAP.GetLastValue() && last.Close < SMA200.GetLastValue())
        //    {
        //        return true;
        //    }

        //    return false;

        //}
        //public bool VWAPMA200SellStrategy(IndicatorsEngine indicatorsEngine, TimeFrames timeFrame, Candle last, float Percentage)
        //{
        //    Indicator VWAP = indicatorsEngine.GetIndicator("VWAP", timeFrame);
        //    Indicator SMA200 = indicatorsEngine.GetIndicator("SMA200", timeFrame);

        //    if (last.Close * (1 - Percentage) > VWAP.GetLastValue() && last.Close > SMA200.GetLastValue())
        //    {
        //        return true;
        //    }

        //    return false;

        //}
    }
}

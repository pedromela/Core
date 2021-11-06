using BotLib;
using BotLib.Models;
using BrokerLib.Lib;
using BrokerLib.Market;
using System;
using System.Collections.Generic;
using System.Text;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Indicators
{
    public class BullishBearish : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BullsPower"/> class. 
        /// </summary>
        /// 
        private EMA ema200;

        public BullishBearish(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("BullishBearish:" + Period, Period, TimeFrame, marketInfo, "Custom BullishBearish")
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
            ema200 = new EMA(Period, TimeFrame, marketInfo);
        }

        public override void Init(Indicator indicator)
        {
            try
            {
                ema200.Init(indicator);//CalculatePrevious(timeSeries, Period);

                AddLastClose(ema200.Slope()*(indicator.GetLastClose() - ema200.GetLastClose()) / ema200.GetLastClose(), indicator.GetLastTimestamp());
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

                ema200.CalculateNext(indicator);
                AddLastClose(ema200.Slope() * (indicator.GetLastClose() - ema200.GetLastClose()), indicator.GetLastTimestamp());

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

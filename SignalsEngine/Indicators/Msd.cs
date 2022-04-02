using BotLib.Models;
using BrokerLib.Lib;
using BrokerLib.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Indicators
{
    class MSD : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MSD"/> class. 
        /// </summary>
        /// 
        public SMA sma20;

        public MSD(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("MSD" + Period, Period, TimeFrame, marketInfo, "Moving Standard Deviation")
        {
            AddArgument("Period");
            sma20 = new SMA(Period, TimeFrame, marketInfo);
        }

        public override void Init(Indicator indicator)
        {
            try
            {
                base.Init(indicator);
                sma20.Init(indicator);
                Calculate(indicator);
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

                sma20.CalculateNext(indicator);
                Calculate(indicator);
                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }

        private float Calculate(Indicator indicator) 
        {
            float ma = sma20.GetLastClose();

            float sumSquared = 0;
            var candle = indicator.GetLastValueNode();

            for (int i = 0; i < indicator.Count() && i < Period; i++)
            {
                sumSquared += (candle.Value["middle"].Close - ma) * (candle.Value["middle"].Close - ma);
                candle = candle.Previous;
            }
            int auxPeriod = Period;
            if (indicator.Count() < auxPeriod)
            {
                auxPeriod = indicator.Count();
            }
            float msd = auxPeriod > 0 ? sumSquared / auxPeriod : 0;
            AddLastClose(MathF.Sqrt(msd), indicator.GetLastTimestamp());
            return msd;

        }
    }
}

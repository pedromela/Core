using BrokerLib.Market;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Indicators
{
    class BB : StdDevIndicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BB"/> class. 
        /// </summary>
        /// 

        public BB(int Period, float StdDevFactor, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("BB", Period, StdDevFactor, marketInfo, TimeFrame, "Bollinger Bands")
        {
        }

    }
}

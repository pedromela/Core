using BrokerLib.Market;
using System;
using System.Collections.Generic;
using System.Text;
using static BrokerLib.BrokerLib;
using System.Reactive.Linq;

namespace SignalsEngine.Indicators
{
    public class CompoundIndicator : Indicator
    {
        List<IObservable<Indicator>> indicators = new List<IObservable<Indicator>>();
        public CompoundIndicator(string ShortName, int Period, TimeFrames TimeFrame, MarketInfo marketInfo, string Name = null, bool AllowInconsistentData = false, bool Store = false, bool Special = false)
        : base(ShortName, Period, TimeFrame, marketInfo, Name, AllowInconsistentData, Store, Special)
        {

        }
    }
}

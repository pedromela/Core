


using BrokerLib.Market;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Indicators
{
    public class IndicatorDayData : Indicator
    {
        public IndicatorDayData(string shortName, string name, TimeFrames timeFrame, MarketInfo marketInfo, bool AllowInconsistentData = false)
        : base(shortName, 0, timeFrame, marketInfo, name, AllowInconsistentData)
        {

        }

    }
}

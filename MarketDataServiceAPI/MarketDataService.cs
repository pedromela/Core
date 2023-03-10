using BrokerLib.Brokers;
using BrokerLib.Market;
using SignalsEngine.Indicators;
using System.Collections.Generic;

namespace MarketDataServiceAPI
{
    public class MarketDataService
    {
        protected Dictionary<string, IndicatorsEngine> _indicatorsEngineDict = null;

        public MarketDataService(Dictionary<Broker, List<MarketInfo>> activeBrokerMarketsDict) 
        {
            _indicatorsEngineDict = new Dictionary<string, IndicatorsEngine>();
            IndicatorsSharedData.InitInstance(activeBrokerMarketsDict);
        }
    }
}

using BrokerLib.Brokers;
using BrokerLib.Market;
using System.Collections.Generic;
using UtilsLib.Utils;

namespace MarketDataClient
{
    public class MarketDataClient
    {
        public MarketDataClient(string host, Dictionary<Broker, List<MarketInfo>> activeBrokerMarketsDict) 
        {
            var response = Request.Post(host, null, );

        }
    }
}

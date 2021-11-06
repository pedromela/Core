using UtilsLib.Utils;

namespace BrokerLib.Models
{
    public class BrokerBulkStore : BulkStore
    {
        private static BrokerBulkStore _instance;

        private BrokerBulkStore(DBContextProvider[] providers) : base(providers) 
        {
        }

        public static BrokerBulkStore Instance => _instance ?? (_instance = new BrokerBulkStore(BrokerDBContext.providers));
    }
}

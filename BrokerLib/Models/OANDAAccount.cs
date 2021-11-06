using System;
using System.Collections.Generic;
using System.Text;

namespace BrokerLib.Models
{
    public class OANDAAccount
    {
        public string accountId { get; set; }
        public string accountName { get; set; }
        public string balance { get; set; }
        public string unrealizedPl { get; set; }
        public string realizedPl { get; set; }
        public string marginUsed { get; set; }
        public string marginAvail { get; set; }
        public string openTrades { get; set; }
        public string openOrders { get; set; }
        public string marginRate { get; set; }
        public string accountCurrency { get; set; }


    }
}

using BrokerLib.Models;
using System.Collections.Generic;

namespace BrokerLib.Lib
{
    public class CandleComparer : IComparer<Candle>
    {
        public int Compare(Candle x, Candle y)
        {
            return x.Timestamp.CompareTo(y.Timestamp);
        }
    }
}

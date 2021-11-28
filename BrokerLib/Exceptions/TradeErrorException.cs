using System;

namespace BrokerLib.Exceptions
{
    public class TradeErrorException : Exception
    {
        public TradeErrorException(string message) : base(message)
        {

        }
    }
}

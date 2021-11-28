using System;

namespace BrokerLib.Exceptions
{
    public class MarginAccountNotFoundException : Exception
    {
        public MarginAccountNotFoundException(string message) : base(message)
        {
        }
    }
}

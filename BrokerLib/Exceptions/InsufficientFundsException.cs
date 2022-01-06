using System;

namespace BrokerLib.Exceptions
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException(string message) : base(message)
        {
                    
        }
    }
}

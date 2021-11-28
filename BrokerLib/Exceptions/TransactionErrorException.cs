using System;

namespace BrokerLib.Exceptions
{
    public class TransactionErrorException : Exception
    {
        public TransactionErrorException(string message) : base(message)
        {

        }
    }
}

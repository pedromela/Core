using BrokerLib.Models;
using SignalsEngine.Signals;
using System;
using System.Collections.Generic;
using System.Text;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Strategys
{
    class SignalStrategy : IStrategy
    {
        public string _name = null;
        public List<Signal> _signals = new List<Signal>();

        public SignalStrategy(string name)
        {
            _name = name;
        }

        public virtual void AddSignal() 
        {

        }

        public float CalculateFitness(TransactionType transactionType, bool invertedStrategy)
        {
            try
            {

            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return 0;
        }
    }
}

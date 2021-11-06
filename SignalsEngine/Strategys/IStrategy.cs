using static BrokerLib.BrokerLib;

namespace SignalsEngine.Strategys
{
    interface IStrategy
    {
        public float CalculateFitness(TransactionType transactionType, bool invertedStrategy);
    }
}

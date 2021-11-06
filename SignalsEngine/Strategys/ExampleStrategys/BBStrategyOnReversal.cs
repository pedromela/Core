using BrokerLib.Lib;
using BrokerLib.Market;
using SignalsEngine.Conditions;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Strategys.ExampleStrategys
{
    class BBStrategyOnReversal : ConditionStrategy
    {
        public BBStrategyOnReversal(MarketInfo marketInfo, TimeFrames timeFrame)
        : base("Bolinger Bands Strategy Example OnReversal", marketInfo, timeFrame)
        {
            AddConditions();
        }

        public BBStrategyOnReversal()
        : base("Bolinger Bands Strategy Example OnReversal", null, TimeFrames.H1)
        {
            AddConditions();
        }

        public override void AddConditions()
        {
            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            TextCondition textCondition = new TextCondition(_marketInfo, "i_price:200_middle < i_BB:200:2_lower and b_onreversal", transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
            textCondition = new TextCondition(_marketInfo, "i_price:200_middle > i_BB:200:2_upper and b_onreversal", transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sell;
            textCondition = new TextCondition(_marketInfo, "i_price:200_middle > i_BB:200:2_upper and b_onreversal", transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
            textCondition = new TextCondition(_marketInfo, "i_price:200_middle < i_BB:200:2_lower and b_onreversal", transactionType, _timeFrame);
            AddCondition(textCondition);
        }
    }
}

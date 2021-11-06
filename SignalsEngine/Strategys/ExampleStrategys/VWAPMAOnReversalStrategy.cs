using BrokerLib.Market;
using SignalsEngine.Conditions;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Strategys.ExampleStrategys
{
    public class VWAPMAOnReversalStrategy : ConditionStrategy
    {
        public VWAPMAOnReversalStrategy(MarketInfo marketInfo, TimeFrames timeFrame)
        : base("VWAP MA OnReversal Strategy Example", marketInfo, timeFrame)
        {
            AddConditions();
        }

        public VWAPMAOnReversalStrategy()
        : base("VWAP MA OnReversal Strategy Example", null, TimeFrames.H1)
        {
            AddConditions();
        }

        public override void AddConditions()
        {
            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            TextCondition textCondition = new TextCondition(_marketInfo, "i_price:200_middle < i_VWAP_middle and i_price:200_middle < i_SMA:200_middle and b_onreversal", transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
            textCondition = new TextCondition(_marketInfo, "i_price:200_middle > i_VWAP_middle and i_price:200_middle > i_SMA:200_middle and b_onreversal", transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sell;
            textCondition = new TextCondition(_marketInfo, "i_price:200_middle > i_VWAP_middle and i_price:200_middle > i_SMA:200_middle  and b_onreversal", transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
            textCondition = new TextCondition(_marketInfo, "i_price:200_middle < i_VWAP_middle and i_price:200_middle < i_SMA:200_middle  and b_onreversal", transactionType, _timeFrame);
            AddCondition(textCondition);
        }
    }
}

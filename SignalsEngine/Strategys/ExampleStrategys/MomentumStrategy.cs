using BrokerLib.Lib;
using BrokerLib.Market;
using SignalsEngine.Conditions;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Strategys.ExampleStrategys
{
    public class MomentumStrategy : ConditionStrategy
    {
        public MomentumStrategy(MarketInfo marketInfo, TimeFrames timeFrame)
        : base("Momentum Strategy Example", marketInfo, timeFrame)
        {
            AddConditions();
        }

        public MomentumStrategy()
        : base("Momentum Strategy Example", null, TimeFrames.H1)
        {
            AddConditions();
        }


        public override void AddConditions()
        {
            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            TextCondition textCondition = new TextCondition(_marketInfo, "i_MOM:12_middle > 0 and i_MOM:1_middle;i_MOM:12_middle > 0", transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
            textCondition = new TextCondition(_marketInfo, "i_MOM:12_middle < 0 and i_MOM:1_middle;i_MOM:12_middle < 0", transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sell;
            textCondition = new TextCondition(_marketInfo, "i_MOM:12_middle < 0 and i_MOM:1_middle;i_MOM:12_middle < 0", transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
            textCondition = new TextCondition(_marketInfo, "i_MOM:12_middle > 0 and i_MOM:1_middle;i_MOM:12_middle > 0", transactionType, _timeFrame);
            AddCondition(textCondition);
        }
    }
}

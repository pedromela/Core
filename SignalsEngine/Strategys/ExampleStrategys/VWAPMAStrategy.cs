using BrokerLib.Market;
using SignalsEngine.Conditions;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Strategys.ExampleStrategys
{
    public class VWAPMAStrategy : ConditionStrategy
    {
        public float _percentage = 0;
        public VWAPMAStrategy(MarketInfo marketInfo, TimeFrames timeFrame)
        : base("VWAP MA Strategy Example", marketInfo, timeFrame)
        {
            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            TextCondition textCondition = new TextCondition(marketInfo, "i_price:200_middle < i_VWAP_middle and i_price:200_middle < i_SMA:200_middle", transactionType, timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
            textCondition = new TextCondition(marketInfo, "i_price:200_middle > i_VWAP_middle and i_price:200_middle > i_SMA:200_middle", transactionType, timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sell;
            textCondition = new TextCondition(marketInfo, "i_price:200_middle > i_VWAP_middle and i_price:200_middle > i_SMA:200_middle", transactionType, timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
            textCondition = new TextCondition(marketInfo, "i_price:200_middle < i_VWAP_middle and i_price:200_middle < i_SMA:200_middle", transactionType, timeFrame);
            AddCondition(textCondition);
        }

        public VWAPMAStrategy(MarketInfo marketInfo, TimeFrames timeFrame, float percentage)
        : base(string.Format("VWAP MA Percentage {0} Strategy Example", percentage), marketInfo, timeFrame)
        {
            _percentage = percentage;
            AddConditions();
        }


        public VWAPMAStrategy(float percentage) // For initialization of database
        : base("VWAP MA Percentage Strategy Example", null, TimeFrames.H1)
        {
            _percentage = percentage;
            AddConditions();
        }

        public override void AddConditions()
        {
            int percentageplus = (int) (10000.0f + _percentage * 10000.0f);
            int percentageminus = (int)(10000.0f - _percentage * 10000.0f);

            string conditionBuy = string.Format("i_price:200_middle < ( i_VWAP_middle + i_SMA:200_middle ) * ( {0} )/ 20000", percentageminus.ToString());
            string conditionSell = string.Format("i_price:200_middle > ( i_VWAP_middle + i_SMA:200_middle ) * ( {0} ) / 20000", percentageplus.ToString());

            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            TextCondition textCondition = new TextCondition(_marketInfo, conditionBuy, transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
            textCondition = new TextCondition(_marketInfo, conditionSell, transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sell;
            textCondition = new TextCondition(_marketInfo, conditionSell, transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
            textCondition = new TextCondition(_marketInfo, conditionBuy, transactionType, _timeFrame);
            AddCondition(textCondition);
        }
    }
}

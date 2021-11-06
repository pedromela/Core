using BrokerLib.Market;
using SignalsEngine.Conditions;
using System;
using static BrokerLib.BrokerLib;


namespace SignalsEngine.Strategys.ExampleStrategys
{
    public class ParabolicSARStrategy : ConditionStrategy
    {
        public int Period { get; set; }
        public ParabolicSARStrategy(MarketInfo marketInfo, TimeFrames timeFrame)
        : base("Parabolic SAR Strategy Example", marketInfo, timeFrame)
        {
            this.Period = 200;
            AddConditions();
        }

        public ParabolicSARStrategy(int Period)
        : base(String.Format("Parabolic SAR Strategy Example"), null, TimeFrames.H1)
        {
            this.Period = Period;
            AddConditions();
        }


        public override void AddConditions()
        {
            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            TextCondition textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle crossup i_PSAR:200_middle", Period), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle crossdown i_PSAR:200_middle", Period), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sell;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle crossdown i_PSAR:200_middle", Period), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle crossup i_PSAR:200_middle", Period), transactionType, _timeFrame);
            AddCondition(textCondition);
        }
    }

}

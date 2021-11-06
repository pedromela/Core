using BrokerLib.Lib;
using BrokerLib.Market;
using SignalsEngine.Conditions;
using System;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Strategys.ExampleStrategys
{
    public class BBStrategy : ConditionStrategy
    {
        public int Period { get; set; }
        public float StdDevFactor { get; set; }
        public BBStrategy(MarketInfo marketInfo, TimeFrames timeFrame) 
        : base("Bolinger Bands Strategy Example", marketInfo, timeFrame)
        {
            this.Period = 200;
            this.StdDevFactor = 2;
            AddConditions();
        }

        public BBStrategy(int Period, float StdDevFactor)
        : base(String.Format("Bolinger Bands {0}:{1} Strategy Example", Period, StdDevFactor), null, TimeFrames.H1)
        {
            this.Period = Period;
            this.StdDevFactor = StdDevFactor;
            AddConditions();
        }


        public override void AddConditions()
        {
            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            TextCondition textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle < i_BB:{0}:{1}_lower", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle > i_BB:{0}:{1}_upper", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sell;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle > i_BB:{0}:{1}_upper", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle < i_BB:{0}:{1}_lower", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
        }
    }
}

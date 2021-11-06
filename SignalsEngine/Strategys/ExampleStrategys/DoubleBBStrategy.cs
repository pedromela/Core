using BrokerLib.Lib;
using BrokerLib.Market;
using SignalsEngine.Conditions;
using System;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Strategys.ExampleStrategys
{
    public class DoubleBBStrategy : ConditionStrategy
    {
        public int Period { get; set; }
        public float StdDevFactor { get; set; }
        public int Period2 { get; set; }
        public float StdDevFactor2 { get; set; }
        public DoubleBBStrategy(MarketInfo marketInfo, TimeFrames timeFrame)
        : base("Double Bolinger Bands Strategy Example", marketInfo, timeFrame)
        {
            this.Period = 200;
            this.StdDevFactor = 2;
            this.Period2 = 100;
            this.StdDevFactor2 = 3;
            AddConditions();
        }

        public DoubleBBStrategy(int Period, float StdDevFactor, int Period2, float StdDevFactor2)
        : base(String.Format("Double Bolinger Bands Buy {0}:{1} Sell {2}:{3} Strategy Example", Period, StdDevFactor, Period2, StdDevFactor2), null, TimeFrames.H1)
        {
            this.Period = Period;
            this.StdDevFactor = StdDevFactor;
            this.Period2 = Period2;
            this.StdDevFactor2 = StdDevFactor2;
            AddConditions();
        }


        public override void AddConditions()
        {
            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            TextCondition textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle < i_BB:{0}:{1}_lower", Period2, StdDevFactor2), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle > i_BB:{0}:{1}_upper", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sell;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle > i_BB:{0}:{1}_upper", Period2, StdDevFactor2), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle < i_BB:{0}:{1}_lower", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
        }
    }
}

using BrokerLib.Market;
using SignalsEngine.Conditions;
using System;
using static BrokerLib.BrokerLib;


namespace SignalsEngine.Strategys.ExampleStrategys
{
    public class BBVWAPStrategy : ConditionStrategy
    {
        public int Period { get; set; }
        public float StdDevFactor { get; set; }
        public BBVWAPStrategy(MarketInfo marketInfo, TimeFrames timeFrame)
        : base("Bolinger Bands VWAP Strategy Example", marketInfo, timeFrame)
        {
            this.Period = 200;
            this.StdDevFactor = 2;
            AddConditions();
        }

        public BBVWAPStrategy(int Period, float StdDevFactor)
        : base("Bolinger Bands VWAP Strategy Example", null, TimeFrames.H1)
        {
            this.Period = Period;
            this.StdDevFactor = StdDevFactor;
            AddConditions();
        }


        public override void AddConditions()
        {
            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            TextCondition textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle < i_BB:{0}:{1}_lower and i_price:200_middle < i_VWAP_middle", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle > i_BB:{0}:{1}_upper and i_price:200_middle > i_VWAP_middle", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sell;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle > i_BB:{0}:{1}_upper and i_price:200_middle > i_VWAP_middle", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle < i_BB:{0}:{1}_lower and i_price:200_middle < i_VWAP_middle", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
        }
    }

}

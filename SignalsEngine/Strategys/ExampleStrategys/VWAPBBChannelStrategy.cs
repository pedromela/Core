using BrokerLib.Market;
using SignalsEngine.Conditions;
using System;
using static BrokerLib.BrokerLib;


namespace SignalsEngine.Strategys.ExampleStrategys
{
    public class VWAPBBChannelStrategy : ConditionStrategy
    {
        public int Period { get; set; }
        public float StdDevFactor { get; set; }
        public VWAPBBChannelStrategy(MarketInfo marketInfo, TimeFrames timeFrame)
        : base("Bolinger Bands VWAP Channel Strategy Example", marketInfo, timeFrame)
        {
            this.Period = 200;
            this.StdDevFactor = 2;
            AddConditions();
        }

        public VWAPBBChannelStrategy(int Period, float StdDevFactor)
        : base(String.Format("Bolinger Bands {0}:{1} VWAP Channel Strategy Example", Period, StdDevFactor), null, TimeFrames.H1)
        {
            this.Period = Period;
            this.StdDevFactor = StdDevFactor;
            AddConditions();
        }


        public override void AddConditions()
        {
            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            TextCondition textCondition = new TextCondition(_marketInfo, String.Format("( i_price:200_middle < i_VWAP_upper and i_price:200_middle > i_VWAP_lower and i_price:200_middle < i_BB:{0}:{1}_lower ) or ( i_price:200_middle > i_VWAP_upper and i_price:200_middle < i_BB:{0}:{1}_middle )", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle > i_BB:{0}:{1}_upper * 11 / 10", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sell;
            textCondition = new TextCondition(_marketInfo, String.Format("( i_price:200_middle < i_VWAP_upper and i_price:200_middle > i_VWAP_lower and i_price:200_middle > i_BB:{0}:{1}_upper ) or ( i_price:200_middle < i_VWAP_lower and i_price:200_middle > i_BB:{0}:{1}_middle )", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_price:200_middle < i_BB:{0}:{1}_lower  * 10 / 11", Period, StdDevFactor), transactionType, _timeFrame);
            AddCondition(textCondition);
        }
    }

}

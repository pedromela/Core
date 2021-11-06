using BrokerLib.Market;
using SignalsEngine.Conditions;
using System;
using static BrokerLib.BrokerLib;


namespace SignalsEngine.Strategys.ExampleStrategys
{
    public class VisionAlgoStrategy : ConditionStrategy
    {
        public int Period { get; set; }
        public VisionAlgoStrategy(MarketInfo marketInfo, TimeFrames timeFrame)
        : base("Vision Algo Strategy Example", marketInfo, timeFrame)
        {
            this.Period = 200;
            AddConditions();
        }

        public VisionAlgoStrategy(int Period)
        : base(String.Format("Vision Algo Strategy Example"), null, TimeFrames.H1)
        {
            this.Period = Period;
            AddConditions();
        }


        public override void AddConditions()
        {
            TransactionType transactionType = BrokerLib.BrokerLib.TransactionType.buy;
            TextCondition textCondition = new TextCondition(_marketInfo, String.Format("i_SMMA:5_middle crossup i_SMMA:8_middle", Period), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.buyclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_SMMA:5_middle crossdown i_SMMA:8_middle", Period), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sell;
            textCondition = new TextCondition(_marketInfo, String.Format("i_SMMA:5_middle crossdown i_SMMA:8_middle", Period), transactionType, _timeFrame);
            AddCondition(textCondition);
            transactionType = BrokerLib.BrokerLib.TransactionType.sellclose;
            textCondition = new TextCondition(_marketInfo, String.Format("i_SMMA:5_middle crossup i_SMMA:8_middle", Period), transactionType, _timeFrame);
            AddCondition(textCondition);
        }
    }

}

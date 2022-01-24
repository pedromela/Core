using BrokerLib.Market;
using BrokerLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Indicators
{
    class FearGreedMetadata
    {
        public string error { get; set; }

    }

    class FearGreedData 
    {
        public string value { get; set; }
        public string value_classification { get; set; }
        public string timestamp { get; set; }
        public string time_until_update { get; set; }

    }

    class FearGreedModel : Parser 
    {
        public string price { get; set; }
        public FearGreedData[] data { get; set; }
        public FearGreedMetadata metadata { get; set; }
    }

    public class FearGreed : Indicator
    {
        public FearGreed(int Period, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("FG:" + Period, Period, TimeFrame, marketInfo, "Fear and Greed Indicator")
        {
            AddArgument("Period");
            this.ShorDescriptionName = GetShorDescriptionName();
        }

        public override void Init(Indicator indicator)
        {
            var response = Request.Get("https://api.alternative.me/fng/?limit=1");
            var responseObj = JsonConvert.DeserializeObject<FearGreedModel>(response);
            float value = Parser.ParseFloat(responseObj.data.First().value);
            AddLastValues(value, indicator.GetLastTimestamp());
            var timeUntilUpdate = Parser.ParseFloat(responseObj.data.First().time_until_update);
            MyTaskScheduler.Instance.ScheduleTaskInDueTimeOnlyOnce<Indicator>(Init, indicator, GetDescription(), TimeSpan.FromSeconds(timeUntilUpdate+10));
        }

        public override bool CalculateNext(Indicator indicator)
        {
            AddLastValues(GetLastClose(), indicator.GetLastTimestamp());
            return true;
        }

        public void AddLastValues(float pvalue, DateTime timestamp)
        {
            try
            {
                Dictionary<string, Candle> valueList = new Dictionary<string, Candle>();
                Candle candle = new Candle();
                candle.Close = pvalue;
                candle.Timestamp = timestamp;
                valueList.Add("middle", candle);
                AddLastValue(valueList);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }
    }
}


using BotLib.Models;
using BrokerLib.Models;
using SignalsEngine.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BacktesterLib.Lib
{
    public enum BacktestingState 
    {
        first,
        running,
        completed,
        error
    }
    public class BacktestData
    {
        public int Positions { get; set; }
        public float Successes { get; set; }
        public float CurrentProfit { get; set; }
        public float AmountGained { get; set; }
        public float AmountGainedDaily { get; set; }
        public float MaxDrawBack { get; set; }
        public DateTime Timestamp { get; set; }
        public int ActiveTransactions { get; set; }
        public IndicatorLines Lines{ get; set; }
        public List<string> LineNames { get; set; }
        //public KeyValuePair<string, List<Candle>> Lines2{ get; set; }
        public List<Candle> Candles { get; set; }
        public BacktestingState State { get; set; }


        public BacktestData()
        {
            Candles = new List<Candle>();
            Lines = new IndicatorLines(1000, null, true);
        }

        public void Update(Score score, DateTime dateTime, Candle lastCandle, Dictionary<string, Candle> lastPoints, BacktestingState state) 
        {
            Positions = score.Positions;
            Successes = score.Successes;
            CurrentProfit = score.CurrentProfit;
            MaxDrawBack = score.MaxDrawBack;
            ActiveTransactions = score.ActiveTransactions;
            AmountGained = score.AmountGained;
            AmountGainedDaily = score.AmountGainedDaily;
            if (lastCandle != null)
            {
                Candles.Add(lastCandle);
            }
            if (lastPoints != null)
            {
                Lines.AddLastValue(lastPoints);
                LineNames = lastPoints.Keys.ToList();
            }
            Timestamp = dateTime;
            State = state;
        }

        public void Reset() 
        {
            Candles.Clear();
            Lines.Clear();
        }
    }
}

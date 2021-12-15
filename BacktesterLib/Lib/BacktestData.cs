
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
        public int ActiveTransactionsCount { get; set; }
        public IndicatorLines Lines{ get; set; }
        public List<string> LineNames { get; set; }
        //public KeyValuePair<string, List<Candle>> Lines2{ get; set; }
        public List<Candle> Candles { get; set; }
        public BacktestingState State { get; set; }
        public List<Transaction> ActiveTransactions { get; set; }
        public List<Transaction> HistoryTransactions { get; set; }

        public BacktestData()
        {
            Candles = new List<Candle>();
            Lines = new IndicatorLines(1000, null, true);
            ActiveTransactions = new List<Transaction>();
            HistoryTransactions = new List<Transaction>();
        }

        public void Update(
            Score score,
            DateTime dateTime,
            Candle lastCandle,
            Dictionary<string, Candle> lastPoints,
            Transaction activeTransaction,
            Transaction historyTransaction,
            BacktestingState state) 
        {
            if (score != null)
            {
                Positions = score.Positions;
                Successes = score.Successes;
                CurrentProfit = score.CurrentProfit;
                MaxDrawBack = score.MaxDrawBack;
                ActiveTransactionsCount = score.ActiveTransactions;
                AmountGained = score.AmountGained;
                AmountGainedDaily = score.AmountGainedDaily;
            }
            if (activeTransaction != null)
            {
                ActiveTransactions.Add(activeTransaction);
            }
            if (historyTransaction != null)
            {
                HistoryTransactions.Add(historyTransaction);
            }
            if (lastCandle != null)
            {
                Candles.Add(lastCandle);
            }
            if (lastPoints != null)
            {
                Lines.AddLastValue(lastPoints);
                LineNames = lastPoints.Keys.ToList();
            }
            if (dateTime != DateTime.MinValue)
            {
                Timestamp = dateTime;
            }
            State = state;
        }

        public void Reset() 
        {
            Candles.Clear();
            Lines.Clear();
            ActiveTransactions.Clear();
            HistoryTransactions.Clear();
        }
    }
}

using BrokerLib.Brokers;
using BrokerLib.Lib;
using BrokerLib.Market;
using SignalsEngine.Indicators;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BacktesterEngine
{ 
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-PT");
            DateTime fromDate = DateTime.Today.ToUniversalTime().AddDays(-8);
            DateTime toDate = DateTime.Today.ToUniversalTime().AddDays(-1);
            BacktesterEngine backtesterEngine = new BacktesterEngine(fromDate, toDate);

            //backtesterEngine.Run();
            //backtesterEngine.BacktestAllBots();
            backtesterEngine.BacktestBot("6262b108-7be4-4668-9192-f1d248f17d8c", fromDate, toDate, null);
            //backtesterEngine.BacktestTelegramBot(2010);
            //backtesterEngine.BacktestInvertedTelegramBot(2009);
            //backtesterEngine.BacktestAllTelegramBots();
        }
    }
}

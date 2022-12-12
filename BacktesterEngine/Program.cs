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
            backtesterEngine.BacktestBot("010365c1-0246-4b02-b629-e1b1c0dafd9b", fromDate, toDate, null);
            //backtesterEngine.BacktestTelegramBot(2010);
            //backtesterEngine.BacktestInvertedTelegramBot(2009);
            //backtesterEngine.BacktestAllTelegramBots();
        }
    }
}

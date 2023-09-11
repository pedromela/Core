using System;
using System.Threading;

namespace BacktesterEngine
{ 
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-PT");
            DateTime fromDate = DateTime.Today.ToUniversalTime().AddDays(-7);
            DateTime toDate = DateTime.Today.ToUniversalTime().AddDays(-1);
            BacktesterEngine backtesterEngine = new BacktesterEngine();

            //backtesterEngine.Run();
            //backtesterEngine.BacktestAllBots();
            //backtesterEngine.BacktestBot("e3da2c25-d87f-4ed6-a1bf-e889fb845842", fromDate, toDate, null);
            backtesterEngine.BacktestTelegramChannel("https://t.me/s/m4d32tr4d3free");
            //backtesterEngine.BacktestTelegramChannel("https://t.me/s/free_forex_signals_fxlifestyle");
            //backtesterEngine.BacktestInvertedTelegramBot(2009);
            //backtesterEngine.BacktestAllTelegramBots();
        }
    }
}

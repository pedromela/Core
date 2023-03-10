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
            DateTime fromDate = DateTime.Today.ToUniversalTime().AddDays(-7);
            DateTime toDate = DateTime.Today.ToUniversalTime().AddDays(-1);
            BacktesterEngine backtesterEngine = new BacktesterEngine(fromDate, toDate);

            //backtesterEngine.Run();
            //backtesterEngine.BacktestAllBots();
            backtesterEngine.BacktestBot("e3da2c25-d87f-4ed6-a1bf-e889fb845842", fromDate, toDate, null);
            //backtesterEngine.BacktestTelegramBot(2010);
            //backtesterEngine.BacktestInvertedTelegramBot(2009);
            //backtesterEngine.BacktestAllTelegramBots();
        }
        const string alphabet = "abcdefghijklmnopqrstuvwxyz";

        //public static string Solution(string word, string cipher)
        //{
        //    // Type your solution here
        //    if (cipher.Length != alphabet.Length)
        //    {
        //        return "";
        //    }
        //    string cipheredWord = "";
        //    for (int i = 0; i < word.Length; i++)
        //    {
        //        cipheredWord += CipherLetter(word[i], cipher);
        //    }
        //    return cipheredWord;
        //}

        //public static char CipherLetter(char c, string cipher)
        //{
        //    int alphaIdx = alphabet.IndexOf(c);
        //    return cipher[alphaIdx];
        //}
    }
}

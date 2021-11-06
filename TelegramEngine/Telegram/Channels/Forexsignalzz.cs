using System;
using System.Collections.Generic;
using System.Text;
using TelegramLib.Models;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace TelegramEngine.Telegram.Channels
{
    class Forexsignalzz : Channel
    {
        public const string URL = "https://t.me/s/forexsignalzz";

        public Forexsignalzz() 
        : base("forexsignalzz", URL)
        {

        }

        public override TelegramTransaction Parse(string rawData) 
        {
            TelegramEngine.DebugMessage("###################### PARSING MESSAGE BEGIN ######################");
            TelegramEngine.DebugMessage("MESSAGE : " + rawData);
            try
            {
                if (!rawData.StartsWith("New signal"))
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                rawData = rawData.Substring("New signal ?? ".Length);

                TelegramTransaction t = new TelegramTransaction();
                t.Channel = _name;
                t.Timestamp = DateTime.UtcNow;

                if (rawData.Contains("BUY"))
                {
                    t.Type = TransactionType.buy;
                    rawData = rawData.Substring("buy ".Length);

                }
                else if (rawData.Contains("SELL"))
                {
                    t.Type = TransactionType.sell;
                    rawData = rawData.Substring("sell ".Length);

                }
                else
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }

                var strList = rawData.Split(" ");
                for (int i = 0; i < strList.Length; i++)
                {
                    if (i == 0)
                    {
                        t.Market = strList[0].Trim();
                        rawData = rawData.Substring(t.Market.Length + 8);
                        break;
                    }
                }

                int index = rawData.IndexOf("TAKE PROFIT AT ");
                string valueNow = rawData.Substring(0, index).Trim();
                t.EntryValue = Parser.ParseFloat(valueNow);

                rawData = rawData.Substring(index + "TAKE PROFIT AT ".Length);

                index = rawData.IndexOf("STOP LOSS AT ");
                valueNow = rawData.Substring(0, index).Trim();
                t.TakeProfit = Parser.ParseFloat(valueNow);

                rawData = rawData.Substring(index + "STOP LOSS AT ".Length);

                valueNow = rawData.Trim();
                t.StopLoss = Parser.ParseFloat(valueNow);
                TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                return t;
            }
            catch (Exception e)
            {
                TelegramEngine.DebugMessage(e.StackTrace);
            }
            TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

            return null;
        }
    }
}

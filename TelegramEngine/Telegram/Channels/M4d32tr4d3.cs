using System;
using TelegramLib.Models;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace TelegramEngine.Telegram.Channels
{
    public class M4d32tr4d3 : Channel
    {
        public const string URL = "https://t.me/s/m4d32tr4d3free";

        public M4d32tr4d3()
        : base("M4d32tr4d3", URL)
        {

        }

        public override TelegramTransaction Parse(string rawData)
        {
            rawData = rawData.ToUpper();
            TelegramEngine.DebugMessage("###################### PARSING MESSAGE BEGIN ######################");
            TelegramEngine.DebugMessage("MESSAGE : " + rawData);
            try
            {
                if (!rawData.StartsWith("NEW SIGNAL"))
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                rawData = rawData.Substring("NEW SIGNAL ?? ".Length);

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

                index = rawData.IndexOf("TAKE PROFIT 2 AT ");
                valueNow = rawData.Substring(0, index).Trim();
                t.TakeProfit = Parser.ParseFloat(valueNow);

                rawData = rawData.Substring(index + "TAKE PROFIT 2 AT ".Length);

                index = rawData.IndexOf("TAKE PROFIT 3 AT ");
                valueNow = rawData.Substring(0, index).Trim();
                t.TakeProfit2 = Parser.ParseFloat(valueNow);

                rawData = rawData.Substring(index + "TAKE PROFIT 3 AT ".Length);

                index = rawData.IndexOf("STOP LOSS AT ");
                valueNow = rawData.Substring(0, index).Trim();
                t.TakeProfit3 = Parser.ParseFloat(valueNow);

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

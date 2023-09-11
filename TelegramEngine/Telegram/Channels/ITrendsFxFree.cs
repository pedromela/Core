using System;
using TelegramLib.Models;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace TelegramEngine.Telegram.Channels
{
    public class ITrendsFxFree : Channel
    {
        public const string URL = "https://t.me/s/itrendsfxfree";

        public ITrendsFxFree() 
        :base("itrendsfxfree_deleted", URL)
        {

        }

        public override TelegramTransaction Parse(string rawData)
        {
            TelegramEngine.DebugMessage("###################### PARSING MESSAGE BEGIN ######################");
            TelegramEngine.DebugMessage("MESSAGE : " + rawData);
            try
            {
                if (!rawData.Contains(" EP: "))
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                TelegramTransaction t = new TelegramTransaction();
                t.Market = rawData.Substring(0, 6);
                t.Channel = _name;
                t.Timestamp = DateTime.UtcNow;

                rawData = rawData.Substring("xxxyyy ".Length);

                if (rawData.Contains("BUY  📈  "))
                {
                    t.Type = TransactionType.buy;
                    rawData = rawData.Substring("BUY  📈  ".Length);
                }
                else if (rawData.Contains("SELL  📉  "))
                {
                    t.Type = TransactionType.sell;
                    rawData = rawData.Substring("SELL  📉  ".Length);
                }
                else
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }

                if (!rawData.StartsWith("EP: "))
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }

                rawData = rawData.Substring("EP: ".Length);

                int index = rawData.IndexOf("SL:  ");
                string valueNow = rawData.Substring(0, index).Trim();
                t.EntryValue = Parser.ParseFloat(valueNow);

                rawData = rawData.Substring(index + "SL: ".Length);
                string token = "   TP:  ";
                int idx = -1;
                index = rawData.IndexOf(token);
                if (index == -1)
                {
                    token = "TP1:  ";
                    index = rawData.IndexOf(token);
                }

                valueNow = rawData.Substring(0, index).Trim();
                t.StopLoss = Parser.ParseFloat(valueNow);

                rawData = rawData.Substring(index + token.Length);

                index = rawData.IndexOf(" ");
                valueNow = rawData.Substring(0, index).Trim();
                t.TakeProfit = Parser.ParseFloat(valueNow);

                index = rawData.IndexOf("🎯");
                rawData = rawData.Substring(index, rawData.Length - index);
                
                token = "TP: ";
                index = rawData.IndexOf(token);
                if (index == -1)
                {
                    token = "TP2: ";
                    index = rawData.IndexOf(token);
                }
                rawData = rawData.Substring(index, rawData.Length - index);
                string[] toks = { "TP:  ", "TP2:  "};
                string number = GetNumberAfterStrings(rawData, toks, ref idx);
                if (number == "-1")
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }

                t.TakeProfit2 = Parser.ParseFloat(number);

                token = "🎯";
                index = rawData.IndexOf(token);
                rawData = rawData.Substring(index, rawData.Length - index);

                string[] toks3 = { "TP:  ", "TP3:  " };
                number = GetNumberAfterStrings(rawData, toks3, ref idx);
                if (number == "-1")
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }

                t.TakeProfit3 = Parser.ParseFloat(number);
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

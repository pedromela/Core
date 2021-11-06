using System;
using System.Collections.Generic;
using System.Text;
using TelegramLib.Models;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace TelegramEngine.Telegram.Channels
{
    class Youthtribe : Channel
    {
        public const string URL = "https://t.me/s/youthtribefx";

        public Youthtribe()
        : base("youthtribefx", URL)
        {

        }

        public override TelegramTransaction Parse(string rawData)
        {
            TelegramEngine.DebugMessage("###################### PARSING MESSAGE BEGIN ######################");
            TelegramEngine.DebugMessage("MESSAGE : " + rawData);
            try
            {
                if (!rawData.StartsWith("#"))
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                rawData = rawData.Substring("#".Length);

                TelegramTransaction t = new TelegramTransaction();
                t.Market = rawData.Substring(0, 6);
                t.Channel = _name;
                t.Timestamp = DateTime.UtcNow;

                rawData = rawData.Substring("xxxyyy".Length);
                string token = "";
                if (rawData.StartsWith("BUY"))
                {
                    t.Type = TransactionType.buy;
                    token = "BUY NOW ";

                }
                else if (rawData.StartsWith("SELL"))
                {
                    t.Type = TransactionType.sell;
                    token = "SELL NOW ";
                }
                else
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }

                string str = GetNumberAfterString(rawData, token);
                if (str == "-1")
                {
                    return null;
                }
                t.EntryValue = Parser.ParseFloat(str);

                rawData = rawData.Substring(token.Length + str.Length);

                string[] toks = { "TP", "T.P" };
                string[] toks1 = { "SL", "S.L" };
                int idx = -1;

                str = GetNumberAfterStrings(rawData, toks, ref idx);
                if (str == "-1")
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                t.TakeProfit = Parser.ParseFloat(str);
                rawData = rawData.Substring(toks[idx].Length + 1 + str.Length);

                int index = GetIndexOfAny(rawData, toks, ref idx);
                if (index == -1) // if no TP tokens left look for SL and return else continue to look for more TP
                {
                    string number = GetNumberAfterStrings(rawData, toks1, ref idx);
                    t.StopLoss = Parser.ParseFloat(number);
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return t;
                }

                rawData = rawData.Substring(index);

                str = GetNumberAfterStrings(rawData, toks, ref idx);
                if (str == "-1")
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                t.TakeProfit2 = Parser.ParseFloat(str);
                rawData = rawData.Substring(toks[idx].Length + 1 + str.Length);

                str = GetNumberAfterStrings(rawData, toks, ref idx);
                if (str == "-1")
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return t;
                }
                t.TakeProfit3 = Parser.ParseFloat(str);

                rawData = rawData.Substring(toks[idx].Length + 1 + str.Length);

                str = GetNumberAfterStrings(rawData, toks1, ref idx);
                if (str == "-1")
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return t;
                }
                t.StopLoss = Parser.ParseFloat(str);
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

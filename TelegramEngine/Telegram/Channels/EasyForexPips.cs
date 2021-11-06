using System;
using System.Collections.Generic;
using System.Text;
using TelegramLib.Models;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace TelegramEngine.Telegram.Channels
{
    class EasyForexPips : Channel
    {
        public const string URL = "https://t.me/s/easyforexpips";

        public EasyForexPips()
        : base("easyforexpips", URL)
        {

        }

        public override TelegramTransaction Parse(string rawData)
        {
            TelegramEngine.DebugMessage("###################### PARSING MESSAGE BEGIN ######################");
            TelegramEngine.DebugMessage("MESSAGE : " + rawData);
            try
            {
                if (!rawData.StartsWith("SELL") && !rawData.StartsWith("BUY"))
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }

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

                t.Market = rawData.Substring(0,6);

                string token = "@";
                string number = GetNumberAfterString(rawData, token);
                if (number == "-1")
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                t.EntryValue = Parser.ParseFloat(number);

                token = "Take profit 1 at ";
                number = GetNumberAfterString(rawData, token);
                if (number == "-1")
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                t.TakeProfit = Parser.ParseFloat(number);

                token = "Take profit 2 at ";
                number = GetNumberAfterString(rawData, token);
                if (number == "-1")
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                t.TakeProfit2 = Parser.ParseFloat(number);

                token = "Take profit 3 at ";
                number = GetNumberAfterString(rawData, token);
                if (number == "-1")
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                t.TakeProfit3 = Parser.ParseFloat(number);

                token = "Stop loss at ";
                number = GetNumberAfterString(rawData, token);
                if (number == "-1")
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                t.StopLoss = Parser.ParseFloat(number);
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

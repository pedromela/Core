using System;
using System.Collections.Generic;
using System.Text;
using TelegramLib.Models;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace TelegramEngine.Telegram.Channels
{
    class ForexSignalsStreet : Channel
    {
        public const string URL = "https://t.me/s/Forexsignalsstreet";

        public ForexSignalsStreet()
        : base("Forexsignalsstreet", URL)
        {

        }

        public override TelegramTransaction Parse(string rawData)
        {
            TelegramEngine.DebugMessage("###################### PARSING MESSAGE BEGIN ######################");
            TelegramEngine.DebugMessage("MESSAGE : " + rawData);
            try
            {
                if (!rawData.Contains("#signal #forex"))
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                rawData = rawData.Substring("A ".Length);

                TelegramTransaction t = new TelegramTransaction();
                t.Channel = _name;
                t.Timestamp = DateTime.UtcNow;

                if (rawData.StartsWith("BUY"))
                {
                    t.Type = TransactionType.buy;
                    rawData = rawData.Substring("buy ".Length);
                }
                else if (rawData.StartsWith("SELL"))
                {
                    t.Type = TransactionType.sell;
                    rawData = rawData.Substring("sell ".Length);
                }
                else
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }

                rawData = rawData.Substring("#signal #forex ".Length);

                t.Market = rawData.Substring(0, 6);
                rawData = rawData.Substring(t.Market.Length + " 🔥".Length);

                if (!rawData.StartsWith("PRICE @ "))
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");

                    return null;
                }
                
                rawData = rawData.Substring("PRICE @ ".Length);


                string[] toks = { "TP1 @ ", "TP1@ " };
                int idx = -1;
                int index = GetIndexOfAny(rawData, toks, ref idx);
                string valueNow = rawData.Substring(0, index).Trim();
                t.EntryValue = Parser.ParseFloat(valueNow);

                rawData = rawData.Substring(index + toks[idx].Length);
                
                string[] toks2 = { "TP2 @ ", "TP2@ " };
                index = GetIndexOfAny(rawData, toks2, ref idx);
                valueNow = rawData.Substring(0, index).Trim();
                t.TakeProfit = Parser.ParseFloat(valueNow);

                rawData = rawData.Substring(index + toks2[idx].Length);
                string[] toks3 = { "TP3 @ ", "TP3@ " };

                index = GetIndexOfAny(rawData, toks3, ref idx);
                valueNow = rawData.Substring(0, index).Trim();
                t.TakeProfit2 = Parser.ParseFloat(valueNow);

                rawData = rawData.Substring(index + toks3[idx].Length);

                string[] toks4 = { "SL @ ", "SL@ " };
                index = GetIndexOfAny(rawData, toks4, ref idx);
                valueNow = rawData.Substring(0, index).Trim();
                t.TakeProfit3 = Parser.ParseFloat(valueNow);

                rawData = rawData.Substring(index + toks4[idx].Length);

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

using System;
using System.Collections.Generic;
using System.Text;
using TelegramLib.Models;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace TelegramEngine.Telegram.Channels
{
    public  class FXNL : Channel
    {
        public const string URL = "https://t.me/s/freesignalsfxnl";

        public FXNL()
        : base("freesignalsfxnl", URL)
        {

        }

        public override TelegramTransaction Parse(string rawData)
        {
            TelegramEngine.DebugMessage("###################### PARSING MESSAGE BEGIN ######################");
            TelegramEngine.DebugMessage("MESSAGE : " + rawData);
            try
            {
                if (!rawData.StartsWith("📈BUY") && !rawData.StartsWith("📈SELL") && !rawData.StartsWith("📉SET"))
                {
                    TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");
                    return null;
                }

                TelegramTransaction t = new TelegramTransaction();
                t.Channel = _name;
                t.Timestamp = DateTime.UtcNow;

                string token = "";

                if (rawData.StartsWith("📉SET"))
                {
                    rawData = rawData.Substring("📉SET".Length + 1);
                    t.Market = rawData.Substring(0, 6);
                    rawData = rawData.Substring(t.Market.Length + 1);
                    if (rawData.StartsWith("BUY"))
                    {
                        t.Type = TransactionType.buy;
                        token = "BUY ";
                    }
                    else if (rawData.StartsWith("SELL"))
                    {
                        t.Type = TransactionType.sell;
                        token = "SELL ";
                    }
                    else
                    {
                        TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");
                        return null;
                    }

                    rawData = rawData.Substring(token.Length);

                    if (rawData.StartsWith("LIMIT"))
                    {
                        if (t.Type == TransactionType.sell)
                        {
                            t.Type = TransactionType.selllimit;
                        }
                        else if (t.Type == TransactionType.buy)
                        {
                            t.Type = TransactionType.buylimit;
                        }
                    }

                    string[] toks = { "TP", "T.P" };
                    string[] toks1 = { "SL", "S.L" };
                    int idx = -1;
                    string str = "-1";
                    str = GetNumberAfterStrings(rawData, toks1, ref idx);
                    if (str == "-1")
                    {
                        TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");
                        return null;
                    }

                    t.StopLoss = Parser.ParseFloat(str);

                    rawData = rawData.Substring(toks1[idx].Length + 1 + str.Length);

                    str = GetNumberAfterStrings(rawData, toks, ref idx);
                    if (str == "-1")
                    {
                        TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");
                        return null;
                    }

                    t.TakeProfit = Parser.ParseFloat(str);

                    rawData = rawData.Substring(toks[idx].Length + 1 + str.Length);

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
                        return null;
                    }

                    t.TakeProfit3 = Parser.ParseFloat(str);

                    //rawData = rawData.Substring(toks[idx].Length + 1 + str.Length);

                }
                else
                {
                    if (rawData.StartsWith("📈BUY"))
                    {
                        t.Type = TransactionType.buy;
                        token = "📈BUY ";
                    }
                    else if(rawData.StartsWith("📈SELL"))
                    {
                        t.Type = TransactionType.sell;
                        token = "📈SELL ";
                    }
                    else
                    {
                        TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");
                        return null;
                    }
                    rawData = rawData.Substring(token.Length);

                    t.Market = rawData.Substring(0, 6);
                    token = t.Market;

                    string str = GetNumberAfterString(rawData, token);
                    if (str == "-1")
                    {
                        TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");
                        return null;
                    }
                    t.EntryValue = Parser.ParseFloat(str);

                    rawData = rawData.Substring(token.Length + 1 + str.Length);

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

                    rawData = rawData.Substring(index, rawData.Length);

                    str = GetNumberAfterStrings(rawData, toks, ref idx);
                    if (str == "-1")
                    {
                        TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");
                        return null;
                    }
                    t.TakeProfit2 = Parser.ParseFloat(str);
                    rawData = rawData.Substring(toks[idx].Length + 1 + token.Length);

                    str = GetNumberAfterStrings(rawData, toks, ref idx);
                    if (str == "-1")
                    {
                        TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");
                        return t;
                    }
                    t.TakeProfit3 = Parser.ParseFloat(str);
                    rawData = rawData.Substring(toks[idx].Length + 1 + token.Length);

                    str = GetNumberAfterStrings(rawData, toks1, ref idx);
                    if (str == "-1")
                    {
                        TelegramEngine.DebugMessage("###################### PARSING MESSAGE END ######################");
                        return t;
                    }
                    t.StopLoss = Parser.ParseFloat(str);
                }


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

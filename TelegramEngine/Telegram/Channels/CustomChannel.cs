using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TelegramLib.Models;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace TelegramEngine.Telegram.Channels
{
    public class CustomChannel : Channel
    {
        List<string> markets = new List<string>()
        {
            "CHF_JPY",
            "TWIX_USD",
            "EUR_JPY",
            "XAG_CAD",
            "EU50_EUR",
            "USB10Y_USD",
            "SG30_SGD",
            "NATGAS_USD",
            "USD_CNH",
            "GBP_PLN",
            "XAG_NZD",
            "XAG_SGD",
            "XAU_EUR",
            "XAU_GBP",
            "XAG_CHF",
            "SPX500_USD",
            "USD_DKK",
            "BCO_USD",
            "EUR_DKK",
            "SOYBN_USD",
            "UK10YB_GBP",
            "AUD_CAD",
            "EUR_GBP",
            "NZD_USD",
            "USD_CZK",
            "NZD_JPY",
            "CN50_USD",
            "US2000_USD",
            "AUD_SGD",
            "AUD_HKD",
            "WTICO_USD",
            "XAG_AUD",
            "GBP_USD",
            "USD_MXN",
            "EUR_CHF",
            "AUD_CHF",
            "XAU_HKD",
            "ZAR_JPY",
            "CHF_ZAR",
            "HKD_JPY",
            "SUGAR_USD",
            "EUR_PLN",
            "XAU_JPY",
            "XCU_USD",
            "XAG_HKD",
            "USD_HKD",
            "XAG_JPY",
            "EUR_SGD",
            "USD_SEK",
            "GBP_SGD",
            "GBP_NZD",
            "XAU_NZD",
            "GBP_HKD",
            "EUR_HKD",
            "USD_JPY",
            "EUR_TRY",
            "USD_CAD",
            "EUR_CZK",
            "CAD_CHF",
            "NZD_HKD",
            "ESPIX_EUR",
            "NZD_CHF",
            "XAU_XAG",
            "XPD_USD",
            "XAU_USD",
            "XPT_USD",
            "JP225Y_JPY",
            "EUR_USD",
            "GBP_JPY",
            "USD_TRY",
            "CHF_HKD",
            "DE30_EUR",
            "NZD_CAD",
            "US30_USD",
            "NL25_EUR",
            "CHINAH_HKD",
            "USB02Y_USD",
            "EUR_NZD",
            "XAU_SGD",
            "GBP_CAD",
            "EUR_AUD",
            "SGD_JPY",
            "TRY_JPY",
            "EUR_ZAR",
            "AUD_JPY",
            "EUR_SEK",
            "USD_SGD",
            "USD_THB",
            "GBP_CHF",
            "GBP_AUD",
            "USD_PLN",
            "XAG_EUR",
            "HK33_HKD",
            "UK100_GBP",
            "USB05Y_USD",
            "EUR_NOK",
            "CH20_CHF",
            "CAD_HKD",
            "XAU_CAD",
            "CAD_JPY",
            "USD_ZAR",
            "XAU_AUD",
            "WHEAT_USD",
            "NZD_SGD",
            "XAU_CHF",
            "DE10YB_EUR",
            "USD_HUF",
            "EUR_CAD",
            "NAS100_USD",
            "CAD_SGD",
            "GBP_ZAR",
            "XAG_GBP",
            "EUR_HUF",
            "AUD_NZD",
            "FR40_EUR",
            "USB30Y_USD",
            "XAG_USD",
            "CORN_USD",
            "USD_CHF",
            "AUD_USD",
            "USD_NOK",
            "AU200_AUD",
            "SGD_CHF",
            "JP225_USD"
        };

        public CustomChannel(string channelUrl)
        : base("custom -> " + channelUrl, channelUrl)
        {

        }

        public override TelegramTransaction Parse(string rawData)
        {
            TelegramEngine.DebugMessage("###################### PARSING MESSAGE BEGIN ######################");
            TelegramEngine.DebugMessage("MESSAGE : " + rawData);
            try
            {
                TelegramTransaction t = new TelegramTransaction();
                t.Channel = _name;
                t.Timestamp = DateTime.UtcNow;
                rawData = rawData.ToLower();
                string[] separatingStrings = { Environment.NewLine, "<br>" };
                var strList = rawData.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var market in markets)
                {
                    if (rawData.Contains(market.Replace("_", "").ToLower()))
                    {
                        t.Market = market.Replace("_", "");
                        break;
                    }
                    else if (rawData.Contains("gold"))
                    {
                        t.Market = "XAUUSD";
                        break;
                    }
                }
                if (t.Market == null)
                {
                    return null;
                }
                for (int i = 0; i < strList.Length; i++)
                {
                    if (strList[i].Contains("sell") || strList[i].Contains("buy") || strList[i].Contains("price"))
                    {
                        if (strList[i].Contains("buy"))
                        {
                            t.Type = TransactionType.buy;
                        }
                        else if (strList[i].Contains("sell"))
                        {
                            t.Type = TransactionType.sell;
                        }

                        if (strList[i].Contains("@") || strList[i].Contains("now") || strList[i].Contains("sell") || strList[i].Contains("buy"))
                        {
                            t.EntryValue = Parser.ParseFloat(Regex.Match(strList[i], @"\d+[\.,]?\d*").Value);
                        }
                    }
                    else if (strList[i].Contains("tp") || strList[i].Contains("tp1") || strList[i].Contains("take profit"))
                    {
                        if (strList[i].Contains("tp3") || strList[i].Contains("take profit 3"))
                        {
                            t.TakeProfit3 = Parser.ParseFloat(Regex.Match(strList[i], @"\d+[\.,]?\d*").Value);
                        }
                        else if (strList[i].Contains("tp2") || strList[i].Contains("take profit 2"))
                        {
                            t.TakeProfit2 = Parser.ParseFloat(Regex.Match(strList[i], @"\d+[\.,]?\d*").Value);
                        }
                        else
                        {
                            t.TakeProfit = Parser.ParseFloat(Regex.Match(strList[i], @"\d+[\.,]?\d*").Value);
                        }
                    }
                    else if (strList[i].Contains("sl") || strList[i].Contains("stop loss"))
                    {
                        t.StopLoss = Parser.ParseFloat(Regex.Match(strList[i], @"\d+[\.,]?\d*").Value);
                    }
                    else if (strList[i].Contains("entry") || strList[i].Contains("now") || strList[i].Contains("sell") || strList[i].Contains("buy"))
                    {
                        t.EntryValue = Parser.ParseFloat(Regex.Match(strList[i], @"\d+[\.,]?\d*").Value);
                    }
                }

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

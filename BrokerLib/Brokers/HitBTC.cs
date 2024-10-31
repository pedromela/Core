using Newtonsoft.Json;
using System;
using System.Globalization;
using UtilsLib.Utils;
using static UtilsLib.Utils.Request;
using System.Collections.Generic;
using BrokerLib.Models;
using static BrokerLib.BrokerLib;
using BrokerLib.Market;
using BrokerLib.Exceptions;

namespace BrokerLib.Brokers
{
    public class Position : Parser
    {
        public string id { get; set; }
        public string symbol { get; set; }
        public string quantity { get; set; }
        public string pnl { get; set; }
        public string priceEntry { get; set; }
        public string priceLiquidation { get; set; }
        public string priceMarginCall { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
    }

    public class MarginAccount : Parser
    {
        public string symbol { get; set; }
        public string leverage { get; set; }
        public string marginBalance { get; set; }
        public string marginBalanceOrders { get; set; }
        public string marginBalanceRequired { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public Position position { get; set; }

    }
    public class HitBTC : Broker
    {
        public const string NAME = "HitBTC";
        public const string URL = "https://api.hitbtc.com/api/";
        public const AuthTypes AUTHTYPE = AuthTypes.BasicAuth;

        public HitBTC()
        : base(URL, AUTHTYPE, BrokerLib.Brokers.HitBTC, BrokerType.exchange, MarketTypes.Crypto, "")
        {
        }

        public HitBTC(bool margin = false)
        : base(URL, AUTHTYPE, BrokerLib.Brokers.HitBTC, margin ? BrokerType.margin : BrokerType.exchange, MarketTypes.Crypto, "")
        {
        }

        public HitBTC(string url, AuthTypes authType)
        : base(url, authType, BrokerLib.Brokers.HitBTC, BrokerType.exchange, MarketTypes.Crypto, "")
        {
        }

        public override bool CheckEquity(TransactionType transactionType, Equity equity, Candle lastCandle, float amount)
        {
            try
            {
                if (GetBrokerType() == BrokerType.margin)
                {
                    if (transactionType == TransactionType.buy || transactionType == TransactionType.sell)
                    {
                        if (equity.RealAvailableAmountSymbol2 > (amount * lastCandle.Close))
                        {
                            return true;
                        }
                    }
                    else if (transactionType == TransactionType.buyclose || transactionType == TransactionType.sellclose)
                    {
                        if (equity.RealAvailableAmountSymbol1 > amount)
                        {
                            return true;
                        }
                    }
                }
                else if (GetBrokerType() == BrokerType.exchange)
                {
                    if (transactionType == TransactionType.buy)
                    {
                        if (equity.RealAvailableAmountSymbol2 > (amount * lastCandle.Close))
                        {
                            return true;
                        }
                    }
                    else if (transactionType == TransactionType.buyclose)
                    {
                        if (equity.RealAvailableAmountSymbol1 > amount)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            BrokerLib.DebugMessage(String.Format("HitBTC::CheckEquity(): {0} attempt, insufficient funds on Equity Id {1}!", transactionType.ToString(), equity.id));

            return false;

        }

        public override Trade CloseTrade(Trade trade, Transaction transaction, AccessPoint accessPoint, string description = "")
        {
            TransactionType apiTransactionType = BrokerLib.OppositeTransactionType(transaction.Type);
            TransactionType tradeTansactionType = BrokerLib.CloseTransactionType(transaction.Type);

            string url = _url + "2/margin/order";
            string basicAuth = accessPoint.PublicKey + ":" + accessPoint.PrivateKey;
            string data = String.Format("symbol={0}&side={1}&quantity={2:0.########}&type={3}",
                            transaction.Market,
                            apiTransactionType,
                            trade.Amount.ToString(new CultureInfo("en-US")), "market");

            string response = Request.Post(url, basicAuth, data, "application/x-www-form-urlencoded");
            dynamic responseObj = JsonConvert.DeserializeObject(response);
            string id = "";
            float quantity = 0;
            float price = 0;

            if (responseObj.error != null)
            {
                if (responseObj.error.code == 20032)
                {
                    throw new MarginAccountNotFoundException(responseObj.message);
                }
                throw new TradeErrorException(String.Format("HitBTC::CloseTrade() : Trade error: {0}", (string)responseObj.error.description));
            }

            if (responseObj.id == null)
            {
                throw new TradeErrorException(String.Format("HitBTC::CloseTrade() : Trade error, responseObj.id is null"));
            }
            else
            {
                id = (string)responseObj.id;
            }

            if (responseObj.quantity == null)
            {
                throw new TradeErrorException(String.Format("HitBTC::CloseTrade() : Trade error, responseObj.quantity is null"));
            }
            else
            {
                quantity = (float)responseObj.quantity;
            }

            if (responseObj.tradesReport == null || responseObj.tradesReport[0].price == null)
            {
                throw new TradeErrorException(String.Format("HitBTC::CloseTrade() : Trade error, responseObj.tradesReport[0].price is null"));
            }
            else
            {
                price = (float)responseObj.tradesReport[0].price;
            }

            Trade closetrade = new Trade(accessPoint.id,
                    transaction.id,
                    id,
                    quantity,
                    price,
                    transaction.Market,
                    tradeTansactionType,
                    trade.id);

            closetrade.Print();

            return closetrade;
        }

        public override Trade Order(Transaction transaction, AccessPoint accessPoint, float amount)
        {
            try
            {
                string url = _url + "2/margin/order";
                string basicAuth = accessPoint.PublicKey + ":" + accessPoint.PrivateKey;
                string data = String.Format("symbol={0}&side={1}&quantity={2:0.########}&type={3}",
                                transaction.Market,
                                transaction.Type.ToString(),
                                amount.ToString(new CultureInfo("en-US")), "market");

                string response = Request.Post(url, basicAuth, data, "application/x-www-form-urlencoded");
                dynamic responseObj = JsonConvert.DeserializeObject(response);

                string id = "";
                float quantity = 0;
                float price = 0;

                if (responseObj.error != null)
                {
                    BrokerLib.DebugMessage(String.Format("HitBTC::Order() : Trade cancelled, reason: {0}", (string)responseObj.error));
                    return null;
                }

                if (responseObj.id == null)
                {
                    BrokerLib.DebugMessage(String.Format("HitBTC::Order() : Trade error, responseObj.id is null"));
                    return null;
                }
                else
                {
                    id = (string)responseObj.id;
                }

                if (responseObj.quantity == null)
                {
                    BrokerLib.DebugMessage(String.Format("HitBTC::Order() : Trade error, responseObj.quantity is null"));
                    return null;
                }
                else
                {
                    quantity = (float) responseObj.quantity;

                }

                if (responseObj.tradesReport == null || responseObj.tradesReport[0].price == null)
                {
                    BrokerLib.DebugMessage(String.Format("HitBTC::Order() : Trade error, responseObj.tradesReport[0].price is null"));
                    price = 0;
                }
                else
                {
                    price = (float)responseObj.tradesReport[0].price;
                }

                Trade trade = new Trade(accessPoint.id,
                                        transaction.id,
                                        id,
                                        quantity,
                                        price,
                                        transaction.Market,
                                        transaction.Type);
                //trade.Store();
                trade.Print();
                return trade;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public override void Buy(float amount, string market, AccessPoint accessPoint)
        {
            try
            {
                string data = String.Format("symbol={0}&side={1}&quantity={2:0.########}&type={3}", market, TransactionType.buy.ToString(), amount.ToString(new CultureInfo("en-US")), "market");
                string url = _url + "2/order";
                string basicAuth = accessPoint.PublicKey + ":" + accessPoint.PrivateKey;
                string response = Request.Post(url, basicAuth, data, "application/x-www-form-urlencoded");
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public override void Sell(float amount, string market, AccessPoint accessPoint)
        {
            try
            {
                string data = String.Format("symbol={0}&side={1}&quantity={2:0.########}&type={3}", market, TransactionType.sell.ToString(), amount.ToString(new CultureInfo("en-US")), "market");
                string url = _url + "2/order";
                string basicAuth = accessPoint.PublicKey + ":" + accessPoint.PrivateKey;
                string response = Request.Post(url, basicAuth, data, "application/x-www-form-urlencoded");
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public override float GetCurrencyBalance(AccessPoint accessPoint, string market, float lastClose, bool invert = false)
        {
            try
            {
                string auth = accessPoint.PublicKey + ":" + accessPoint.PrivateKey;
                if (GetBrokerType() == BrokerType.margin)
                {
                    string url = _url + "2/margin/account/" + market;
                    string response = Request.Get(url, auth);
                    MarginAccount marginAccount = JsonConvert.DeserializeObject<MarginAccount>(response);
                    float balance = Parser.ParseFloat(marginAccount.marginBalance) * Parser.ParseFloat(marginAccount.leverage);

                    if (marginAccount.position != null)
                    {
                        balance -= Math.Abs(Parser.ParseFloat(marginAccount.position.quantity) * lastClose);
                    }
                    if (invert)
                    {
                        return balance;
                    }
                    return balance / lastClose;
                }
                else if (GetBrokerType() == BrokerType.exchange)
                {
                    string currency = market.Substring(0, 3);

                    if (invert)
                    {
                        currency = market.Substring(3);
                    }
                    string url = _url + "2/trading/balance";
                    string response = Request.Get(url, auth);
                    float balannceInSymbol2 = 0;
                    List<Balance> balances = JsonConvert.DeserializeObject<List<Balance>>(response);
                    foreach (Balance balance in balances)
                    {
                        if (balance.Currency.Equals(currency))
                        {
                            balannceInSymbol2 += balance.Available;
                            break;
                        }
                    }
                    return balannceInSymbol2;
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public override float GetAvailableMarketBalance(AccessPoint accessPoint, string market, float lastClose, bool invert = false)
        {
            try
            {
                string auth = accessPoint.PublicKey + ":" + accessPoint.PrivateKey;

                if (GetBrokerType() == BrokerType.margin)
                {
                    string url = _url + "2/margin/account/" + market;
                    string response = Request.Get(url, auth);
                    MarginAccount marginAccount = JsonConvert.DeserializeObject<MarginAccount>(response);
                    return float.Parse(marginAccount.marginBalance);
                }
                else if (GetBrokerType() == BrokerType.exchange)
                {
                    string symbol1 = market.Substring(0, 3);
                    string symbol2 = market.Substring(3);

                    if (invert)
                    {
                        string symbolaux = symbol1;
                        symbol1 = symbol2;
                        symbol2 = symbolaux;
                    }

                    string url = _url + "2/trading/balance";
                    string response = Request.Get(url, auth);
                    float balannceInSymbol2 = 0;
                    List<Balance> balances = JsonConvert.DeserializeObject<List<Balance>>(response);
                    foreach (Balance balance in balances)
                    {
                        if (invert)
                        {
                            if (balance.Currency.Equals(symbol1))
                            {
                                balannceInSymbol2 += balance.Available / lastClose;
                            }
                            else if (balance.Currency.Equals(symbol2))
                            {
                                balannceInSymbol2 += balance.Available;
                            }
                        }
                        else
                        {
                            if (balance.Currency.Equals(symbol1))
                            {
                                balannceInSymbol2 += balance.Available * lastClose;
                            }
                            else if (balance.Currency.Equals(symbol2))
                            {
                                balannceInSymbol2 += balance.Available;
                            }
                        }
                    }
                    return balannceInSymbol2;
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public override float GetTotalMarketBalance(AccessPoint accessPoint, string market, float lastClose, bool invert = false)
        {
            try
            {
                string symbol1 = market.Substring(0, 3);
                string symbol2 = market.Substring(3);

                if (invert)
                {
                    string symbolaux = symbol1;
                    symbol1 = symbol2;
                    symbol2 = symbolaux;
                }

                string auth = accessPoint.PublicKey + ":" + accessPoint.PrivateKey;
                string url = _url + "2/trading/balance";

                string response = Request.Get(url, auth);
                float balannceInSymbol2 = 0;
                List<Balance> balances = JsonConvert.DeserializeObject<List<Balance>>(response);
                foreach (Balance balance in balances)
                {
                    if (invert)
                    {
                        if (balance.Currency.Equals(symbol1))
                        {
                            balannceInSymbol2 += (balance.Reserved + balance.Available) / lastClose;
                        }
                        else if (balance.Currency.Equals(symbol2))
                        {
                            balannceInSymbol2 += balance.Reserved + balance.Available;
                        }
                    }
                    else
                    {
                        if (balance.Currency.Equals(symbol1))
                        {
                            balannceInSymbol2 += (balance.Reserved + balance.Available) * lastClose;
                        }
                        else if (balance.Currency.Equals(symbol2))
                        {
                            balannceInSymbol2 += balance.Reserved + balance.Available;
                        }
                    }
                }
                return balannceInSymbol2;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public override List<Candle> GetLastCandles(string market, TimeFrames timeFrame, int lastCount = 1)
        {
            try
            {
                DateTime today = DateTime.UtcNow.AddMinutes(-(int)timeFrame * lastCount);
                string date = String.Format("{0} {1}", today.ToShortDateString().Replace("/", "-"), today.ToShortTimeString());

                string url = String.Format(_url + "2/public/candles/{0}?period={1}&from={2}&limit={3}", market, timeFrame.ToString(), date, lastCount);
                string result = Request.Get(url);
                List<Candle> candles = JsonConvert.DeserializeObject<List<Candle>>(result);
                if (candles != null)
                {
                    foreach (Candle candle in candles)
                    {
                        candle.TimeFrame = timeFrame;
                        candle.Symbol = market;
                    }
                }


                //using (var context = DataDBContext.newDBContext())//must be needed in botengine or something
                //{
                //    context.Candles.AddRange(candles);
                //    context.SaveChanges();
                //}


                return candles;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public override List<Candle> GetCandles(string market, TimeFrames timeFrame, DateTime fromDate, int count)
        {
            try
            {
                if (fromDate > DateTime.UtcNow)
                {
                    BrokerLib.DebugMessage(String.Format("HitBTC::GetCandles({0},{1},{2},{3}) : Invalid value specified for 'fromDate'. Time is in the future", market, timeFrame, fromDate, count));
                    return new List<Candle>();
                }
                int remaining = count;
                if (count > 1000)
                {
                    count = 1000;
                    remaining -= 1000;
                }
                else
                {
                    remaining = 0;
                }

                //DateTime today = DateTime.Today.ToUniversalTime();//.ToUniversalTime();
                string date = String.Format("{0} {1}", fromDate.ToShortDateString().Replace("/", "-"), fromDate.ToShortTimeString());
                string url = String.Format(_url + "2/public/candles/{0}?period={1}&from={2}&limit={3}", market, timeFrame.ToString(), date, count);
                string result = Request.Get(url);
                List<Candle> candles = JsonConvert.DeserializeObject<List<Candle>>(result);

                for (int i = 0; i < candles.Count; i++)
                {
                    candles[i].TimeFrame = timeFrame;
                    candles[i].Symbol = market;
                }

                if (remaining > 0 && candles.Count > 0)
                {
                    DateTime lastDate = candles[candles.Count - 1].Timestamp.AddMinutes((int)timeFrame);
                    if (lastDate == DateTime.MinValue)
                    {
                        return null;
                    }
                    candles.AddRange(GetCandles(market, timeFrame, lastDate, remaining));
                }

                return candles;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public override void InitMarketsFirstTime(bool ignoreDiscovery = false)
        {
            try
            {
                _availableMarkets.Clear();
                _availableMarkets.Add(new MarketInfo("BTC_USD", this, ignoreDiscovery));
                _availableMarkets.Add(new MarketInfo("ETH_USD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("XRPUSD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("TRXUSD", this, ignoreDiscovery));
                _availableMarkets.Add(new MarketInfo("ADA_USD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("BCHUSD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("BSVUSD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("EOSUSD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("XLMUSD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("DOGEUSD", this, ignoreDiscovery));
                _availableMarkets.Add(new MarketInfo("ZEC_USD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("NEOUSD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("LTCUSD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("DOTUSD", this, ignoreDiscovery));
                _availableMarkets.Add(new MarketInfo("LTC_USD", this, ignoreDiscovery));
                _availableMarkets.Add(new MarketInfo("ETH_BTC", this, ignoreDiscovery));
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public override float CalculateMinimumTransactionAmount(string market)
        {
            try
            {
                return base.CalculateMinimumTransactionAmount(market);
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public override float GetAccountBalance(AccessPoint accessPoint)
        {
            throw new NotImplementedException();
        }
    }
}
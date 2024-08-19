using System;
using System.Collections.Generic;
using UtilsLib.Utils;
using System.Globalization;
using static UtilsLib.Utils.Request;
using Newtonsoft.Json;
using BrokerLib.Models;
using static BrokerLib.BrokerLib;
using BrokerLib.Market;
using BrokerLib.Exceptions;

namespace BrokerLib.Brokers
{
    public class TakeProfitOnFill
    {
        public string price { get; set; }
    }

    public class StopLossOnFill
    {
        public string price { get; set; }
    }

    public class OrderModel
    {
        public string price { get; set; }
        public StopLossOnFill stopLossOnFill { get; set; }
        public TakeProfitOnFill takeProfitOnFill { get; set; }
        public string timeInForce { get; set; }
        public string instrument { get; set; }
        public string units { get; set; }
        public string type { get; set; }
        public string positionFill { get; set; }

        public OrderModel(string units, string type, string instrument, string positionFill, string price, string stopLossOnFill, string takeProfitOnFill, string timeInForce)
        {
            this.price = price;

            this.takeProfitOnFill = new TakeProfitOnFill();
            this.takeProfitOnFill.price = takeProfitOnFill;

            this.stopLossOnFill = new StopLossOnFill();
            this.stopLossOnFill.price = stopLossOnFill;

            this.timeInForce = timeInForce;
            this.instrument = instrument;
            this.units = units;
            this.type = type;
            this.positionFill = positionFill;
        }
    }

    public class Order : Parser
    {
        public OrderModel order { get; set; }

        public Order(string units, string type, string instrument, string positionFill, string price, string stopLossOnFill, string takeProfitOnFill, string timeInForce)
        : base()
        {
            order = new OrderModel(units, type, instrument, positionFill, price, stopLossOnFill, takeProfitOnFill, timeInForce);
        }

    }

    public class OrderCloseModel
    {
        public string price { get; set; }
        public string timeInForce { get; set; }
        public string instrument { get; set; }
        public string units { get; set; }
        public string type { get; set; }
        public string positionFill { get; set; }

        public OrderCloseModel(string units, string type, string instrument, string positionFill, string price, string timeInForce)
        {
            this.price = price;
            this.timeInForce = timeInForce;
            this.instrument = instrument;
            this.units = units;
            this.type = type;
            this.positionFill = positionFill;
        }
    }

    public class OrderClose : Parser
    {
        public OrderCloseModel order { get; set; }

        public OrderClose(string units, string type, string instrument, string positionFill, string price, string timeInForce)
        : base()
        {
            order = new OrderCloseModel(units, type, instrument, positionFill, price, timeInForce);
        }
    }

    public class OANDACandleOHLC
    {
        public float c { get; set; }
        public float h { get; set; }
        public float l { get; set; }
        public float o { get; set; }

    }

    public class OANDACandleModel
    {
        public OANDACandleOHLC ask { get; set; }
        public OANDACandleOHLC mid { get; set; }
        public OANDACandleOHLC bid { get; set; }
        public bool complete { get; set; }
        public DateTime time { get; set; }
        public float volume { get; set; }
    }

    public class OANDACandles : Parser 
    {
        public List<OANDACandleModel> candles { get; set; }
        public string granularity { get; set; }
        public string instrument { get; set; }
        public string errorMessage { get; set; }

        public List<Candle> ConvertToCandleList() 
        {
            try
            {
                List<Candle> convertedCandleList = new List<Candle>();

                if (candles == null || candles.Count == 0)
                {
                    BrokerLib.DebugMessage("OANDACandles::ConvertToCandleList() : candle list to convert in null or empty.");
                    return convertedCandleList;
                }

                foreach (var c in candles)
                {
                    Candle candle = new Candle();
                    candle.Timestamp = c.time;
                    candle.Open = c.mid.o;
                    candle.Close = c.mid.c;
                    candle.Max = c.mid.h;
                    candle.Min = c.mid.l;
                    candle.TimeFrame = granularity == "D" ? TimeFrames.D1 : (TimeFrames) Enum.Parse(typeof(TimeFrames), granularity);
                    candle.Symbol = instrument.Replace("_", "");
                    candle.Volume = c.volume;
                    convertedCandleList.Add(candle);
                }
                return convertedCandleList;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }
    }

    public class OANDA : Broker
    {
        public const string NAME = "OANDA";
        public const string URL = "https://api-fxtrade.oanda.com/v3/";
        const AuthTypes AUTHTYPE = AuthTypes.BearerToken;

        public OANDA()
        : base(URL, AUTHTYPE, BrokerLib.Brokers.OANDA, BrokerType.margin, MarketTypes.Forex, AccessPoint.Construct("1"))
        {
        }

        public OANDA(string url, AuthTypes authType)
        : base(url, authType, BrokerLib.Brokers.OANDA, BrokerType.margin, MarketTypes.Forex, AccessPoint.Construct("1"))
        {
        }

        private string ParseMarket(string market) 
        {
            try
            {
                if (!market.Contains("_"))
                {
                     return market.Insert(3, "_");
                }
                return market;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public string DecideMarketType(string market) 
        {
            try
            {
                if (market.Contains("limit"))
                {
                    return "LIMIT";
                }
                return "MARKET";
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public override bool CheckEquity(TransactionType transactionType, Equity equity, Candle lastCandle, float amount)
        {
            try
            {
                if (transactionType == TransactionType.buy || transactionType == TransactionType.sell)
                {
                    if (equity.Amount > amount)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            BrokerLib.DebugMessage(String.Format("OANDA::CheckEquity(): {0} attempt, insufficient funds on Equity Id {1}!", transactionType.ToString(), equity.id));

            return false;

        }


        public int CountNumberDecimalPlaces(float number) 
        {
            string numberStr = number.ToString();
            var tokens = numberStr.Split(",");
            if (tokens.Length != 2)
            {
                return 0;
            }
            return tokens[1].Length;
        }
        public override Trade Order(Transaction transaction, AccessPoint accessPoint, float amount)
        {
            try
            {
                int decimalPlaces = CountNumberDecimalPlaces(transaction.Price);
                transaction.TakeProfit = MathF.Round(transaction.TakeProfit, decimalPlaces);
                transaction.StopLoss = MathF.Round(transaction.StopLoss, decimalPlaces);
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                //nfi.NumberDecimalDigits = decimalPlaces;
                if (transaction.Type == TransactionType.sell || transaction.Type == TransactionType.buyclose)
                {
                    amount = -amount;
                }
                Parser order = null;
                if (transaction.Type == TransactionType.sell || transaction.Type == TransactionType.buy)
                {
                    order = new Order(amount.ToString(nfi), "MARKET", ParseMarket(transaction.Market), "DEFAULT", null, transaction.StopLoss.ToString(nfi), transaction.TakeProfit.ToString(nfi), null);
                }
                else if (transaction.Type == TransactionType.sellclose || transaction.Type == TransactionType.buyclose)
                {
                    order = new OrderClose(amount.ToString(nfi), "MARKET", ParseMarket(transaction.Market), "DEFAULT", null, null);
                }
                else
                {
                    throw new TradeErrorException("OANDA::Order() : Trade error: Invalid transaction type, done transactions should not be processed.");
                }
                string url = _url + "accounts/" + accessPoint.Account + "/orders";
                string orderTxt = order.Parse();
                Console.WriteLine("########ORDERBEGIN########");
                Console.WriteLine("AcessPointID: " + accessPoint.id);
                Console.WriteLine(orderTxt);
                Console.WriteLine("########ORDEREND########");
                string response = Request.Post(url, accessPoint.BearerToken, orderTxt, "application/json", AuthTypes.BearerToken);
                dynamic responseObj = JsonConvert.DeserializeObject(response);
                dynamic orderFillTransaction = responseObj.orderFillTransaction;
                dynamic orderCancelTransaction = responseObj.orderCancelTransaction;
                dynamic errorMessage = responseObj.errorMessage;
                dynamic tradeID = null; 
                if (errorMessage != null)
                {
                    throw new TradeErrorException(String.Format("OANDA::Order() : Trade error: {0}", errorMessage));
                }
                else if (orderCancelTransaction != null)
                {
                    throw new TradeErrorException(String.Format("OANDA::Order() : Trade cancelled, reason: {0}", orderCancelTransaction.reason));
                }
                else if (orderFillTransaction == null || orderFillTransaction.tradeOpened == null)
                {
                    if (orderFillTransaction.tradeOpened != null)
                    {
                        tradeID = orderFillTransaction.tradeOpened.tradeID;
                    }
                    else if (orderFillTransaction.tradesClosed != null)
                    {
                        foreach (var item in orderFillTransaction.tradesClosed)
                        {
                            tradeID = item.tradeID;
                        }
                    }
                    else if (orderFillTransaction.tradeReduced != null)
                    {
                        tradeID = orderFillTransaction.tradeReduced.tradeID;
                    }
                    else 
                    {
                        throw new TradeErrorException(String.Format("OANDA::Order() : Trade cancelled, reason: {0}", "orderFillTransaction was null!"));
                    }
                }
                Trade trade = new Trade(accessPoint.id,
                                        transaction.id,
                                        Convert.ToString((string) tradeID),
                                        Convert.ToSingle((float) orderFillTransaction.units),
                                        Convert.ToSingle((float) orderFillTransaction.price),
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
                market = ParseMarket(market);
                string data = String.Format("symbol={0}&side={1}&quantity={2:0.########}&type={3}", ParseMarket(market), TransactionType.buy.ToString(), amount.ToString(new CultureInfo("en-US")), "market");
                string url = _url + "accounts/" + accessPoint.Account + "/orders";
                string response = Request.Post(url, accessPoint.BearerToken, data, "application/x-www-form-urlencoded");
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
                market = ParseMarket(market);
                string data = String.Format("symbol={0}&side={1}&quantity={2:0.########}&type={3}", market, TransactionType.buy.ToString(), amount.ToString(new CultureInfo("en-US")), "market");
                string url = _url + "accounts/"+ accessPoint.Account + "/orders";
                string response = Request.Post(url, accessPoint.BearerToken, data, "application/x-www-form-urlencoded");
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public override Trade CloseTrade(Trade trade, Transaction transaction, AccessPoint accessPoint, string description = "")
        {
            //OrderClose orderClose = new OrderClose(Math.Abs(trade.Amount).ToString());
            //string url = _url + "accounts/" + accessPoint.Account + "/trades/" + trade.BrokerTransactionId + "/close";
            //string orderCloseTxt = orderClose.Parse();
            //Console.WriteLine("####### ORDER CLOSE BEGIN ######");
            //Console.WriteLine("AcessPointID: " + accessPoint.id);
            //Console.WriteLine(orderCloseTxt);
            //Console.WriteLine("####### ORDER CLOSE END ########");
            //string response = Request.Put(url, accessPoint.BearerToken, orderCloseTxt, "application/json", AuthTypes.BearerToken);
            //dynamic responseObj = JsonConvert.DeserializeObject(response);
            //dynamic orderFillTransaction = responseObj.orderFillTransaction;
            //dynamic orderCancelTransaction = responseObj.orderCancelTransaction;
            //dynamic orderRejectTransaction = responseObj.orderRejectTransaction;
            //dynamic errorMessage = responseObj.errorMessage;

            //if (errorMessage != null)
            //{
            //    BrokerLib.DebugMessage(String.Format("OANDA::CloseTrade() : Trade error: {0}", errorMessage));
            //    return 0;
            //}
            //if (orderCancelTransaction != null)
            //{
            //    BrokerLib.DebugMessage(String.Format("OANDA::CloseTrade() : Trade cancelled, reason: {0}", orderCancelTransaction.reason));
            //    return 0;
            //}
            //if (orderRejectTransaction != null)
            //{
            //    BrokerLib.DebugMessage(String.Format("OANDA::CloseTrade() : Trade rejected, reason: {0}", orderRejectTransaction.reason));
            //    return 0;
            //}

            //trade.Type = BrokerLib.CloseTransactionType(trade.Type);
            //trade.Update();

            //Trade closetrade = new Trade(accessPoint.id,
            //                        transaction.id,
            //                        Convert.ToString(orderFillTransaction.orderID),
            //                        Convert.ToSingle(orderFillTransaction.units),
            //                        Convert.ToSingle(orderFillTransaction.price),
            //                        transaction.Market,
            //                        BrokerLib.CloseTransactionType(transaction.Type),
            //                        trade.id);

            //closetrade.Store();

            //float profit = (float) responseObj.orderFillTransaction.pl;

            //return profit;

            TransactionType transactionType = transaction.Type;
            TransactionType tradeTansactionType = BrokerLib.CloseTransactionType(transaction.Type);
            Transaction closetransaction = new Transaction(transaction);
            closetransaction.Type = tradeTansactionType;

            Trade closetrade = Order(closetransaction, accessPoint, trade.Amount);
            if (closetrade == null)
            {
                int idebug = 0;
            }

            return closetrade;
        }


        public override float GetAccountBalance(AccessPoint accessPoint)
        {
            try
            {
                string url = _url + "accounts/" + accessPoint.Account + "/summary";
                string response = Request.Get(url, accessPoint.BearerToken, AuthTypes.BearerToken);
                dynamic accountSummary = JsonConvert.DeserializeObject(response);
                return accountSummary.account.balance;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public override float GetCurrencyBalance(AccessPoint accessPoint, string market, float lastClose, bool invert = false)
        {
            try
            {
                int leverage = 20;
                return GetAccountBalance(accessPoint) / leverage;
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
                return GetAccountBalance(accessPoint);
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public override float GetTotalMarketBalance(AccessPoint accessPoint, string currency, float lastClose, bool invert = false)
        {
            try
            {
                return GetAccountBalance(accessPoint);
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
                market = ParseMarket(market);
                string timeFrameStr = timeFrame == TimeFrames.D1 ? timeFrame.ToString().Substring(0, 1) : timeFrame.ToString();
                string url = String.Format(_url + "instruments/" + market + "/candles/?count={0}&granularity={1}", lastCount, timeFrameStr);
                string response = Request.Get(url, _defaultAccessPoint.BearerToken, AuthTypes.BearerToken);
                OANDACandles candles = JsonConvert.DeserializeObject<OANDACandles>(response);
                return candles.ConvertToCandleList();
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
                    BrokerLib.DebugMessage(String.Format("OANDA::GetCandles({0},{1},{2},{3}) : Invalid value specified for 'fromDate'. Time is in the future", market, timeFrame, fromDate, count));
                    return new List<Candle>();
                }
                //if (MarketWatch.IsWeekendDay(fromDate))
                //{
                //    return new List<Candle>();
                //}
                if (!market.Contains("_"))
                {
                    market = market.Insert(3, "_");
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
                string timeFrameStr = timeFrame == TimeFrames.D1 ? timeFrame.ToString().Substring(0, 1) : timeFrame.ToString();
                string date = fromDate.ToString("o", DateTimeFormatInfo.InvariantInfo);
                string timezone = fromDate.ToString("zzzz");
                date = date.Replace(timezone, "+00:00");
                //string date = String.Format("{0} {1}:00.0Z", fromDate.ToShortDateString().Replace("/", "-"), fromDate.ToShortTimeString());
                string url = String.Format(_url + "instruments/" + market + "/candles/?count={0}&granularity={1}&from={2}", count, timeFrameStr, date);
                string response = Request.Get(url, _defaultAccessPoint.BearerToken, AuthTypes.BearerToken);
                OANDACandles candles = JsonConvert.DeserializeObject<OANDACandles>(response);

                if (candles.candles == null || candles.candles.Count == 0)
                {
                    return new List<Candle>();
                }

                //if (lastCount > 5000)
                //{
                //    DateTime lastDate = candles.candles[candles.candles.Count].time;
                //    lastCount -= 1000;
                //    candles.AddRange(GetCandles(market, timeFrame, lastDate, lastCount));
                //}

                List<Candle> candleList = candles.ConvertToCandleList();

                if (remaining > 0 && candleList.Count > 0)
                {
                    DateTime lastDate = candleList[candleList.Count - 1].Timestamp.AddMinutes((int) timeFrame);
                    if (lastDate == DateTime.MinValue)
                    {
                        return null;
                    }
                    candleList.AddRange(GetCandles(market, timeFrame, lastDate, remaining));
                }

                return candleList;
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
                _availableMarkets.Add(new MarketInfo("EUR_USD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("EURNZD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("EURAUD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("NZDUSD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("NZDCAD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("NZDCHF", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("AUDUSD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("AUDCHF", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("EURJPY", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("AUDJPY", this, ignoreDiscovery));
                _availableMarkets.Add(new MarketInfo("USD_CHF", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("USDJPY", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("EURCAD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("CHFJPY", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("CADJPY", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("GBPAUD", this, ignoreDiscovery));
                _availableMarkets.Add(new MarketInfo("GBP_USD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("GBPNZD", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("GBPJPY", this, ignoreDiscovery));
                //_markets.Add(new MarketInfo("XAUUSD", this, ignoreDiscovery));
                _availableMarkets.Add(new MarketInfo("SPX500_USD", this, ignoreDiscovery));
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
                if (GetBrokerType() == BrokerType.margin)
                {
                    return base.CalculateMinimumTransactionAmount(market);
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }
    }
}

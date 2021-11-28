using BrokerLib.Lib;
using BrokerLib.Market;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static BrokerLib.BrokerLib;
using static UtilsLib.Utils.Request;

namespace BrokerLib.Brokers
{
    public class BrokerDescription
    {
        public BrokerLib.Brokers BrokerId;
        public BrokerType BrokerType;

        public BrokerDescription(BrokerLib.Brokers brokerId, BrokerType brokerType)
        {
            BrokerId = brokerId;
            BrokerType = brokerType;
        }

        public override bool Equals(Object obj)
        {
            try
            {
                if (obj == null || !this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    BrokerDescription brokerInfo = (BrokerDescription)obj;
                    if (BrokerId == brokerInfo.BrokerId &&
                        BrokerType == brokerInfo.BrokerType)
                    {
                        return true;
                    }
                }

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return BrokerId.GetHashCode() + BrokerType.GetHashCode();
        }
    }

    public class BrokerView
    {
        public int id { get; set; }
        public string name { get; set; }
        public int authMode { get; set; }
        public string marketType { get; set; }
        public string brokerType { get; set; }


        public BrokerView(int _id, string _name, int _authMode, string _marketType, string _brokerType)
        {
            id = _id;
            name = _name;
            authMode = _authMode;
            marketType = _marketType;
            brokerType = _brokerType;
        }

    }

    public class Broker
    {
        protected string _url = "";
        protected AuthTypes _authType = AuthTypes.BasicAuth;
        protected AccessPoint _defaultAccessPoint = null;
        protected BrokerDescription _brokerDescription;
        protected MarketTypes _marketType = MarketTypes.Crypto;
        protected List<MarketInfo> _markets = new List<MarketInfo>();// switch to dictionary
        protected List<string> _activeMarketStrings = new List<string>();
        protected int _hourDifference = 0;
        public Broker(string url, AuthTypes authType, BrokerLib.Brokers brokerId, BrokerType brokerType, MarketTypes marketType, AccessPoint defaultAccessPoint = null)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-PT");

            _url = url;
            _authType = authType;
            _brokerDescription = new BrokerDescription(brokerId, brokerType);
            _marketType = marketType;
            _defaultAccessPoint = defaultAccessPoint;
            //_hourDifference = DateTime.Now - ;
        }

        public virtual BrokerLib.Brokers GetBrokerId()
        {
            return _brokerDescription.BrokerId;
        }

        public BrokerDescription GetBrokerDescription()
        {
            return _brokerDescription;
        }

        public BrokerType GetBrokerType()
        {
            return _brokerDescription.BrokerType;
        }

        public MarketTypes GetMarketType()
        {
            return _marketType;
        }

        public virtual Trade Order(Transaction transaction, AccessPoint accessPoint, float amount)
        {
            return null;
        }

        public virtual void Buy(float amount, string market, AccessPoint accessPoint)
        {

        }
        public virtual void Sell(float amount, string market, AccessPoint accessPoint)
        {

        }

        public virtual Trade CloseTrade(Trade trade, Transaction transaction, AccessPoint accessPoint, string description = "")
        {
            try
            {

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public virtual bool CheckEquity(TransactionType transactionType, Equity equity, Candle lastCandle, float amount)
        {
            try
            {
                if (transactionType == TransactionType.buy || transactionType == TransactionType.sell)
                {
                    if (equity.Amount < amount)
                    {
                        BrokerLib.DebugMessage("CFDBot::CheckEquity(): Buy attempt, insufficient funds on Equity Id " + equity.id + "!");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return false;

        }

        public virtual float GetAccountBalance(AccessPoint accessPoint)
        {
            try
            {

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public virtual float GetCurrencyBalance(AccessPoint accessPoint, string market, float lastClose, bool invert = false)
        {
            try
            {

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public virtual float GetAvailableMarketBalance(AccessPoint accessPoint, string market, float lastClose, bool invert = false)
        {
            try
            {

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public virtual float GetTotalMarketBalance(AccessPoint accessPoint, string market, float lastClose, bool invert = false)
        {
            try
            {

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public Candle GetLastCandle(string market, TimeFrames timeFrame)
        {
            try
            {
                List<Candle> candles = GetLastCandles(market, timeFrame);
                if (candles != null && candles.Count == 1)
                {
                    return candles[0];
                }
                else if (candles == null)
                {
                    BrokerLib.DebugMessage("Broker::GetLastCandle() : candles null!");
                }
                else if (candles.Count == 0)
                {
                    BrokerLib.DebugMessage("Broker::GetLastCandle() : 0 candles!");
                }
                else if (candles.Count > 1)
                {
                    BrokerLib.DebugMessage("Broker::GetLastCandle() : more than 1 candle.");
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public virtual List<Candle> GetLastCandles(string market, TimeFrames timeFrame, int lastCount = 1)
        {
            try
            {

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public virtual List<Candle> GetCandlesToInitDay(string market, TimeFrames timeFrame)
        {
            try
            {
                DateTime fromDate = DateTime.Today.ToUniversalTime();
                int count = 0;
                switch (timeFrame)
                {
                    case TimeFrames.M1:
                        count = 1440;
                        break;
                    case TimeFrames.M5:
                        count = 288;
                        break;
                    case TimeFrames.M15:
                        count = 96;
                        break;
                    case TimeFrames.M30:
                        count = 48;
                        break;
                    case TimeFrames.H1:
                        count = 24;
                        break;
                    default:
                        break;
                }
                List<Candle> candles = GetCandles(market, timeFrame, fromDate, count);

                return candles;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public virtual List<Candle> GetCandlesFirstDayOfWeek(string market, TimeFrames timeFrame)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-PT");
                DateTime fromDate = DateTime.Today.ToUniversalTime().AddDays(-(int)DateTime.Today.ToUniversalTime().DayOfWeek + (int)DayOfWeek.Monday - 1);

                int count = 0;
                switch (timeFrame)
                {
                    case TimeFrames.M1:
                        count = 1440;
                        break;
                    case TimeFrames.M5:
                        count = 288;
                        break;
                    case TimeFrames.M15:
                        count = 96;
                        break;
                    case TimeFrames.M30:
                        count = 48;
                        break;
                    case TimeFrames.H1:
                        count = 24;
                        break;
                    default:
                        break;
                }
                List<Candle> candles = GetCandles(market, timeFrame, fromDate, count);

                return candles;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public virtual List<Candle> GetCandles(string market, TimeFrames timeFrame, DateTime fromDate, int lastCount)
        {
            try
            {

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public Candle PredictNextCandle()
        {
            try
            {

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public virtual List<Candle> GetCandles(string market, TimeFrames timeFrame, DateTime fromDate, DateTime toDate)
        {
            try
            {
                DateTime _fromDate = fromDate;
                DateTime _toDate = toDate;
                List<Candle> candleList = new List<Candle>();
                double period = (toDate - fromDate).TotalMinutes / (int)timeFrame;

                if (period < 10)
                {
                    BrokerLib.DebugMessage("GetCandles() : missing period is too short. continue...");
                    return candleList;
                }

                while (_fromDate < _toDate)
                {
                    //DecideSleep(timeFrame);

                    List<Candle> auxCandleList = GetCandles(market, timeFrame, _fromDate, (int)period);

                    if (auxCandleList == null)
                    {
                        _fromDate = _fromDate.AddMinutes((int)timeFrame * period);
                        continue;
                    }

                    if (auxCandleList.Count > 0)
                    {

                        if (!Candles.IsConsistent(auxCandleList))
                        {
                            BrokerLib.DebugMessage("GetCandles() : auxCandleList inconsistent...");
                        }

                        foreach (var candle in auxCandleList)
                        {
                            if (_fromDate <= candle.Timestamp && candle.Timestamp <= _toDate)
                            {
                                _fromDate = candle.Timestamp;
                            }
                            else
                            {
                                _fromDate = _toDate;
                                break;
                            }

                            candleList.Add(candle);

                        }

                    }
                    else
                    {
                        _fromDate = _toDate;
                    }

                    if (candleList.Count > 0)
                    {
                        if (candleList[0].Timestamp < fromDate || candleList[candleList.Count - 1].Timestamp > toDate)
                        {
                            candleList.Clear();
                            return candleList;
                        }
                    }

                }

                return candleList;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public virtual void InitMarketsFirstTime(bool ignoreDiscovery = false)
        {
            try
            {

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public virtual void InitMarkets(List<string> activeMarketStrings = null, bool ignoreDiscovery = false)
        {
            try
            {
                _markets.Clear();
                if (activeMarketStrings != null)
                {
                    _activeMarketStrings = activeMarketStrings;
                }

                if (_activeMarketStrings == null || _activeMarketStrings.Count == 0)
                {
                    BrokerLib.DebugMessage("Broker::InitMarkets() : activeMarketsStrings list is null or empty!");
                    return;
                }
                foreach (string market in _activeMarketStrings)
                {
                    _markets.Add(new MarketInfo(market, this, ignoreDiscovery));
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public virtual List<MarketInfo> GetMarketInfos(bool ignoreDiscovery = false)
        {
            try
            {
                if (_markets.Count == 0)
                {
                    BrokerLib.DebugMessage("Broker::GetMarketInfos() : markets list is empty!");

                    //InitMarkets(ignoreDiscovery);
                }
                //if (_markets.Count > 0 && !ignoreDiscovery)
                //{
                //    if (_markets[0]._minimumTransactionAmount == 0)
                //    {
                //        InitMarkets(ignoreDiscovery);
                //    }
                //}
                return _markets;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public virtual MarketInfo GetMarketInfo(string market)
        {
            try
            {
                if (_markets.Count == 0)
                {
                    InitMarkets();
                }
                foreach (MarketInfo marketInfo in _markets)
                {
                    if (marketInfo.GetMarket() == market)
                    {
                        return marketInfo;
                    }
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public virtual List<string> GetMarkets()
        {
            try
            {
                if (_markets.Count == 0)
                {
                    InitMarketsFirstTime(true);
                }
                List<string> marketNames = new List<string>();
                foreach (var info in _markets)
                {
                    marketNames.Add(info.GetMarket());
                }
                return marketNames;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }
        /////////////////////////////////////////////////////
        /////////////// STATIC FUNCTIONS ////////////////////
        /////////////////////////////////////////////////////
        /// <summary>
        /// 
        /// </summary>
        /// 

        private static BrokerDescription[] _brokerDescriptions =
        {
            new BrokerDescription(BrokerLib.Brokers.HitBTC, BrokerType.exchange),
            new BrokerDescription(BrokerLib.Brokers.HitBTC, BrokerType.margin),
            new BrokerDescription(BrokerLib.Brokers.OANDA, BrokerType.margin)
        };

        private static Dictionary<BrokerDescription, Broker> _brokersDict = null;
        private static bool _initialized = false;

        public static BrokerDescription[] GetBrokerDescriptions()
        {
            return _brokerDescriptions;
        }

        public static void InitBrokers(Dictionary<BrokerDescription, List<string>> brokerMarketsStringDict, bool backtest = false)
        {
            try
            {
                if (_initialized && !backtest)
                {
                    return;
                }
                _brokersDict = new Dictionary<BrokerDescription, Broker>();
                foreach (var pair in brokerMarketsStringDict)
                {
                    Broker broker = SwitchBroker(pair.Key, brokerMarketsStringDict);
                    _brokersDict.Add(pair.Key, broker);
                }
                _initialized = true;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public static Broker SwitchBroker(BrokerDescription brokerDescription, Dictionary<BrokerDescription, List<string>> markets, bool ignoreDiscovery = false)
        {
            try
            {
                Broker broker = null;
                if (brokerDescription.BrokerId.Equals(BrokerLib.Brokers.OANDA))
                {
                    if (brokerDescription.BrokerType.Equals(BrokerType.margin) || brokerDescription.BrokerType.Equals(BrokerType.margin_dev))
                    {
                        broker = new OANDA();
                    }
                    if (markets != null && markets.ContainsKey(brokerDescription))
                    {
                        broker.InitMarkets(markets[brokerDescription], ignoreDiscovery);
                    }
                }
                else if (brokerDescription.BrokerId.Equals(BrokerLib.Brokers.HitBTC))
                {
                    if (brokerDescription.BrokerType.Equals(BrokerType.exchange) || brokerDescription.BrokerType.Equals(BrokerType.exchange_dev))
                    {
                        broker = new HitBTC();
                    }
                    else if (brokerDescription.BrokerType.Equals(BrokerType.margin) || brokerDescription.BrokerType.Equals(BrokerType.margin_dev))
                    {
                        broker = new HitBTC(true);
                    }
                    if (markets != null && markets.ContainsKey(brokerDescription))
                    {
                        broker.InitMarkets(markets[brokerDescription], ignoreDiscovery);
                    }
                }
                if (markets == null && broker != null)
                {
                    broker.InitMarketsFirstTime(ignoreDiscovery);
                }
                return broker;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public static Broker DecideBroker(BrokerDescription brokerDescription, bool ignoreDiscovery = false)
        {
            try
            {
                if (!_initialized)
                {
                    if (_brokersDict == null)
                    {
                        _brokersDict = new Dictionary<BrokerDescription, Broker>();
                    }
                    if (!_brokersDict.ContainsKey(brokerDescription))
                    {
                        Broker broker2 = SwitchBroker(brokerDescription, null);
                        if (broker2 != null)
                        {
                            _brokersDict.Add(brokerDescription, broker2);
                        }
                    }
                }
                if (_brokersDict.ContainsKey(brokerDescription))
                {
                    Broker broker = _brokersDict[brokerDescription];
                    return broker;
                }
                else
                {
                    BrokerLib.DebugMessage(String.Format("Broker::DecideBroker({0},{1}) : return broker is null!", brokerDescription.BrokerId, brokerDescription.BrokerType));
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public static List<Broker> GetAllBrokers(bool ignoreDiscovery = false)
        {
            try
            {
                List<Broker> brokers = new List<Broker>();
                Broker broker = null;
                foreach (var brokerDescription in _brokerDescriptions)
                {
                    broker = DecideBroker(brokerDescription);
                    if (broker == null)
                    {
                        continue;
                    }
                    brokers.Add(broker);
                }
                _initialized = true;
                return brokers;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public static List<BrokerView> GetAllBrokerViews(bool ignoreDiscovery = false)
        {
            try
            {
                List<BrokerView> brokerViews = new List<BrokerView>();
                List<Broker> brokers = GetAllBrokers();

                foreach (Broker broker in brokers)
                {
                    brokerViews.Add(new BrokerView((int)broker.GetBrokerId(),
                                    broker.GetBrokerId().ToString(),
                                    (int)broker._authType,
                                    broker.GetMarketType().ToString(),
                                    broker.GetBrokerType().ToString()));
                }
                return brokerViews;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public virtual float CalculateMinimumTransactionAmount(string market)
        {
            try
            {
                string currency = market.Substring(3);
                string Market = null;
                if (currency == "USD")
                {
                    Market = market;
                }
                else
                {
                    Market = currency + "USD";
                }
                if (string.IsNullOrEmpty(market))
                {
                    return 0;
                }
                try
                {
                    List<Candle> candles = GetLastCandles(Market, TimeFrames.H1);

                    if (candles != null && candles.Count > 0)
                    {
                        Candle lastCandle = candles[candles.Count - 1];
                        return 2 / lastCandle.Close;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    Market = "USD" + currency;

                    List<Candle> candles = GetLastCandles(Market, TimeFrames.H1);

                    if (candles != null && candles.Count > 0)
                    {
                        Candle lastCandle = candles[candles.Count - 1];
                        return 2 * lastCandle.Close;
                    }

                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                Candle candle;
                using (BrokerDBContext brokerContext = BrokerDBContext.newDBContextClient())
                {
                    candle = brokerContext.Candles.FirstOrDefault(m => m.Symbol == market && m.TimeFrame == TimeFrames.M1);
                }

                if (candle != null)
                {
                    return 2 * candle.Close;
                }
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

    }
}
using BrokerLib.Brokers;
using BrokerLib.Lib;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace BrokerLib.Market
{

    public class MarketDescription
    {
        public string Market1 { get; private set; }
        public string Market2 { get; private set; }
        public string Market { get { return Market1 + Market2; } }
        public MarketTypes MarketType { get; set; }
        public BrokerType BrokerType { get; set; }

        public MarketDescription(string Market, MarketTypes MarketType, BrokerType BrokerType)
        {
            var markets = Market.Split("_");
            if (markets.Length != 2)
            {
                BrokerLib.DebugMessage($"Market {Market} did not contain _ or 2 two markets.");
            }
            this.Market1 = markets[0];
            this.Market2 = markets[1];
            this.MarketType = MarketType;
            this.BrokerType = BrokerType;
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
                    MarketDescription marketInfo = (MarketDescription)obj;
                    if (Market == marketInfo.Market && 
                        MarketType == marketInfo.MarketType 
                        //&&
                        //BrokerType == marketInfo.BrokerType
                        )
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
            return Market.GetHashCode() + MarketType.GetHashCode() /*+ BrokerType.GetHashCode()*/;
        }
    }
    public class MarketInfo
    {
        public MarketWatch _watch = null;

        private MarketDescription _marketDescription = null;
        public BrokerType _brokerType = BrokerType.exchange;
        public BrokerLib.Brokers _brokerId = BrokerLib.Brokers.HitBTC;
        public float _minimumTransactionAmount = 0;//should be greater than 1 $
        public Broker _broker = null;
        public MarketInfo(string market, Broker broker, bool ignoreDiscovery = false)
        {
            _marketDescription = new MarketDescription(market, broker.GetMarketType(), broker.GetBrokerType());
            _brokerType = broker.GetBrokerType();
            _brokerId = broker.GetBrokerId();
            _broker = broker;
            if (!ignoreDiscovery)
            {
                if (broker.GetMarketType() != MarketTypes.Crypto)
                {
                    InitMarketWatch(broker);
                }
                MinimumTransactionAmount(broker, market);
            }
        }

        public MarketDescription GetMarketDescription()
        {
            return _marketDescription;
        }

        public string GetMarket() 
        {
            return _marketDescription.Market;
        }

        public string GetMarketUnderscore()
        {
            return _marketDescription.Market1 + "_" + _marketDescription.Market2;
        }

        public MarketTypes GetMarketType()
        {
            return _marketDescription.MarketType;
        }

        public void MinimumTransactionAmount(Broker broker, string market) 
        {
            try
            {
                _minimumTransactionAmount = CalculateMinimumTransactionAmount(broker, market);
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public static float CalculateMinimumTransactionAmount(Broker broker, string market)
        {
            try
            {
                return broker.CalculateMinimumTransactionAmount(market);
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public void InitMarketWatch(Broker broker) 
        {
            try
            {
                _watch = new MarketWatch(_marketDescription.MarketType, DiscoverOpenTime(broker, _marketDescription.Market));
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public bool IsAlwaysOpen() 
        {
            try
            {
                return _watch.IsAlwaysOpen();
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return false;
        }

        //public static MarketInfo Construct(BrokerLib.Brokers brokerType, string market, bool ignoreDiscovery = false)
        //{
        //    try
        //    {
        //        Broker broker = Broker.DecideBroker(brokerType);
        //        List<MarketInfo> marketInfos = broker.GetMarketInfos(ignoreDiscovery);
        //        foreach (MarketInfo marketInfo in marketInfos)
        //        {
        //            if (market.Equals(marketInfo._market))
        //            {
        //                return marketInfo;
        //            }
        //        }
        //        return null;
        //    }
        //    catch (Exception e)
        //    {
        //        BrokerLib.DebugMessage(e);
        //    }
        //    return null;
        //}

        public static DateTime DiscoverOpenTime(Broker broker, string market)
        {
            try
            {
                List<Candle> candles = broker.GetCandlesFirstDayOfWeek(market, TimeFrames.H1);

                if (candles != null && candles.Count > 0)
                {
                    Candle firstCandle = candles[0];
                    return firstCandle.Timestamp;
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return DateTime.Today;
        }

        public static string ParseMarket(string market)
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
    }
}

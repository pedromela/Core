using BrokerLib.Lib;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace BrokerLib.Market
{

    public class MarketManager
    {
        private Dictionary<MarketDescription, MarketData> _marketData = null;
        public MarketManager() 
        {
            _marketData = new Dictionary<MarketDescription, MarketData>();
        }

        public void Init(Dictionary<BrokerLib.Brokers, List<MarketInfo>> activeBrokerMarketsDict)
        {
            foreach (var pair in activeBrokerMarketsDict)
            {
                foreach (MarketInfo marketInfo in pair.Value)
                {
                    MarketData marketData = new MarketData(marketInfo);
                    _marketData.Add(marketInfo.GetMarketDescription(), marketData);
                }
            }
        }


        public void AddMarketData(MarketInfo marketInfo, TimeFrames timeFrame, Candles candleData)
        {
            try
            {
                if (!_marketData.ContainsKey(marketInfo.GetMarketDescription()))
                {
                    BrokerLib.DebugMessage(String.Format("MarketManager::AddMarketData({0},{1}) : Market not present in shared data. Initializing dicionary with empty list.", marketInfo.GetMarket(), timeFrame.ToString()));
                    _marketData.Add(marketInfo.GetMarketDescription(), new MarketData(marketInfo));
                }
                if (_marketData[marketInfo.GetMarketDescription()].TimeFrame2Candles.ContainsKey(timeFrame))
                {
                    BrokerLib.DebugMessage(String.Format("MarketManager::AddMarketData({0},{1}) : TimeFrame already present in shared data.", marketInfo.GetMarket(), timeFrame.ToString()));
                    _marketData[marketInfo.GetMarketDescription()].TimeFrame2Candles[timeFrame] = candleData;
                }
                else
                {
                    BrokerLib.DebugMessage(String.Format("MarketManager::AddMarketData({0},{1}) : TimeFrame not present in shared data. Initializing empty CandlesData.", marketInfo.GetMarket(), timeFrame.ToString()));
                    _marketData[marketInfo.GetMarketDescription()].TimeFrame2Candles.Add(timeFrame, candleData);
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public MarketInfo GetMarketInfo(MarketDescription marketDescription, TimeFrames timeFrame)
        {
            try
            {
                MarketData marketData = GetMarketData(marketDescription);

                Candles candleData = marketData.TimeFrame2Candles[timeFrame];
                return candleData.GetMarketInfo();
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public MarketData GetMarketData(MarketDescription marketDescription)
        {
            try
            {
                if (string.IsNullOrEmpty(marketDescription.Market))
                {
                    BrokerLib.DebugMessage(String.Format("MarketManager::GetMarketData() : Market is null."));
                    return null;
                }
                if (!_marketData.ContainsKey(marketDescription))
                {
                    BrokerLib.DebugMessage(String.Format("MarketManager::GetMarketData({0}) : Market not present in shared data.", marketDescription.Market));
                    return null;
                }

                MarketData marketData = _marketData[marketDescription];
                return marketData;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public Candles GetCandleData(MarketDescription marketDesc, TimeFrames timeFrame)
        {
            try
            {
                MarketData marketData = GetMarketData(marketDesc);
                Candles candleData = marketData.TimeFrame2Candles[timeFrame];
                return candleData;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public List<Candle> GetLastCandles(MarketDescription marketDescription, TimeFrames timeFrame)
        {
            try
            {
                Candles candleData = GetCandleData(marketDescription, timeFrame);
                List<Candle> candlesListShared = candleData.GetLastCandles(timeFrame, 1);
                return candlesListShared;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public bool IsLastCandleReversal(MarketDescription marketDescription, TimeFrames timeFrame, TransactionType transactionType)
        {
            try
            {
                Candles candleData = GetCandleData(marketDescription, timeFrame);
                LinkedListNode<Candle> candleNode = candleData.GetCurrentCandleNode();

                if (candleNode == null || candleNode.Previous == null)
                {
                    return false;
                }

                LinkedListNode<Candle> prevCandleNode = candleNode.Previous;

                if (transactionType == TransactionType.buy || transactionType == TransactionType.sellclose)
                {
                    return ReversalCandlesBuy(prevCandleNode.Value, candleNode.Value);
                }
                else if (transactionType == TransactionType.sell || transactionType == TransactionType.buyclose)
                {
                    return ReversalCandlesSell(prevCandleNode.Value, candleNode.Value);
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return false;
        }


        public static bool GreenCandle(Candle candle)
        {
            if (candle.Close - candle.Open > 0)
            {
                return true;
            }
            return false;
        }

        public static bool RedCandle(Candle candle)
        {
            if (candle.Close - candle.Open <= 0)
            {
                return true;
            }
            return false;
        }

        public static bool ReversalCandlesBuy(Candle previous, Candle last)
        {
            if (GreenCandle(last) && RedCandle(previous))// if last is green candle and previous is red
            {
                return true;
            }
            return false;
        }

        public static bool ReversalCandlesSell(Candle previous, Candle last)
        {
            if (RedCandle(last) && GreenCandle(previous))// if last is red candle and previous is green
            {
                return true;
            }
            return false;
        }
    }
}

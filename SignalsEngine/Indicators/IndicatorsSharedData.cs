using BrokerLib.Brokers;
using BrokerLib.Lib;
using BrokerLib.Market;
using BrokerLib.Models;
using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace SignalsEngine.Indicators
{
    public class IndicatorsSharedData
    {
        public Dictionary<MarketDescription, Dictionary<TimeFrames, Dictionary<string, Indicator>>> Market2TimeFrame2Indicators;
        public bool _backtest = false;

        private static IndicatorsSharedData _instance;
        public static IndicatorsSharedData Instance => _instance;

        private IndicatorsSharedData(bool backtest = false)
        : base()
        {
            _backtest = backtest;
            Market2TimeFrame2Indicators = new Dictionary<MarketDescription, Dictionary<TimeFrames, Dictionary<string, Indicator>>>();
        }

        public void Init(Dictionary<Broker, List<MarketInfo>> activeBrokerMarketsDict)
        {
            foreach (var pair in activeBrokerMarketsDict)
            {
                foreach (MarketInfo marketInfo in pair.Value)
                {
                    foreach (TimeFrames timeFrame in Enum.GetValues(typeof(TimeFrames)))
                    {
                        InitPriceDataIndicator(marketInfo, timeFrame, pair.Key, 200);
                    }
                }
            }
        }

        public void InitPriceDataIndicator(MarketInfo marketInfo, TimeFrames timeFrame, Broker broker, int period)
        {
            try
            {
                PriceData priceData = new PriceData(period, timeFrame, broker, marketInfo);
                string name = priceData.ShortName;
                MarketDescription marketDescription = marketInfo.GetMarketDescription();
                if (!Market2TimeFrame2Indicators.ContainsKey(marketDescription))
                {
                    Market2TimeFrame2Indicators.Add(marketDescription, new Dictionary<TimeFrames, Dictionary<string, Indicator>>());
                }
                if (!Market2TimeFrame2Indicators[marketDescription].ContainsKey(timeFrame))
                {
                    Market2TimeFrame2Indicators[marketDescription].Add(timeFrame, new Dictionary<string, Indicator>());
                }

                if (Market2TimeFrame2Indicators[marketDescription][timeFrame].ContainsKey(name))
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorsSharedData::InitPriceDataIndicator({0},{1},{2}) : Indicator {2} was alreaddy present in the dictionary.", marketDescription.Market, name, timeFrame.ToString()));
                }
                else
                {
                    Market2TimeFrame2Indicators[marketDescription][timeFrame].Add(name, priceData);
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public static void InitInstance(Dictionary<Broker, List<MarketInfo>> activeBrokerMarketsDict, bool backtest = false) 
        {
            try
            {
                if (_instance == null || backtest)
                {
                    _instance = new IndicatorsSharedData(backtest);
                    _instance.Init(activeBrokerMarketsDict);
                }
                else
                {
                    SignalsEngine.DebugMessage("IndicatorsSharedData::Init() : Already initialized!");
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public bool ContainsCandleData(MarketDescription marketDescription, TimeFrames timeFrame) 
        {
            try
            {
                return ContainsIndicator(marketDescription, timeFrame, "i_price:200");
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }

        public bool ContainsIndicator(MarketDescription marketDesc, TimeFrames timeFrame, string name)
        {
            try
            {
                if (!Market2TimeFrame2Indicators.ContainsKey(marketDesc))
                {
                    return false;
                }
                if (!Market2TimeFrame2Indicators[marketDesc].ContainsKey(timeFrame))
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorsSharedData::ContainsIndicator({0},{1}) : TimeFrame not present in shared data.", marketDesc.Market, timeFrame.ToString()));
                    return false;
                }
                if (Market2TimeFrame2Indicators[marketDesc][timeFrame].ContainsKey(name))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }

        public bool ContainsIndicators(MarketDescription marketDesc, TimeFrames timeFrame)
        {
            try
            {
                if (!Market2TimeFrame2Indicators.ContainsKey(marketDesc))
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorsSharedData::ContainsIndicator({0},{1}) : Market not present in shared data.", marketDesc.Market, timeFrame.ToString()));
                    return false;
                }
                if (Market2TimeFrame2Indicators[marketDesc].ContainsKey(timeFrame))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }

        public void AddIndicators(MarketDescription marketDesc, TimeFrames timeFrame, Dictionary<string, Indicator> indicators)
        {
            try
            {
                foreach (Indicator indicator in indicators.Values)
                {
                    AddIndicator(marketDesc, indicator.ShortName, timeFrame, indicator);
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void AddIndicator(MarketDescription marketDesc, string name, TimeFrames timeFrame, Indicator indicator)
        {
            try
            {
                if (!Market2TimeFrame2Indicators.ContainsKey(marketDesc))
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorsSharedData::AddIndicator({0},{1},{2}) : Market not present in shared data. Initializing dicionary with empty list.", marketDesc.Market, name, timeFrame.ToString()));
                    Market2TimeFrame2Indicators.Add(marketDesc, new Dictionary<TimeFrames, Dictionary<string, Indicator>>());
                }
                if (!Market2TimeFrame2Indicators[marketDesc].ContainsKey(timeFrame))
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorsSharedData::AddIndicator({0},{1},{2}) : TimeFrame not present in shared data. Initializing dicionary with empty list.", marketDesc.Market, name, timeFrame.ToString()));
                    Market2TimeFrame2Indicators[marketDesc].Add(timeFrame, new Dictionary<string, Indicator>());
                }
                if (Market2TimeFrame2Indicators[marketDesc][timeFrame].ContainsKey(name) && !_backtest)
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorsSharedData::AddIndicator({0},{1},{2}) : Indicator already present present in shared data.", marketDesc.Market, name, timeFrame.ToString()));
                    return;
                }
                if (_backtest)
                {
                    Market2TimeFrame2Indicators[marketDesc][timeFrame][name] = indicator;
                }
                else
                {
                    Market2TimeFrame2Indicators[marketDesc][timeFrame].Add(name, indicator);
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public Indicator GetIndicator(MarketDescription marketDesc, string name, TimeFrames timeFrame)
        {
            try
            {
                if (name.Contains("i_VWAP"))
                {
                    timeFrame = TimeFrames.M1;
                }
                if (!Market2TimeFrame2Indicators.ContainsKey(marketDesc))
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorsSharedData::GetIndicator({0},{1},{2}) : Market not present in shared data.", marketDesc.Market, name, timeFrame.ToString()));
                    return null;
                }
                if (!Market2TimeFrame2Indicators[marketDesc].ContainsKey(timeFrame))
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorsSharedData::GetIndicator({0},{1},{2}) : TimeFrame not present in shared data.", marketDesc.Market, name, timeFrame.ToString()));
                    return null;
                }
                if (!Market2TimeFrame2Indicators[marketDesc][timeFrame].ContainsKey(name))
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorsSharedData::GetIndicator({0},{1},{2}) : Indicator not present in shared data.", marketDesc.Market, name, timeFrame.ToString()));
                    return null;
                }

                Indicator indicator = Market2TimeFrame2Indicators[marketDesc][timeFrame][name];
                return indicator;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Dictionary<string, Indicator> GetIndicators(MarketDescription marketDesc, TimeFrames timeFrame)
        {
            try
            {
                if (!Market2TimeFrame2Indicators.ContainsKey(marketDesc))
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorsSharedData::GetIndicators({0},{1}) : Market not present in shared data.", marketDesc, timeFrame.ToString()));
                    return null;
                }
                if (!Market2TimeFrame2Indicators[marketDesc].ContainsKey(timeFrame))
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorsSharedData::GetIndicators({0},{1}) : TimeFrame not present in shared data.", marketDesc, timeFrame.ToString()));
                    return null;
                }

                Dictionary<string, Indicator> indicators = Market2TimeFrame2Indicators[marketDesc][timeFrame];
                return indicators;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        ///////////////////////////////////////
        ///////////////////////////////////////

        public PriceData GetCandleData(MarketDescription marketDesc, TimeFrames timeFrame)
        {
            try
            {
                PriceData price =(PriceData) GetIndicator(marketDesc, "i_price:200", timeFrame);
                return price;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public List<Candle> GetLastCandles(MarketDescription marketDescription, TimeFrames timeFrame)
        {
            try
            {
                PriceData candleData = GetCandleData(marketDescription, timeFrame);
                List<Candle> candlesListShared = candleData.GetLastCandles(timeFrame, 1);
                return candlesListShared;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }


        public MarketInfo GetMarketInfo(MarketDescription marketDescription, TimeFrames timeFrame)
        {
            try
            {
                PriceData candleData = GetCandleData(marketDescription, timeFrame);

                return candleData.GetMarketInfo();
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }


        //////////////////////////////////////////////////////////////
        ///////////////////////// AUX CODE ///////////////////////////
        //////////////////// TO BE REMOVED/REFACTORED ////////////////

        public bool IsTrendUp(MarketDescription marketDescription, TimeFrames timeFrame, TransactionType transactionType)
        {
            try
            {
                Indicator sma200 = GetIndicator(marketDescription, "i_SMA:200", timeFrame);
                return sma200.Slope() > 0;
                //PriceData candleData = GetCandleData(marketDescription, timeFrame);
                //LinkedListNode<Dictionary<string, Candle>> candleNode = candleData.GetCurrentCandleNode();

                //if (candleNode == null || candleNode.Previous == null)
                //{
                //    return false;
                //}

                //LinkedListNode<Dictionary<string, Candle>> prevCandleNode = candleNode.Previous;

                //if (transactionType == TransactionType.buy || transactionType == TransactionType.sellclose)
                //{
                //    return ReversalCandlesBuy(prevCandleNode.Value["middle"], candleNode.Value["middle"]);
                //}
                //else if (transactionType == TransactionType.sell || transactionType == TransactionType.buyclose)
                //{
                //    return ReversalCandlesSell(prevCandleNode.Value["middle"], candleNode.Value["middle"]);
                //}
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }

        public bool IsTrendDown(MarketDescription marketDescription, TimeFrames timeFrame, TransactionType transactionType)
        {
            Indicator sma200 = GetIndicator(marketDescription, "i_SMA:200", timeFrame);
            return sma200.Slope() < 0;
            //try
            //{
            //    PriceData candleData = GetCandleData(marketDescription, timeFrame);
            //    LinkedListNode<Dictionary<string, Candle>> candleNode = candleData.GetCurrentCandleNode();

            //    if (candleNode == null || candleNode.Previous == null)
            //    {
            //        return false;
            //    }

            //    LinkedListNode<Dictionary<string, Candle>> prevCandleNode = candleNode.Previous;

            //    if (transactionType == TransactionType.buy || transactionType == TransactionType.sellclose)
            //    {
            //        return ReversalCandlesBuy(prevCandleNode.Value["middle"], candleNode.Value["middle"]);
            //    }
            //    else if (transactionType == TransactionType.sell || transactionType == TransactionType.buyclose)
            //    {
            //        return ReversalCandlesSell(prevCandleNode.Value["middle"], candleNode.Value["middle"]);
            //    }
            //}
            //catch (Exception e)
            //{
            //    SignalsEngine.DebugMessage(e);
            //}
            //return false;
        }

        public bool IsLastCandleReversal(MarketDescription marketDescription, TimeFrames timeFrame, TransactionType transactionType)
        {
            try
            {
                PriceData candleData = GetCandleData(marketDescription, timeFrame);
                LinkedListNode<Dictionary<string, Candle>> candleNode = candleData.GetCurrentCandleNode();

                if (candleNode == null || candleNode.Previous == null)
                {
                    return false;
                }

                LinkedListNode<Dictionary<string, Candle>> prevCandleNode = candleNode.Previous;

                if (transactionType == TransactionType.buy || transactionType == TransactionType.sellclose)
                {
                    return ReversalCandlesBuy(prevCandleNode.Value["middle"], candleNode.Value["middle"]);
                }
                else if (transactionType == TransactionType.sell || transactionType == TransactionType.buyclose)
                {
                    return ReversalCandlesSell(prevCandleNode.Value["middle"], candleNode.Value["middle"]);
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
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

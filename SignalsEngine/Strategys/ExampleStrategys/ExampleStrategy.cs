using BrokerLib.Lib;
using System;
using System.Collections.Generic;
using System.Text;
using static BrokerLib.BrokerLib;
using Utils.Utils;

namespace SignalsEngine.Strategys.ExampleStrategys
{
    public class ExampleStrategy
    {
        public static ConditionStrategy GetRandomStrategy() 
        {
            try
            {
                List<ConditionStrategy> strategies = GetAllExampleStrategies();
                int idx = RandomGenerator.RandomNumber(0, strategies.Count);
                return strategies[idx];
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public static List<ConditionStrategy> GetAllExampleStrategies() 
        {
            try
            {
                List<ConditionStrategy> strategies = new List<ConditionStrategy>();
                ConditionStrategy strategy = new BBStrategy(200, 2);
                strategies.Add(strategy);
                strategy = new BBStrategy(100, 3);
                strategies.Add(strategy);
                strategy = new DoubleBBStrategy(100, 3, 200, 2);
                strategies.Add(strategy);
                strategy = new BBStrategyOnReversal();
                strategies.Add(strategy);
                strategy = new BBVWAPTrendStrategy(200, 2);
                strategies.Add(strategy);
                strategy = new BBVWAPStrategy(200, 2);
                strategies.Add(strategy);
                strategy = new VWAPMAStrategy(0);
                strategies.Add(strategy);
                strategy = new VWAPMAStrategy(0.05f);
                strategies.Add(strategy);
                strategy = new VWAPMAStrategy(0.01f);
                strategies.Add(strategy);
                strategy = new VWAPMAStrategy(0.0075f);
                strategies.Add(strategy);
                strategy = new VWAPMAOnReversalStrategy();
                strategies.Add(strategy);
                strategy = new MomentumStrategy();
                strategies.Add(strategy);
                strategy = new TradingRushVWAPBBStrategy(200, 2);
                strategies.Add(strategy);
                strategy = new TradingRushVWAPBBStrategy(100, 3);
                strategies.Add(strategy);
                strategy = new VWAPBBChannelStrategy(200, 2);
                strategies.Add(strategy);
                strategy = new ParabolicSARStrategy(200);
                strategies.Add(strategy);
                strategy = new VisionAlgoStrategy(200);
                strategies.Add(strategy);

                return strategies;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }
    }
}

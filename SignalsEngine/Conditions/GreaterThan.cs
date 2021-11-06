using BotLib;
using SignalsEngine.Indicators;
using System;

namespace SignalsEngine.Conditions
{
    public class GreaterThan : SignalCondition
    {
        public GreaterThan(Indicator indicator, Indicator compareIndicator, SignalState defaultSignalState)
        : base(indicator, compareIndicator, defaultSignalState)
        {
        }

        public override bool True()
        {
            try
            {
                if (_indicator.GetLastClose() > _compareIndicator.GetLastClose())
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
    }
}

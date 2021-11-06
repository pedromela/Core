using BotLib;
using SignalsEngine.Indicators;
using System;

namespace SignalsEngine.Conditions
{
    public class CrossingUp : Crossing
    {
        public CrossingUp(Indicator indicator, Indicator compareIndicator, SignalState defaultSignalState)
        : base(indicator, compareIndicator, defaultSignalState)
        {
            //DecideSide();
        }

        public override bool True()
        {
            try
            {
                DecideSide();
                if (IsDown)
                {
                    if (_indicator.GetLastClose() > _compareIndicator.GetLastClose())
                    {
                        return true;
                    }
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

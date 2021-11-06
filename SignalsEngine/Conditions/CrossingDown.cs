using BotLib;
using SignalsEngine.Indicators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalsEngine.Conditions
{
    public class CrossingDown : Crossing
    {
        public CrossingDown(Indicator indicator, Indicator compareIndicator, SignalState defaultSignalState)
        : base(indicator, compareIndicator, defaultSignalState)
        {
            //DecideSide();
        }

        public override bool True()
        {
            try
            {
                DecideSide();
                if (!IsDown)
                {
                    if (_indicator.GetLastClose() < _compareIndicator.GetLastClose())
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

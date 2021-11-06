using BotLib;
using SignalsEngine.Indicators;
using System;

namespace SignalsEngine.Conditions
{
    public class Crossing : SignalCondition
    {
        public bool IsDown = false;
        public Crossing(Indicator indicator, Indicator compareIndicator, SignalState defaultSignalState)
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
                else
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
        public virtual void DecideSide()
        {
            try
            {
                if (_indicator.GetLastClose() > _compareIndicator.GetLastClose())
                {
                    IsDown = false;
                }
                else
                {
                    IsDown = true;
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }
    }
}

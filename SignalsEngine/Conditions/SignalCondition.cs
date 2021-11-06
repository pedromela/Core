using BotLib;
using SignalsEngine.Indicators;
using System;

namespace SignalsEngine.Conditions
{
    public abstract class SignalCondition : ICondition
    {
        public SignalState _defaultSignalState { get; protected set; }
        public Indicator _indicator = null;
        public Indicator _compareIndicator = null;
        public Indicator _compareIndicator2 = null;

        public SignalCondition(Indicator indicator, SignalState defaultSignalState) 
        {
            _indicator = indicator;
            _defaultSignalState = defaultSignalState;
        }

        public SignalCondition(Indicator indicator, Indicator compareIndicator, SignalState defaultSignalState)
        : this(indicator, defaultSignalState)
        {
            _compareIndicator = compareIndicator;
        }

        public SignalCondition(Indicator indicator, Indicator compareIndicator, Indicator compareIndicator2, SignalState defaultSignalState)
        : this(indicator, compareIndicator, defaultSignalState)
        {
            _compareIndicator2 = compareIndicator2;
        }

        public virtual bool True() 
        {
            try
            {

            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }
    }
}

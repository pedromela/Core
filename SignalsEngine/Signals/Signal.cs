using BotLib;
using BotLib.Models;
using BrokerLib.Models;
using SignalsEngine.Conditions;
using SignalsEngine.Indicators;
using System;
using System.Collections.Generic;

namespace SignalsEngine.Signals
{
    public class Signal
    {
        public string _name { get; protected set; }
        public SignalState _defaultSignalState { get; protected set; }
        public LinkedList<SignalState> _values { get; protected set; }
        public List<ICondition> _conditions { get; protected set; }
        private float _fitness;
        public Signal(string name, SignalState defaultSignalState = SignalState.Buy, float fitness = 0.5f) 
        {
            _name = name;
            _fitness = fitness;
            _defaultSignalState = defaultSignalState;
            _values = new LinkedList<SignalState>();
            _conditions = new List<ICondition>();
        }

        public virtual SignalState ProcessNext()
        {
            try
            {
                int max = _conditions.Count;
                float nextFitness = 0;
                foreach (var condition in _conditions)
                {
                    if (condition.True())
                    {
                        nextFitness++;
                    }
                }
                if (nextFitness/max > _fitness )
                {
                    return _defaultSignalState;
                }
                return SignalState.DoNothing;

            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return SignalState.DoNothing;
        }
    }
}

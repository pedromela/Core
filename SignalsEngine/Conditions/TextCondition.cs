using System;
using System.Collections.Generic;
using UtilsLib.Utils;
using SignalsEngine.Indicators;
using static BrokerLib.BrokerLib;
using BrokerLib.Lib;
using BrokerLib.Market;
using BrokerLib.Models;

namespace SignalsEngine.Conditions
{
    public class IndicatorDescription 
    {
        public string _name;
        public string _line;
        public string _input;


        public IndicatorDescription(string name, string line, string input) 
        {
            _name = name;
            _line = line;
            _input = input;
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
                    IndicatorDescription indicatorDescription = (IndicatorDescription)obj;
                    if (_name == indicatorDescription._name &&
                        _line == indicatorDescription._line &&
                        _input == indicatorDescription._line)
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

        public override int GetHashCode()
        {
            int hashCode = 0;
            if (!string.IsNullOrEmpty(_name))
            {
                hashCode += _name.GetHashCode();
            }
            if (!string.IsNullOrEmpty(_line))
            {
                hashCode += _line.GetHashCode();
            }
            if (!string.IsNullOrEmpty(_input))
            {
                hashCode += _input.GetHashCode();
            }
            return hashCode;
        }
    }

    public class FunctionDescription
    {
        public string _functionName;
        public string _indicatorName;

        public FunctionDescription(string functionName, string indicatorName)
        {
            _functionName = functionName;
            _indicatorName = indicatorName;
        }
    }

    public class TextCondition : ICondition
    {
        public const string INDICATOR_PREFIX = "i_";
        public const string FLAG_PREFIX = "b_";
        public const string FUNCTION_PREFIX = "f_";
        LogicExpressionParser _logicExpressionParser;
        LogicExpression _logicExpression;
        public string _expression;
        Dictionary<IndicatorDescription, Indicator> _indicatorsDictionary;
        Dictionary<FunctionDescription, Indicator> _functionsDictionary;
        Dictionary<string, bool> _flagsDictionary;
        TimeFrames _timeFrame;
        public TransactionType _transactionType;
        MarketInfo _marketInfo;
        bool _parseUpdated = false;

        public TextCondition(MarketInfo marketInfo, string expression, TransactionType transactionType, TimeFrames timeFrame = TimeFrames.H1) 
        {
            _marketInfo = marketInfo;
            _expression = expression;
            _timeFrame = timeFrame;
            _transactionType = transactionType;
            _logicExpressionParser = new LogicExpressionParser();
            _logicExpression = _logicExpressionParser.Parse(_expression);
            _indicatorsDictionary = new Dictionary<IndicatorDescription, Indicator>();
            _flagsDictionary = new Dictionary<string, bool>();
            _functionsDictionary = new Dictionary<FunctionDescription, Indicator>();
        }

        public void ParseExpression() 
        {
            if (!_parseUpdated)
            {
                _parseUpdated = true;
                ParseIndicators();
                ParseFlags();
                ParseFunctions();
            }
        }

        public void ApplyValues() 
        {
            ApplyFlagValues(); 
            ApplyIndicatorValues();
            ApplyFunctionValues();
        }

        public void ParseFunctions()
        {
            try
            {
                var tokens = _expression.Split(" ");
                foreach (var token in tokens)
                {
                    if (token.StartsWith(FUNCTION_PREFIX))
                    {
                        string functionDescription = token;
                        FunctionDescription funcDesc = ParseFunctionDescription(functionDescription);
                        IndicatorDescription indicatorDesc = ParseIndicatorDescription("i_" + funcDesc._indicatorName);
                        Indicator indicator = GetIndicatorFromDescription(indicatorDesc);

                        if (_functionsDictionary.ContainsKey(funcDesc))
                        {
                            _functionsDictionary[funcDesc] = indicator;
                        }
                        else
                        {
                            _functionsDictionary.Add(funcDesc, indicator);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
        }

        public FunctionDescription ParseFunctionDescription(string functionDescription)
        {
            try
            {
                functionDescription = functionDescription.Remove(0, 2);
                if (functionDescription.Contains("_"))
                {
                    var tokens = functionDescription.Split("_");
                    if (tokens.Length != 2)
                    {
                        BrokerLib.BrokerLib.DebugMessage(String.Format("TextCondition::ParseFunctionDescription({0}) : Could not parse function description.", functionDescription));
                        return null;
                    }
                    return new FunctionDescription(tokens[0], tokens[1]);
                }
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public float CalculateFunction(FunctionDescription functionDescription)
        {
            try
            {
                switch (functionDescription._functionName)
                {
                    case "slope":
                        return _functionsDictionary[functionDescription].Slope();
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return 0;
        }

        public void InitFlags() 
        {
            _flagsDictionary.Add("onreversal", false);
            _flagsDictionary.Add("ontrendup", false);
            _flagsDictionary.Add("ontrenddown", false);

        }

        public void ParseFlags()
        {
            try
            {
                var tokens = _expression.Split(" ");
                foreach (var token in tokens)
                {
                    if (token.StartsWith(FLAG_PREFIX))
                    {
                        string flagDescription = token;
                        string flagDesc = ParseFlagDescription(flagDescription);
                        if (_flagsDictionary.ContainsKey(flagDesc))
                        {
                            _flagsDictionary[flagDesc] = true;
                        }
                        else
                        {
                            _flagsDictionary.Add(flagDesc, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
        }

        public string ParseFlagDescription(string flagDescription)
        {
            try
            {
                flagDescription = flagDescription.Remove(0, 2);
                return flagDescription;
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public void ParseIndicators() 
        {
            try
            {
                var tokens = _expression.Split(" ");
                foreach (var token in tokens)
                {
                    if (token.StartsWith(INDICATOR_PREFIX))
                    {
                        string indicatorDescription = token;
                        IndicatorDescription indicatorDesc = ParseIndicatorDescription(indicatorDescription);

                        Indicator indicator = GetIndicatorFromDescription(indicatorDesc);
                        if (indicator == null)
                        {
                            BrokerLib.BrokerLib.DebugMessage(String.Format("TextCondition::ParseIndicators() : Initialization failed."));
                            continue;
                        }
                        if (_indicatorsDictionary.ContainsKey(indicatorDesc))
                        {
                            continue;
                        }
                        _indicatorsDictionary.Add(indicatorDesc, indicator);
                    }
                }
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
        }

        public Indicator GetIndicatorFromDescription(IndicatorDescription indicatorDescription) 
        {
            try
            {
                if (string.IsNullOrEmpty(indicatorDescription._input))
                {
                    return IndicatorsSharedData.Instance.GetIndicator(_marketInfo.GetMarketDescription(), indicatorDescription._name, _timeFrame);
                }
                else
                {
                    return IndicatorsSharedData.Instance.GetIndicator(_marketInfo.GetMarketDescription(), indicatorDescription._name + ";" +  indicatorDescription._input, _timeFrame);
                }
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public IndicatorDescription ParseIndicatorDescription(string indicatorDescription) 
        {
            try
            {
                if (indicatorDescription.Contains(";"))
                {
                    var tokens = indicatorDescription.Split(";");
                    if (tokens.Length != 2)
                    {
                        BrokerLib.BrokerLib.DebugMessage(String.Format("TextCondition::IndicatorDescription({0}) : Could not parse indicator description.", indicatorDescription));
                        return null;
                    }
                    IndicatorDescription indicatorDesc = ParseIndicatorDescription(tokens[0]);
                    IndicatorDescription inputIndicatorDesc = ParseIndicatorDescription(tokens[1]);
                    indicatorDesc._input = inputIndicatorDesc._name;
                    return indicatorDesc;
                }
                else if (indicatorDescription.Contains("_"))
                {
                    indicatorDescription = indicatorDescription.Remove(0, 2);
                    var tokens = indicatorDescription.Split("_");
                    if (tokens.Length != 2)
                    {
                        BrokerLib.BrokerLib.DebugMessage(String.Format("TextCondition::IndicatorDescription({0}) : Could not parse indicator description.", indicatorDescription));
                        return null;
                    }
                    return new IndicatorDescription(INDICATOR_PREFIX + tokens[0], tokens[1], null);
                }
                return new IndicatorDescription(INDICATOR_PREFIX + indicatorDescription, "middle", null);

            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public bool CalculateFlag(string flag) 
        {
            try
            {
                switch (flag)
                {
                    case "onreversal":
                        return IndicatorsSharedData.Instance.IsLastCandleReversal(_marketInfo.GetMarketDescription(), _timeFrame, _transactionType);
                    case "ontrendup":
                        return IndicatorsSharedData.Instance.IsTrendUp(_marketInfo.GetMarketDescription(), _timeFrame, _transactionType);
                    case "ontrenddown":
                        return IndicatorsSharedData.Instance.IsTrendDown(_marketInfo.GetMarketDescription(), _timeFrame, _transactionType);
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return false;
        }

        public void ApplyFunctionValues()
        {
            try
            {
                foreach (var pair in _functionsDictionary)
                {
                    string idx = FUNCTION_PREFIX + pair.Key._functionName + "_" + pair.Key._indicatorName;
                    _logicExpression[idx].Set(CalculateFunction(pair.Key));
                }
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
        }

        public void ApplyFlagValues()
        {
            try
            {
                foreach (var pair in _flagsDictionary)
                {
                    _logicExpression[FLAG_PREFIX + pair.Key].Set(CalculateFlag(pair.Key));
                }
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
        }
        public string BuildIndicatorDictionaryIndex(IndicatorDescription indicatorDescription) 
        {
            try
            {
                if (string.IsNullOrEmpty(indicatorDescription._input))
                {
                    return indicatorDescription._name + "_" + indicatorDescription._line;
                }
                return indicatorDescription._name + "_" + indicatorDescription._line + ";" + indicatorDescription._input;
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return null;
        }

        public void ApplyIndicatorValues() 
        {
            try
            {
                foreach (var pair in _indicatorsDictionary)
                {
                    Candle candle = pair.Value.GetLastValue(pair.Key._line);
                    if (candle == null || candle.Close == 0)
                    {
                        continue;
                    }
                    _logicExpression[BuildIndicatorDictionaryIndex(pair.Key)].Set(candle.Close);
                }
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
        }

        public bool True()
        {
            try
            {
                ParseExpression();
                ApplyValues();
                if (_logicExpression.GetResult())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                BrokerLib.BrokerLib.DebugMessage(e);
            }
            return false;
        }
    }
}

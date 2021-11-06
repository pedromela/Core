using BrokerLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilsLib.Utils;

namespace SignalsEngine.Indicators
{
    public class IndicatorLines
    {
        public bool AllowInconsistentData  { get; protected set; }
        public LinkedList<Dictionary<string, Candle>> Values { get; protected set; }
        public Dictionary<string, int> Lines { get; protected set; }
        public int Period { get; set; }

        public IndicatorLines(int Period, List<string> Lines, bool AllowInconsistentData = false) 
        {
            this.Period = Period;
            this.Lines = new Dictionary<string, int>();
            this.AllowInconsistentData = AllowInconsistentData;
            for (int i = 0; i < Lines.Count; i++)
            {
                this.Lines.Add(Lines[i], i);
            }
            this.Values = new LinkedList<Dictionary<string, Candle>>();
        }

        public void Clear()
        {
            Values.Clear();
        }


        public LinkedList<Dictionary<string, Candle>> GetValues()
        {
            return Values;
        }

        public Dictionary<string, int> GetLines() 
        {
            return Lines;
        }

        public void AddCurrentValue(Candle value, string line = "middle")
        {
            try
            {
                var dictValue = GetLastValue();
                if (dictValue.ContainsKey(line))
                {
                    dictValue[line] = value;
                }
                else
                {
                    dictValue.Add(line, value);
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void AddLastValue(Candle value, string line = "middle")
        {
            try
            {
                if (Lines.Count > 1 && !AllowInconsistentData)
                {
                    SignalsEngine.DebugMessage(String.Format("IndicatorLines::AddLastValue() : AddLastValue(Candle) failed because Indicator contains more than 1 Line."));
                    return;
                }
                Dictionary<string, Candle> valueList = new Dictionary<string, Candle>();
                valueList.Add(line, value);
                AddLastValue(valueList);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void AddLastValue(Dictionary<string, Candle> value)
        {
            try
            {
                if (Period > 0 && Values.Count >= Period)
                {
                    RemoveFirst();
                }
                Values.AddLast(value);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void AddValues(LinkedList<Dictionary<string ,Candle>> values)
        {
            try
            {
                Values = values;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void RemoveFirst()
        {
            try
            {
                if (Values.Count > 0)
                {
                    Values.RemoveFirst();
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public void RemoveLast()
        {
            try
            {
                if (Values.Count > 0)
                {
                    Values.RemoveLast();
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public Dictionary<string, Candle> ValueAt(int idx)
        {
            try
            {
                return Values.ElementAt(idx);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Candle ValueAt(int idx, string name = "middle")
        {
            try
            {
                Dictionary<string, Candle> valueList = ValueAt(idx);
                //int linesIdx = Lines[name];
                return valueList[name];
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public int Count()
        {
            try
            {
                return Values.Count;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return 0;
        }

        public Dictionary<string, Candle> GetLastValue()
        {
            try
            {
                if (Values.Last == null)
                {
                    SignalsEngine.DebugMessage(String.Format("Indicator::GetLastValue() : {0} Error last value is NULL."));
                    return null;
                }
                return Values.Last.Value;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public float GetLastClose(string name = "middle")
        {
            try
            {
                return GetLastValue(name).Close;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return 0;
        }

        public Candle GetLastValue(string name = "middle")
        {
            try
            {
                if (Lines.ContainsKey(name) && Count() > 0)
                {
                    Dictionary<string, Candle> valueList = Values.Last.Value;
                    int idx = Lines[name];
                    if (valueList != null && valueList.Count < idx)
                    {
                        SignalsEngine.DebugMessage(String.Format("Indicator::GetLastValue() : {0} Error index is greater than the values lenght.", name));
                    }
                    return valueList[name];
                }
                else
                {
                    SignalsEngine.DebugMessage(String.Format("Indicator::GetLastValue() : {0} line does not exist in the Lines dictionary.", name));
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public Candle GetValue(int countFromLast, string name = "middle")
        {
            try
            {
                if (Values.Count - countFromLast >= 0)
                {
                    var valueList = Values.ElementAt(Values.Count - countFromLast);
                    return valueList[name];
                }
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public LinkedListNode<Dictionary<string, Candle>> GetFirstValueNode()
        {
            try
            {
                if (Values.First == null)
                {
                    SignalsEngine.DebugMessage(String.Format("Indicator::GetFirstValueNode() : Error first is null."));
                    return null;
                }
                return Values.First;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public LinkedListNode<Dictionary<string, Candle>> GetLastValueNode()
        {
            try
            {
                if (Values.Last == null)
                {
                    SignalsEngine.DebugMessage(String.Format("Indicator::GetFirstValueNode() : Error last is null."));
                    return null;
                }
                return Values.Last;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return null;
        }

        public virtual float Slope(string name = "middle")
        {
            try
            {
                int count = Values.Count > Period ? Period : Values.Count;
                float[] x = new float[count];
                float[] y = new float[count];
                LinkedListNode<Dictionary<string, Candle>> last = Values.Last;

                for (int i = count-1; i >= 0 && last != null; i--)
                {
                    x[i] = i + 1;
                    y[i] = last.Value[name].Close;
                    last = last.Previous;
                }

                return LinearRegression.Slope(x, y);
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return 0;
        }


    }
}

using Newtonsoft.Json;
using System;

namespace UtilsLib.Utils
{
    public class Parser
    {
        public Parser()
        {

        }

        public virtual string Parse()
        {
            try
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
            return null;
        }

        public static string ParseFloatString(string floatStr) 
        {
            string[] toks;
            if (floatStr.Contains("."))
            {
                toks = floatStr.Split(".");
                if (toks[1].Length > 8)
                {
                    toks[1] = toks[1].Substring(8);
                    floatStr = toks[0] + "." + toks[1];
                }
            }
            else if (floatStr.Contains(","))
            {
                toks = floatStr.Split(",");
                if (toks[1].Length > 8)
                {
                    toks[1] = toks[1].Substring(8);
                    floatStr = toks[0] + "." + toks[1];
                }
            }


            return floatStr;
        }
        public static float ParseFloat(string floatStr) 
        {
            try
            {
                //floatStr = ParseFloatString(floatStr);

                float f = 0.0f;
                try
                {
                    f = float.Parse(floatStr);
                }
                catch (Exception)
                {
                    if (floatStr.Contains("."))
                    {
                        floatStr = floatStr.Replace(".", ",");
                    }
                    else if (floatStr.Contains(","))
                    {
                        floatStr = floatStr.Replace(",", ".");
                    }

                    f = float.Parse(floatStr);
                }

                return f;
            }
            catch (Exception)
            {
                UtilsLib.DebugMessage("Parser.ParseFloat(string) : Second parse atempt failed.");
                //UtilsLib.DebugMessage(e);
            }
            return 0;
        }
    }
}

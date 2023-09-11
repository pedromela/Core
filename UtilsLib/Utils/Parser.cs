using Newtonsoft.Json;
using System;
using System.Globalization;

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

        public static float ParseFloat(string floatStr) 
        {
            if (floatStr.Contains(","))
            {
                floatStr = floatStr.Replace(",", ".");
            }

            return float.Parse(floatStr, CultureInfo.InvariantCulture);
        }
    }
}

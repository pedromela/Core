using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLib
{
    public class DataLib
    {
        public static bool Verbose = true;

        public static bool Logging = false;

        public static Logger log = LogManager.GetCurrentClassLogger();

        public static void DebugMessage(string msg)
        {
            if (DataLib.Logging)
            {
                DataLib.log.Debug(msg);

            }
            if (DataLib.Verbose)
            {
                Console.WriteLine(msg);
            }
        }

        public static void DebugMessage(Exception e)
        {
            if (DataLib.Logging)
            {
                DataLib.log.Debug(e);

            }
            if (DataLib.Verbose)
            {
                Console.WriteLine(e);
            }
        }
    }
}

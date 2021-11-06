using System;
using NLog;

namespace UtilsLib.Utils
{
    public class MyLogger
    {
        public static bool Verbose = true;
        public static bool Logging = true;
        public static Logger log = LogManager.GetCurrentClassLogger();

        public MyLogger()
        {

        }

        public static void DebugMessage(string msg)
        {
            if (Logging)
            {
                log.Debug(msg);

            }
            if (Verbose)
            {
                Console.WriteLine(msg);
            }
        }

        public static void DebugMessage(Exception e)
        {
            if (Logging)
            {
                log.Debug(e);

            }
            if (Verbose)
            {
                Console.WriteLine(e);
            }
        }
    }
}

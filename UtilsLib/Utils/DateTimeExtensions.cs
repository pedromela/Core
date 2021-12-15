using System;

namespace UtilsLib.Utils
{
    public static class DateTimeExtensions
    {
        public static DateTime Normalize(DateTime date, int timeFrame) 
        {
            date = date.AddSeconds(-date.Second);
            date = date.AddMilliseconds(-date.Millisecond);
            date = date.AddMinutes(timeFrame - date.Minute % timeFrame);
            return date;
        }

        public static bool IsMidnight(DateTime date) 
        {
            if (date.Hour == 0 && date.Minute == 0 && date.Second == 0)
            {
                return true;
            }
            return false;
        }
    }
}

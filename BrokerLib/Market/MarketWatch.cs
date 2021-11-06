using System;
using System.Collections.Generic;
using static BrokerLib.BrokerLib;

namespace BrokerLib.Lib
{
    public class DayTime
    {
        public DayOfWeek _day;
        public TimeSpan _openTime;
        public TimeSpan _closeTime;

        public DayTime(DayOfWeek day, TimeSpan openTime, TimeSpan closeTime) 
        {
            _day = day;
            _openTime = openTime;
            _closeTime = closeTime;
        }
    }
    public class MarketWatch
    {
        private bool _alwaysOpen = false;

        private List<DayTime> _openDays = null;

        public MarketWatch(MarketTypes marketType, DateTime openTime) 
        {
            _openDays = new List<DayTime>();

            TimeSpan beforeMidnight = new TimeSpan(23, 59, 0);
            TimeSpan midnight = TimeSpan.Zero;

            if (marketType == MarketTypes.Crypto)
            {
                _alwaysOpen = true;
                _openDays.Add(new DayTime(DayOfWeek.Monday, midnight, beforeMidnight));
                _openDays.Add(new DayTime(DayOfWeek.Tuesday, midnight, beforeMidnight));
                _openDays.Add(new DayTime(DayOfWeek.Wednesday, midnight, beforeMidnight));
                _openDays.Add(new DayTime(DayOfWeek.Thursday, midnight, beforeMidnight));
                _openDays.Add(new DayTime(DayOfWeek.Friday, midnight, beforeMidnight));
                _openDays.Add(new DayTime(DayOfWeek.Saturday, midnight, beforeMidnight));
                _openDays.Add(new DayTime(DayOfWeek.Sunday, midnight, beforeMidnight));
            }
            else if (marketType == MarketTypes.Forex)
            {
                _alwaysOpen = false;
                if (openTime.TimeOfDay == midnight)
                {
                    _openDays.Add(new DayTime(DayOfWeek.Monday, midnight, beforeMidnight));
                    _openDays.Add(new DayTime(DayOfWeek.Tuesday, midnight, beforeMidnight));
                    _openDays.Add(new DayTime(DayOfWeek.Wednesday, midnight, beforeMidnight));
                    _openDays.Add(new DayTime(DayOfWeek.Thursday, midnight, beforeMidnight));
                    _openDays.Add(new DayTime(DayOfWeek.Friday, midnight, beforeMidnight));
                }
                else if (openTime.TimeOfDay < beforeMidnight)
                {
                    _openDays.Add(new DayTime(DayOfWeek.Sunday, openTime.TimeOfDay, beforeMidnight));
                    _openDays.Add(new DayTime(DayOfWeek.Monday, midnight, beforeMidnight));
                    _openDays.Add(new DayTime(DayOfWeek.Tuesday, midnight, beforeMidnight));
                    _openDays.Add(new DayTime(DayOfWeek.Wednesday, midnight, beforeMidnight));
                    _openDays.Add(new DayTime(DayOfWeek.Thursday, midnight, beforeMidnight));
                    _openDays.Add(new DayTime(DayOfWeek.Friday, midnight, openTime.TimeOfDay));
                }

            }
            else if (marketType == MarketTypes.Stocks)
            {
                _alwaysOpen = false;
                _openDays.Add(new DayTime(DayOfWeek.Monday, openTime.TimeOfDay, beforeMidnight));
                _openDays.Add(new DayTime(DayOfWeek.Tuesday, openTime.TimeOfDay, beforeMidnight));
                _openDays.Add(new DayTime(DayOfWeek.Wednesday, openTime.TimeOfDay, beforeMidnight));
                _openDays.Add(new DayTime(DayOfWeek.Thursday, openTime.TimeOfDay, beforeMidnight));
                _openDays.Add(new DayTime(DayOfWeek.Friday, openTime.TimeOfDay, beforeMidnight));
            }
        }

        public bool IsOpen(DateTime date)
        {
            try
            {
                if (_alwaysOpen)
                {
                    return true;
                }
                foreach (var dayTime in _openDays)
                {
                    if (dayTime._day.Equals(date.DayOfWeek) && date.TimeOfDay > dayTime._openTime && date.TimeOfDay < dayTime._closeTime)
                    {
                        return true;
                    }
                }


                return false;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return false;
        }

        public bool IsAlwaysOpen() 
        {
            try
            {
                return _alwaysOpen;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return false;
        }

        public DateTime NextValidDate(DateTime date)
        {
            try
            {
                if (IsOpen(date))
                {

                }
                if (IsWeekendDay(date))
                {
                    date = date.AddDays(1);
                }
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }

            return date;
        }
        public static bool IsWeekendDay(DateTime day)
        {
            try
            {
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }

            return false;
        }

        //public static void AddWeekDays(ref List<DayTime> days) 
        //{
        //    try
        //    {
        //        days.Add(DayOfWeek.Monday);
        //        days.Add(DayOfWeek.Tuesday);
        //        days.Add(DayOfWeek.Wednesday);
        //        days.Add(DayOfWeek.Thursday);
        //        days.Add(DayOfWeek.Friday);

        //    }
        //    catch (Exception e)
        //    {
        //        BrokerLib.DebugMessage(e);
        //    }
        //}

        //public static void AddWeekEndDays(ref List<DayTime> days)
        //{
        //    try
        //    {
        //        days.Add(new DayTime(DayOfWeek.Saturday, _openTime, _closeTime));
        //        days.Add(DayOfWeek.Sunday);
        //    }
        //    catch (Exception e)
        //    {
        //        BrokerLib.DebugMessage(e);
        //    }
        //}


    }
}

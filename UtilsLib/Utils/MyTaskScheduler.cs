using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UtilsLib.Utils
{
    public class MyTaskScheduler
    {
        private static MyTaskScheduler _instance;
        private Dictionary<string, Timer> timers = new Dictionary<string, Timer>();

        private MyTaskScheduler() { }

        public static MyTaskScheduler Instance => _instance ?? (_instance = new MyTaskScheduler());

        public void ScheduleTask(Action task, string taskId, float intervalInHour)
        {
            try
            {
                var timer = new Timer(x =>
                {
                    task.Invoke();
                }, null, TimeSpan.FromHours(intervalInHour), TimeSpan.FromHours(intervalInHour));

                if (timers.ContainsKey(taskId))
                {
                    timers[taskId].Dispose();
                    timers[taskId] = timer;
                }
                else
                {
                    timers.Add(taskId, timer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ScheduleTaskByMinute(Action task, string taskId, int min)
        {
            try
            {
                var timer = new Timer(x =>
                {
                    task.Invoke();
                }, null, TimeSpan.Zero, TimeSpan.FromMinutes(min));

                if (timers.ContainsKey(taskId))
                {
                    timers[taskId].Dispose();
                    timers[taskId] = timer;
                }
                else
                {
                    timers.Add(taskId, timer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ScheduleTaskByInitOfMinute<T>(Action<T> task, string taskId, T data, int min, int second, int minuteOffset = 0)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                int startMinute = now.Minute + (min - now.Minute % min) + minuteOffset;
                DateTime firstRun = new DateTime(now.Year, now.Month, now.Hour == 23 && startMinute > 59 ? now.Day + 1 : now.Day, startMinute > 59 ? (now.Hour + 1) % 24 : now.Hour, startMinute % 60, second);

                if (now > firstRun)
                {
                    firstRun = firstRun.AddMinutes(min);
                }
                TimeSpan timeToGo = firstRun - now;
                var timer = new Timer(x =>
                {
                    task.Invoke(data);
                }, data, timeToGo, TimeSpan.FromMinutes(min));

                if (timers.ContainsKey(taskId))
                {
                    timers[taskId].Dispose();
                    timers[taskId] = timer;
                }
                else
                {
                    timers.Add(taskId, timer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ScheduleTaskOnlyOnce(Action task, int min, int second)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                DateTime firstRun = now.AddMinutes(min).AddSeconds(second);//new DateTime(now.Year, now.Month, now.Day, (now.Hour + (int) intervalInHour)%24, now.Minute, second);

                if (now > firstRun)
                {
                    firstRun = firstRun.AddMinutes(1);
                }

                TimeSpan timeToGo = firstRun - now;
                var timer = new Timer(x =>
                {
                    task.Invoke();
                    
                }, null, timeToGo, TimeSpan.Zero);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ScheduleTaskByInitOfHour(Action task, string taskId, float intervalInHour, int second)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                DateTime firstRun = now.AddHours(intervalInHour);//new DateTime(now.Year, now.Month, now.Day, (now.Hour + (int) intervalInHour)%24, now.Minute, second);

                if (now > firstRun)
                {
                    firstRun = firstRun.AddHours(intervalInHour);
                }
                TimeSpan timeToGo = firstRun - now;

                var timer = new Timer(x =>
                {
                    task.Invoke();
                }, null, TimeSpan.FromHours(intervalInHour), TimeSpan.FromHours(intervalInHour));

                if (timers.ContainsKey(taskId))
                {
                    timers[taskId].Dispose();
                    timers[taskId] = timer;
                }
                else
                {
                    timers.Add(taskId, timer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ScheduleTaskDaily(Action task, string taskId, int hour = 0, int minute = 0, int second = 0)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, hour, minute, second);

                if (now > firstRun)
                {
                    firstRun = firstRun.AddDays(1);
                }
                TimeSpan timeToGo = firstRun - now;

                var timer = new Timer(x =>
                {
                    task.Invoke();
                }, null, timeToGo, TimeSpan.FromHours(24));

                if (timers.ContainsKey(taskId))
                {
                    timers[taskId].Dispose();
                    timers[taskId] = timer;
                }
                else
                {
                    timers.Add(taskId, timer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ScheduleTaskByMilisecond(Action task, string taskId, int milisecond)
        {
            try
            {
                var timer = new Timer(x =>
                {
                    task.Invoke();
                }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(milisecond));

                if (timers.ContainsKey(taskId))
                {
                    timers[taskId].Dispose();
                    timers[taskId] = timer;
                }
                else
                {
                    timers.Add(taskId, timer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ScheduleTaskInDueTimeOnlyOnce<T>(Action<T> task, T data, string taskId, TimeSpan dueTime)
        {
            try
            {
                var timer = new Timer(x =>
                {
                    task.Invoke(data);
                }, null, dueTime, TimeSpan.Zero);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ScheduleTaskPeriodically<T>(Action<T> task, T data, string taskId, TimeSpan period)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

                if (now > firstRun)
                {
                    firstRun = now;
                }
                TimeSpan timeToGo = firstRun - now;

                var timer = new Timer(x =>
                {
                    task.Invoke(data);
                }, null, timeToGo, period);

                if (timers.ContainsKey(taskId))
                {
                    timers[taskId].Dispose();
                    timers[taskId] = timer;
                }
                else
                {
                    timers.Add(taskId, timer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DisposeTask(string taskId) 
        {
            try
            {
                timers.TryGetValue(taskId, out Timer timer);
                timer.Dispose();
                timers.Remove(taskId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

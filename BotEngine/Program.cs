using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BotEngine
{
    class Program
    {
        public static void Main(string[] args)
        {
            BotEngine genBot = new BotEngine();
            //Process.GetCurrentProcess().WorkingSet64 / 1024
        }

        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        static void MinimizeFootprint()
        {
            EmptyWorkingSet(Process.GetCurrentProcess().Handle);
        }
    }
}

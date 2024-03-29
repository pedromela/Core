﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UtilsLib.Utils
{
    public class ProcessManager
    {
        public ProcessManager() 
        {

        }

        public static string StartBotEngine()
        {
            try
            {

                if (IsProcessOpen("BotEngine"))
                {
                    return "AlreadyRunning";
                }
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.StartInfo.FileName = @"C:\Users\vm1pm\Documents\IA\Bot\Core\BotEngine\bin\Debug\net5.0\BotEngine.exe";
                    myProcess.StartInfo.CreateNoWindow = false;
                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    myProcess.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return "OK";
        }

        private static bool IsProcessOpen(string name)
        {
            //here we're going to get a list of all running processes on
            //the computer
            foreach (Process clsProcess in Process.GetProcesses())
            {
                //now we're going to see if any of the running processes
                //match the currently running processes. Be sure to not
                //add the .exe to the name you provide, i.e: NOTEPAD,
                //not NOTEPAD.EXE or false is always returned even if
                //notepad is running.
                //Remember, if you have the process running more than once, 
                //say IE open 4 times the loop thr way it is now will close all 4,
                //if you want it to just close the first one it finds
                //then add a return; after the Kill
                if (clsProcess.ProcessName.Contains(name))
                {
                    //if the process is found to be running then we
                    //return a true
                    return true;
                }
            }
            //otherwise we return a false
            return false;
        }

    }
}

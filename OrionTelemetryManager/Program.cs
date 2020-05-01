using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace OrionTelemetryManager
{
    class Program
    {
        private static bool isRunning = false;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            //https://miromannino.com/blog/hide-console-window-in-c/
            IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
            ShowWindow(h, 0);

            isRunning = true;

            Console.WriteLine("Hello! I am the Orion Telemetry Tool~\nI run in the background as you play our game to make sure everything is working in tip top shape.\n");


            if (Process.GetProcessesByName("Orion").Length > 0)
            {
                Console.WriteLine("Orion is Running!\n");
            }
            else
            {
                Environment.Exit(0);
            }

            while (isRunning)
            {
                if (Process.GetProcessesByName("Orion").Length > 0)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    isRunning = false;
                }
            }

            //append save data
            string currentDir = Directory.GetCurrentDirectory();
            string OrionDataTempPath = currentDir.Replace("\\OrionTelemetryManager", "") + "\\OrionTelemetryTemp.Data";
            string OrionDataPath = currentDir.Replace("\\OrionTelemetryManager", "") + "\\OrionTelemetry.Data";

            if (File.Exists(OrionDataTempPath))
            {

                if (File.Exists(OrionDataPath) == false)
                {
                    Console.WriteLine("New OrionTelemetry.Data created");
                    File.Create(OrionDataPath).Close();
                }

                string previousData = File.ReadAllText(OrionDataPath);
                string tempData = File.ReadAllText(OrionDataTempPath);

                string newData = "";

                if (previousData.Length == 0)
                {
                    newData = tempData;
                }
                else
                {
                    //format to json
                    //this is super hard coded
                    previousData = previousData.Remove(previousData.LastIndexOf("]"), 1);
                    Console.WriteLine(previousData);

                    previousData = previousData.Insert(previousData.LastIndexOf("}") + 1, ",");
                    //Console.WriteLine(previousData);

                    tempData = tempData.Remove(0, 1);
                    //Console.WriteLine(tempData);

                    newData = previousData + tempData;

                }

                File.WriteAllText(OrionDataPath, newData);
                File.Delete(OrionDataTempPath);
            }
            else
            {
                Console.WriteLine("Orion temp file not found");
                Environment.Exit(0);
            }

            Console.WriteLine("Orion closed");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.Windows;

namespace Osmo.Core.Logging
{
    class SystemInformation
    {
        private static Random rnd = new Random();

        public static void CreateCrashLog(Exception ex, MainWindow window)
        {

            string fileName = string.Format("{0}_osmo_crashlog.txt", Helper.GetDateAndTime('_', '-'));

            string path = string.Format("{0}Logs\\{1}", AppDomain.CurrentDomain.BaseDirectory,
                fileName);
            string content = string.Format("Osmo has stopped working! The following exception occurred (Line {0}):\n{1}\n\nSystem specifications:\n{2}",
                 Logger.GetExceptionLine(ex), ex, SystemData);

            File.WriteAllText(path, content);
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
                window.ShowInTaskbar = false;
                Logger.Instance.WriteLog("Osmo has stopped working! See crashlog \"{0}\" for more details.", LogType.FATAL, fileName);
            }

            MessageBox.Show(string.Format("{0}{1}{2}", Helper.FindString("crash_description1"), 
                path, Helper.FindString("crash_description2")), GetRandomCrashTitle());
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer",
                Arguments = string.Format("/e, /select, \"{0}\"", path)
            });
            Environment.Exit(0);
        }
        
        private static string GetRandomCrashTitle()
        {
            switch (rnd.Next(0, 7))
            {
                case 1:
                    return Helper.FindString("crash_title2");
                case 2:
                    return Helper.FindString("crash_title3");
                case 3:
                    return Helper.FindString("crash_title4");
                case 4:
                    return Helper.FindString("crash_title5");
                case 5:
                    return Helper.FindString("crash_title6");
                case 6:
                    return Helper.FindString("crash_title7");
                default:
                    return Helper.FindString("crash_title1");
            }
        }

        private static string SystemData
        {
            get
            {
                string data = string.Format("Osmo version: {0}\n\n{1}", OsmoVersion, FrameworkLookup.GetFrameworks);
                try
                {
                    string OS_Name = "";
                    string Version = "";

                    ManagementObjectSearcher OS = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                    ManagementObjectCollection queryCollection1 = OS.Get();
                    foreach (ManagementObject mo in queryCollection1)
                    {
                        OS_Name = mo["name"].ToString();
                        Version = mo["version"].ToString();
                    }

                    data += string.Format("OS: {0}, Version: {1}\nCPU: {2}\nGPU: {3}\nMemory: {4}\nMemory usage at crash: {5} MB",
                        OSName,
                        OSVersion,
                        Processor,
                        VideoController,
                        Memory,
                        (GC.GetTotalMemory(false) / 128) / 1024);
                }
                catch
                {
                }
                return data;
            }
        }

        public static string OSName
        {
            get
            {
                try
                {
                    ManagementObjectSearcher OS = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                    ManagementObjectCollection queryCollection = OS.Get();
                    foreach (ManagementObject mo in queryCollection)
                        return mo["name"].ToString().Split('|')[0];
                }
                catch { }
                return "Unknown!";
            }
        }

        public static string OSVersion
        {
            get
            {
                try
                {
                    ManagementObjectSearcher OS = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                    ManagementObjectCollection queryCollection = OS.Get();
                    foreach (ManagementObject mo in queryCollection)
                        return mo["version"].ToString();
                }
                catch { }
                return "Unknown!";
            }
        }

        public static string OsmoVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public static string Processor
        {
            get
            {
                try
                {
                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_Processor");

                    foreach (ManagementObject queryObj in searcher.Get())
                        return queryObj["Name"].ToString();
                }
                catch
                {
                }
                return "Unknown!";
            }
        }

        public static string VideoController
        {
            get
            {
                try
                {
                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_VideoController");

                    foreach (ManagementObject queryObj in searcher.Get())
                        return queryObj["Name"].ToString();
                }
                catch
                {
                }
                return "Unknown!";
            }
        }

        public static string MemoryCapacity
        {
            get
            {
                try
                {
                    float totalCapacity = 0;
                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_PhysicalMemory");

                    foreach (ManagementObject queryObj in searcher.Get())
                        totalCapacity += float.Parse(queryObj["Capacity"].ToString());

                    totalCapacity = ((totalCapacity / 1024) / 1024);
                    return totalCapacity.ToString();
                }
                catch { }
                return "Unknown!";
            }
        }

        static string Memory
        {
            get
            {
                try
                {
                    List<string> memory = new List<string>();
                    string stringBuilder = "";
                    float totalCapacity = 0;
                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_PhysicalMemory");

                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        totalCapacity += float.Parse(queryObj["Capacity"].ToString());
                        memory.Add("Capacity: " + (float.Parse(queryObj["Capacity"].ToString()) / 1024) / 1024 + " MB, Speed: " + queryObj["Speed"] + " MHz; ");
                    }

                    foreach (string s in memory)
                        stringBuilder += "\n\t" + s;

                    totalCapacity = ((totalCapacity / 1024) / 1024);
                    return memory.Count + "x, Total Capacity: " + totalCapacity + "; MB: " + stringBuilder;
                }
                catch
                {
                }
                return "Unknown!";
            }
        }
    }
}

using System;
using System.IO;
using System.Text;

namespace Installer.Objects
{
    class Logger
    {
        static readonly string _logFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Vibrance Player\\Logs\\";
        static readonly string _logPath = _logFolderPath + "log_installer.txt";

        public void WriteLog(string msg)
        {
            WriteLog(msg, LoggingType.INFO);
        }

        public void WriteLog(string msg, LoggingType mode)
        {
            if (!Directory.Exists(_logFolderPath))
                Directory.CreateDirectory(_logFolderPath);

            if (!File.Exists(_logPath))
            {
                string logStart = "---Installer log from [DATE], [TIME]---\r";
                logStart = logStart.Replace("[DATE]", GetDate());
                logStart = logStart.Replace("[TIME]", GetTime());
                File.WriteAllText(_logPath, logStart);
            }

            StringBuilder logBuilder = new StringBuilder();

            switch (mode)
            {
                case LoggingType.INFO:
                    logBuilder.Append("[INFO] ");
                    break;

                case LoggingType.WARNING:
                    logBuilder.Append("[WARNING] ");
                    break;
            }
        }

        public void WriteLog(string msg, LoggingType mode, Exception ex)
        {
            WriteLog(msg, mode, ex, false);
            if (ex.InnerException != null)
                WriteLog(msg, mode, ex.InnerException, true);
        }

        private void WriteLog(string msg, LoggingType mode, Exception ex, bool isInner)
        {
            string stackTrace = ex.StackTrace.Remove(0, ex.StackTrace.LastIndexOf(' ') + 1);
            stackTrace = stackTrace.Replace(".", "");
            if (msg == "")
            {
                WriteLog(string.Format("[{0}] {1} \"{2}\": {3} (Class \"{4}\", Line {5})",
                    ex.HResult, GetExceptionText(isInner), ex.TargetSite, ex.Message, ex.Source, stackTrace), mode);
            }
            else
            {
                WriteLog(string.Format("[{0}] {1} - {2} \"{3}\": {4} (Class \"{5}\", Line {6})",
                    ex.HResult, msg, GetExceptionText(isInner), ex.TargetSite, ex.Message, ex.Source, stackTrace), mode);
            }
        }

        private string GetExceptionText(bool isInner)
        {
            if (!isInner)
                return "Exception thrown in method";
            else
                return "Inner exception";
        }

        private string GetDate(char splitter = '.')
        {
            string d = DateTime.Now.Day.ToString(), m = DateTime.Now.Month.ToString();
            if (d.Length == 1)
                d = "0" + d;

            if (m.Length == 1)
                m = "0" + m;

            return d + splitter + m + splitter + DateTime.Now.Year;
        }

        private string GetTime(char splitter = ':')
        {
            string h = DateTime.Now.Hour.ToString(), m = DateTime.Now.Minute.ToString(), s = DateTime.Now.Second.ToString();
            if (h.Length == 1)
                h = "0" + h;

            if (m.Length == 1)
                m = "0" + m;
            if (s.Length == 1)
                s = "0" + s;

            return h + splitter + m + splitter + s;
        }
    }

    public enum LoggingType
    {
        INFO,
        WARNING
    }
}

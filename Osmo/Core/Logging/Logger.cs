using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Logging
{
    class Logger
    {
        private static Logger instance;
        private static readonly string _logFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\";
        private string _fileName = "log.txt";

        private string _logFilePath;
        private int _sessionId = -1;
        private Random rnd = new Random();

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logger();
                }

                return instance;
            }
        }

        public int SessionID { get => _sessionId; }

        private Logger()
        {
            _sessionId = rnd.Next(10000000, 99999999);
            _logFilePath = Path.Combine(_logFolderPath, _fileName);
        }

        /// <summary>
        /// Writes a formatted message into the log with <see cref="LogType.INFO"/> tag
        /// </summary>
        /// <param name="msg">The message to print (with formatters)</param>
        /// <param name="param">Optional: All parameters for the formatted string</param>
        public void WriteLog(string msg, params object[] param)
        {
            WriteLog(msg, LogType.INFO, param);
        }

        /// <summary>
        /// Writes a formatted exception message into the log with <see cref="LogType.WARNING"/>
        /// </summary>
        /// <param name="msg">The message to print (with formatters)</param>
        /// <param name="ex">The exception to write into the log</param>
        /// <param name="param">Optional: All parameters for the formatted string</param>
        public void WriteLog(string msg, Exception ex, params object[] param)
        {
            WriteLog(msg, LogType.WARNING, ex, param);
        }

        /// <summary>
        /// Writes a message into the log with <see cref="LogType.INFO"/>
        /// </summary>
        /// <param name="msg">The message to print</param>
        public void WriteLog(string msg)
        {
            WriteLog(msg, LogType.INFO);
        }

        /// <summary>
        /// Writes an exception message into the log with <see cref="LogType.WARNING"/>
        /// </summary>
        /// <param name="msg">The message to print</param>
        /// <param name="ex">The exception to write into the log</param>
        public void WriteLog(string msg, Exception ex)
        {
            WriteLog(msg, LogType.WARNING, ex);
        }

        /// <summary>
        /// Writes a formatted message into the log
        /// </summary>
        /// <param name="msg">The message to print (with formatters)</param>
        /// <param name="mode">The <see cref="LogType"/> tag</param>
        /// <param name="param">Optional: All parameters for the formatted string</param>
        public void WriteLog(string msg, LogType mode, params object[] param)
        {
            WriteLog(string.Format(msg, param), mode);
        }

        /// <summary>
        /// Writes a formatted exception message into the log
        /// </summary>
        /// <param name="msg">The message to print (with formatters)</param>
        /// <param name="mode">The <see cref="LogType"/> tag</param>
        /// <param name="ex">The exception to write into the log</param>
        /// <param name="param">All parameters for the formatted string</param>
        public void WriteLog(string msg, LogType mode, Exception ex, params object[] param)
        {
            WriteLog(string.Format(msg, param), mode, ex);
        }


        /// <summary>
        /// Writes an exception message into the log
        /// </summary>
        /// <param name="msg">The message to print</param>
        /// <param name="mode">The <see cref="LogType"/> tag</param>
        /// <param name="ex">The exception to write into the log</param>
        public void WriteLog(string msg, LogType mode, Exception ex)
        {
            WriteLog(msg, mode, ex, false);
            if (ex.InnerException != null)
                WriteLog(msg, mode, ex.InnerException, true);
        }

        private void WriteLog(string msg, LogType mode, Exception ex, bool isInner)
        {
            string stackTrace = GetExceptionLine(ex);

            if (msg == string.Empty)
            {
                if (stackTrace != "-1")
                    WriteLog(string.Format("[{0}] {1} \"{2}\": {3} (Class \"{4}\", Line {5})",
                        ex.GetType().Name, GetExceptionText(isInner), ex.TargetSite, ex.Message, ex.Source, stackTrace), mode);
                else
                    WriteLog(string.Format("[{0}] {1} \"{2}\": {3} (Class \"{4}\")",
                        ex.GetType().Name, GetExceptionText(isInner), ex.TargetSite, ex.Message, ex.Source), mode);
            }
            else
            {
                if (stackTrace != "-1")
                    WriteLog(string.Format("[{0}] {1} - {2} \"{3}\": {4} (Class \"{5}\", Line {6})",
                        ex.GetType().Name, msg, GetExceptionText(isInner), ex.TargetSite, ex.Message, ex.Source, stackTrace), mode);
                else
                    WriteLog(string.Format("[{0}] {1} - {2} \"{3}\": {4} (Class \"{5}\")",
                        ex.GetType().Name, msg, GetExceptionText(isInner), ex.TargetSite, ex.Message, ex.Source), mode);
            }
        }

        /// <summary>
        /// Writes a message into the log. This is the core method!
        /// </summary>
        /// <param name="msg">The message to print</param>
        /// <param name="mode">The <see cref="LogType"/> tag</param>
        public void WriteLog(string msg, LogType mode)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Directory.CreateDirectory(_logFolderPath);

                    if (!File.Exists(_logFilePath) && _fileName != null)
                    {
                        string logStart = Properties.Resources.LogStart;
                        logStart = logStart.Replace("[DATE]", Helper.GetDate());
                        logStart = logStart.Replace("[TIME]", Helper.GetTime());
                        logStart = logStart.Replace("[NAME]", _fileName);
                        File.WriteAllText(_logFilePath, logStart);
                    }

                    StringBuilder logBuilder = new StringBuilder();

                    switch (mode)
                    {
                        case LogType.INFO:
                            logBuilder.Append("[INFO] ");
                            break;

                        case LogType.WARNING:
                            logBuilder.Append("[WARNING] ");
                            break;

                        case LogType.ERROR:
                            logBuilder.Append("[ERROR] ");
                            break;

                        case LogType.FATAL:
                            logBuilder.Append("------------------------------   FATAL APPLICATION ERROR   ------------------------------\r");
                            break;

                        case LogType.SYSTEMINF:
                            logBuilder.Append("----------------------------------     S Y S T E M     ----------------------------------\r");
                            break;

                        case LogType.DEBUG:
                            logBuilder.Append("[DEBUG] ");
                            break;

                        case LogType.CONSOLE:
                            logBuilder.Append("[CONSOLE] ");
                            break;

                        case LogType.VERBOSE:
                            logBuilder.Append("[VERBOSE] ");
                            break;
                    }
                    logBuilder.Append(string.Format("({0}) ", _sessionId));

                    if (mode != LogType.SYSTEMINF || mode != LogType.FATAL)
                    {
                        logBuilder.Append(Helper.GetDateAndTime()).Append(": ").Append(msg).Append("\r");
                    }
                    else
                        logBuilder.Append(msg).Append("\r\r\r");

                    if (_fileName != null)
                    {
                        if (mode != LogType.VERBOSE)
                            File.AppendAllText(_logFilePath, logBuilder.ToString());
                        else
                            File.AppendAllText(_logFilePath + ".verbose", logBuilder.ToString());
                    }

                    if (_fileName != null)
                        Console.WriteLine(string.Format("({0}) {1}", _fileName, logBuilder.ToString().Replace("\r", "")));
                    else
                        Console.WriteLine(string.Format("{0}", logBuilder.ToString().Replace("\r", "")));
                    break;
                }
                catch
                {

                }
            }
        }

        public static string GetExceptionLine(Exception ex)
        {
            if (ex.StackTrace != null)
            {
                string stackTrace = ex.StackTrace;
                stackTrace = ex.StackTrace.Remove(0, ex.StackTrace.LastIndexOf(' ') + 1);
                return stackTrace.Replace(".", "");
            }
            else
                return "-1";
        }

        private string GetExceptionText(bool isInner)
        {
            return !isInner ? "Exception thrown in method" : "Inner exception";
        }
    }
}

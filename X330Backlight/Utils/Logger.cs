using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace X330Backlight.Utils
{
    public class Logger
    {
        private const int MaxLogFiles = 10;
        private static readonly Stream LogStream;
        private static readonly StreamWriter Writer;
        private static readonly object WriteLocker = new object();
        private static bool _closed;

        /// <summary>
        /// Gets the current log file path.
        /// </summary>
        public static string CurrentLogFile { get; }

        static Logger()
        {
            var exeName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.ModuleName);
            var now = DateTime.Now;
            var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            var existingLogFiles = Directory.GetFiles(logFolder).ToList();
            existingLogFiles.Sort();
            if (existingLogFiles.Count > MaxLogFiles)
            {
                for (int i = 0; i < existingLogFiles.Count - MaxLogFiles; i++)
                {
                    File.Delete(existingLogFiles[i]);
                }
            }
            CurrentLogFile = Path.Combine(logFolder, $"{exeName}_{now:yyyyMMddHHmmss}{now.Millisecond:d3}.log");
            LogStream = new FileStream(CurrentLogFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            Writer = new StreamWriter(LogStream);
        }

        /// <summary>
        /// Write one log into the log file.
        /// </summary>
        /// <param name="log"></param>
        public static void Write(string log)
        {
            lock (WriteLocker)
            {
                if (!_closed)
                {
                    try
                    {
                        Writer.WriteLine($"[{DateTime.Now}] - " + log);
                        Writer.Flush();
                    }
                    catch
                    {
                        //Do nothing.
                    }
                }
            }
        }

        /// <summary>
        /// Close the logger, if this method was called, no log can be written.
        /// </summary>
        public static void Close()
        {
            lock (WriteLocker)
            {
                Writer.Dispose();
                LogStream.Dispose();
                _closed = true;
            }
        }
    }
}

using System.Diagnostics;

namespace LoggerUtil.Console
{
    public class ConsoleWritter : IConsoleWriter
    {
        private log4net.Core.Level _loggerLevel;
        
        public ConsoleWritter(log4net.Core.Level level)
        {
            this._loggerLevel = level;
        }

        public void ConsoleWrite(string message, string level)
        {
            if(ShouldLog(level))
            {
                System.Console.WriteLine("[{0}] {1}", level, message);
                Debug.WriteLine("[{0}] {1}", level, message);
            }
        }

        private bool ShouldLog(string level)
        {
            if (_loggerLevel == log4net.Core.Level.All ||
                _loggerLevel == log4net.Core.Level.Finer)
                return true;
            if (_loggerLevel == log4net.Core.Level.Debug)
                return level.Equals("DEBUG") || level.Equals("INFO") || level.Equals("WARN") || level.Equals("ERROR") ||
                       level.Equals("FATAL");
            if (_loggerLevel == log4net.Core.Level.Error)
                return level.Equals("ERROR") || level.Equals("FATAL");
            if (_loggerLevel == log4net.Core.Level.Fatal)
                return level.Equals("FATAL");
            if (_loggerLevel == log4net.Core.Level.Info)
                return level.Equals("INFO") || level.Equals("WARN") || level.Equals("ERROR") || level.Equals("FATAL");
            if (_loggerLevel == log4net.Core.Level.Warn)
                return level.Equals("WARN") || level.Equals("ERROR") || level.Equals("FATAL");
            return true;
        }
    }

    public class NullConsoleWriter : IConsoleWriter
    {
        public void ConsoleWrite(string message, string level)
        {
            
        }
    }
}

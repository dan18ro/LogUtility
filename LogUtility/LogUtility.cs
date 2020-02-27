using System.Linq;

using log4net;

using LogUtility.Configuration;
using LogUtility.Console;

namespace LogUtility
{
    /// <summary>
    /// Provides the implementation of a synchronous logging.
    /// </summary>
    public class LogUtility: ILogUtility
    {
        private readonly ILogConfiguration _configuration;
        protected readonly IConsoleWriter ConsoleWriter;
        protected readonly SystemInformation LoggerStartupInformation;
        protected ILog Logger;
        private log4net.Layout.ILayout _currentLayout;


        #region C'tor

        /// <summary>
        /// Instantiates a synchronous logging object.
        /// </summary>
        /// <param name="logConfiguration">Log configuration object should be provided.</param>
        /// <param name="consoleAvailable">If console is available, and we want to log on console as well it should be set to true.</param>
        public LogUtility(ILogConfiguration logConfiguration, bool consoleAvailable = false)           
        {
            _configuration = logConfiguration;
            ConsoleWriter = consoleAvailable ? (IConsoleWriter)(new ConsoleWriter(_configuration.LoggerLevel)): new NullConsoleWriter();
            CreateLogger();
            LoggerStartupInformation = new SystemInformation(this);
        }

        
        #endregion

        #region ILogUtilityity

        /// <inheritdoc />
        public void LogStartUpInformation()
        {
            BeginStartupInformationLog();
            LoggerStartupInformation.Line();
            Info(string.Empty);
            LoggerStartupInformation.ExecutionAssemblyInformation();
            LoggerStartupInformation.UserMachineDateTime();
            LoggerStartupInformation.WindowsFramework();
            LoggerStartupInformation.TimeZone();
            LoggerStartupInformation.CurrentCulture();
            LoggerStartupInformation.CommandLine();
            LoggerStartupInformation.FileLocations(_configuration.FilePath);
            LoggerStartupInformation.BuildInfo();
            LoggerStartupInformation.Assemblies();
            LoggerStartupInformation.EnvironmentVariables();
            LoggerStartupInformation.ConfigFile(_configuration.FilePath);
            LoggerStartupInformation.Line();
            EndStartupInformationLog();
        }
        
        /// <inheritdoc />
        public virtual void Error(string message, params object[] args)
        {
            var msg = GetMessage(message, args);
            Logger.Logger.Log(typeof(LogUtility), log4net.Core.Level.Error, msg, null);
            ConsoleWriter.ConsoleWrite(msg, "ERROR");
        }

        /// <inheritdoc />
        public virtual void Info(string message, params object[] args)
        {
            var msg = GetMessage(message, args);
            Logger.Logger.Log(typeof(LogUtility), log4net.Core.Level.Info, msg, null);
            ConsoleWriter.ConsoleWrite(msg, "INFO");
        }

        /// <inheritdoc />
        public virtual void Debug(string message, params object[] args)
        {
            var msg = GetMessage(message, args);
            Logger.Logger.Log(typeof(LogUtility), log4net.Core.Level.Debug, msg, null);
            ConsoleWriter.ConsoleWrite(msg, "DEBUG");
        }

        /// <inheritdoc />
        public virtual void Fatal(string message, params object[] args)
        {
            var msg = GetMessage(message, args);
            Logger.Logger.Log(typeof(LogUtility), log4net.Core.Level.Fatal, msg, null);
            ConsoleWriter.ConsoleWrite(msg, "FATAL");
        }

        /// <inheritdoc />
        public virtual void Warning(string message, params object[] args)
        {
            var msg = GetMessage(message, args);
            Logger.Logger.Log(typeof(LogUtility), log4net.Core.Level.Warn, msg, null);
            ConsoleWriter.ConsoleWrite(msg, "WARN");
        }

        #endregion
        
        protected void BeginStartupInformationLog()
        {
            _currentLayout = LogManager.GetRepository().GetAppenders().OfType<log4net.Appender.RollingFileAppender>().First().Layout;

            var appenders = LogManager.GetRepository().GetAppenders();
            foreach (var rollingFileAppender in appenders.OfType<log4net.Appender.RollingFileAppender>())
            {
                var layout = new log4net.Layout.PatternLayout("%message%newline");
                rollingFileAppender.Layout = layout;
                layout.ActivateOptions();
            }
        }

        protected void EndStartupInformationLog()
        {
            var appenders = LogManager.GetRepository().GetAppenders();
            foreach (var rollingFileAppender in appenders.OfType<log4net.Appender.RollingFileAppender>())
            {
                rollingFileAppender.Layout = _currentLayout;
            }
            _currentLayout = null;
        }

        private string GetMessage(string message, object[] args)
        {
            return args != null ? string.Format(message, args) : string.Format("{0}", message);
        }

       
        private void CreateLogger()
        {
            Logger = LogManager.GetLogger(_configuration.LogName);
        }

        public virtual void StopLogging()
        {
        }
    }
}

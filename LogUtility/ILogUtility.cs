namespace LogUtility
{
    public interface ILogUtility
    {
        /// <summary>
        /// Logs information about the system, and the configuration file.
        /// </summary>
        void LogStartUpInformation();

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Error(string message, params object[] args);

        /// <summary>
        /// Logs an Information message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Info(string message, params object[] args);

        /// <summary>
        /// Logs a Debug message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// Logs a Fatal message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Fatal(string message, params object[] args);

        /// <summary>
        /// Logs an Warning message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Warning(string message, params object[] args);

        /// <summary>
        /// Should be called in order to terminate logging.
        /// </summary>
        void StopLogging();
    }
}

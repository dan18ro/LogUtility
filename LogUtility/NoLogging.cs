using System;

namespace LogUtility
{
    /// <summary>
    /// This is an empty implementation. 
    /// Singleton to be used if we want to have control on when to log in a specific component.
    /// </summary>
    public class NoLogging : ILogUtility
    {
        private static readonly Lazy<NoLogging> _instance = new Lazy<NoLogging>(() => new NoLogging());

        public static NoLogging Instance => _instance.Value;

        private NoLogging()
        {
        }

        /// <inheritdoc />
        public void LogStartUpInformation()
        {
        }

        /// <inheritdoc />
        public void Error(string message, params object[] args)
        {
        }

        /// <inheritdoc />
        public void Info(string message, params object[] args)
        {
        }

        /// <inheritdoc />
        public void Debug(string message, params object[] args)
        {
        }

        /// <inheritdoc />
        public void Fatal(string message, params object[] args)
        {
        }

        /// <inheritdoc />
        public void Warning(string message, params object[] args)
        {
        }

        /// <inheritdoc />
        public void StopLogging()
        {
        }
    }
}

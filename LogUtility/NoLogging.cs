namespace LogUtility
{
    /// <summary>
    /// This is an empty implementation. 
    /// Singleton to be used if we want to have control on when to log in a specific component.
    /// </summary>
    public class NoLogging : ILogUtility
    {
        private static NoLogging instance;

        public static NoLogging Instance
        {
            get
            {
                return instance ?? (instance = new NoLogging());
            }
        }

        private NoLogging()
        {
        }

        public void LogStartUpInformation()
        {
        }

        public void BeginStartupInformationLog()
        {
        }

        public void EndStartupInformationLog()
        {
        }

        public void Error(string message, params object[] args)
        {
        }

        public void Info(string message, params object[] args)
        {
        }

        public void Debug(string message, params object[] args)
        {
        }

        public void Fatal(string message, params object[] args)
        {
        }

        public void Warning(string message, params object[] args)
        {
        }

        public void StopLogging()
        {
        }
    }
}

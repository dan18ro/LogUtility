using log4net.Core;

namespace LogUtility.Configuration
{
    /// <summary>
    /// Provides log configuration data.
    /// </summary>
    public interface ILogConfiguration
    {
        string LogName { get; }
        Level LoggerLevel { get; }
        string FilePath { get; }
    }
}

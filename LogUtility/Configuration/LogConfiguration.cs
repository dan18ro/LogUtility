using System.Diagnostics;
using System.Xml;

using log4net;
using log4net.Core;

namespace LogUtility.Configuration
{
    public class LogConfiguration : ILogConfiguration
    {
        private XmlNode _logXmlConfig;
        private XmlDocument _doc;
        private const string XmlServiceLogConfig = "configuration/LogConfiguration";

        public LogConfiguration(string configFilePath, string name)
        {
            LoggerLevel = Level.Error;
            LogName = name;
            FilePath = configFilePath;
            GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;
            LoadConfigFile();
            ReadConfigNode();
            ReadLoggerLevel(FindLoggerByName());
        }

        /// <summary>
        /// Gets the configuration file path.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the log name.
        /// </summary>
        public string LogName { get; }

        /// <summary>
        /// Gets the Logger level.
        /// </summary>
        public Level LoggerLevel { get; private set; }

        private void LoadConfigFile()
        {
            var doc = new XmlDocument();
            doc.Load(FilePath);

            var logNode = doc.SelectSingleNode(XmlServiceLogConfig);

            if (logNode != null)
            {
                _logXmlConfig = logNode.CloneNode(true);
            }
            else
            {
                throw new LogException("Couldn't find log configuration node");
            }
        }

        private XmlNode FindLoggerByName()
        {
            var loggers = _doc.SelectNodes("/log4net/logger");
            foreach (XmlNode selectSingleNode in loggers)
            {
                if (selectSingleNode.Attributes != null
                    && LogName == selectSingleNode.Attributes["name"].Value)
                {
                    return selectSingleNode;
                }
            }

            throw new LogException(
                string.Format("There is no logger configuration with the name: {0}", LogName));
        }

        private void ReadLoggerLevel(XmlNode loggerNode)
        {
            var selectSingleNode = loggerNode.SelectSingleNode("level");

            if (selectSingleNode?.Attributes == null)
            {
                return;
            }

            var level = selectSingleNode.Attributes["value"].Value;
            switch (level)
            {
                case "Info":
                    LoggerLevel = Level.Info;
                    break;
                case "All":
                    LoggerLevel = Level.All;
                    break;
                case "Error":
                    LoggerLevel = Level.Error;
                    break;
                case "Warn":
                    LoggerLevel = Level.Warn;
                    break;
                case "Fatal":
                    LoggerLevel = Level.Fatal;
                    break;
                case "Debug":
                    LoggerLevel = Level.Debug;
                    break;
                default:
                    LoggerLevel = Level.Info;
                    break;
            }
        }

        private void ReadConfigNode()
        {
            _doc = new XmlDocument();
            var elem = _doc.CreateElement("log4net");
            var rawNode = _doc.ImportNode(_logXmlConfig, true);
            while (rawNode.HasChildNodes)
                elem.AppendChild(rawNode.ChildNodes[0]);
            log4net.Config.XmlConfigurator.Configure(elem);
            _doc.AppendChild(elem);
        }
    }
}

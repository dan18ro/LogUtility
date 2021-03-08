using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

using log4net.Util;

using Microsoft.Win32;

namespace LogUtility
{
    /// <summary>
    /// Defines the means to read the system information.
    /// </summary>
    public class SystemInformation
    {
        private ILogUtility _logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemInformation"/> class.
        /// </summary>
        /// <param name="logInterface">Logger object instance.</param>
        public SystemInformation(ILogUtility logInterface)
        {
            _logger = logInterface;
        }

        /// <summary>
        /// Adds a separation line.
        /// </summary>
        public void Line()
        {
            _logger.Info("********************************************************************************");
        }

        /// <summary>
        /// Adds the execution assembly information.
        /// </summary>
        public void ExecutionAssemblyInformation()
        {
            AssemblyName name = ExecutionAssembly.GetName();
            _logger.Info("Starting '{0}' ({1})", SystemInfo.ApplicationFriendlyName, name.Version);
        }
        
        /// <summary>
        /// Adds the machine's user and date time information.
        /// </summary>
        public void UserMachineDateTime()
        {
            _logger.Info(
                "User '{0}\\{1}' on '{2}' at '{3:HH:mm:ss dd MMM yyyy}'",
                Environment.UserDomainName,
                Environment.UserName,
                SystemInfo.HostName,
                DateTime.Now);
        }

        /// <summary>
        /// Adds information about the machine OS and other framework information.
        /// </summary>
        public void WindowsFramework()
        {
            _logger.Info(
                "Machine runs '{0}' ({1}) on '{2}' ({3})",
                GetFrameworkName(),
                Environment.Version.ToString(),
                GetOSName(),
                Environment.OSVersion.Version.ToString());
        }

        /// <summary>
        /// Adds information about the timezone.
        /// </summary>
        public void TimeZone()
        {
            TimeZoneInfo local = TimeZoneInfo.Local;
            TimeSpan utcOffset = local.GetUtcOffset(DateTime.Now);
            _logger.Info(
                "Timezone is '{0}'. The base offset is UTC {1:+;-}{2:00}:{3:00}. The current offset is UTC {4:+;-}{5:00}:{6:00} ({7})",
                TimeZoneInfo.Local.StandardName,
                local.BaseUtcOffset.CompareTo(TimeSpan.Zero),
                local.BaseUtcOffset.Hours,
                local.BaseUtcOffset.Minutes,
                utcOffset.CompareTo(TimeSpan.Zero),
                utcOffset.Hours,
                utcOffset.Minutes,
                local.IsDaylightSavingTime(DateTime.Now) ? "daylight saving time" : "not daylight saving time");
        }

        /// <summary>
        /// Adds information about the Current culture.
        /// </summary>
        public void CurrentCulture()
        {
            _logger.Info("CurrentCulture is '{0}'", CultureInfo.CurrentCulture.ToString());
        }

        /// <summary>
        /// Adds information on the command line arguments.
        /// </summary>
        public void CommandLine()
        {
            _logger.Info("Arguments: {0}", Environment.CommandLine);
        }

        /// <summary>
        /// Adds information about config file.
        /// </summary>
        /// <param name="configFile"></param>
        public void FileLocations(string configFile)
        {
            Line();
            _logger.Info("Locations:");
            Line();
            var text = string.Empty;
            try
            {
                text = ExecutionAssembly.Location;
            }
            catch (NotSupportedException)
            {
            }

            _logger.Info("    Executable: '{0}'", text);
            var text2 = string.Empty;
            try
            {
                text2 = Directory.GetCurrentDirectory();
            }
            catch (NotSupportedException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }

            _logger.Info("    Working directory: '{0}'", text2);
            string text3 = string.Empty;
            try
            {
                text3 = Path.GetFullPath(configFile);
            }
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (PathTooLongException)
            {
            }

            _logger.Info("    Config file: '{0}'", text3);
        }

        /// <summary>
        /// Adds information on the build.
        /// </summary>
        public void BuildInfo()
        {
            Line();
            _logger.Info("Build Information:");
            Line();
            var customAttributes = ExecutionAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (customAttributes.Length <= 0) return;
            var description = ((AssemblyDescriptionAttribute)customAttributes.First()).Description;
            var array = description.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var t in array)
            {
                _logger.Info("    {0}", t);
            }
        }

        /// <summary>
        /// Adds the loaded assemblies.
        /// </summary>
        public void Assemblies()
        {
            Line();
            _logger.Info("Loaded assemblies:");
            Line();
            var source = new Assembly[0];
            try
            {
                source = AppDomain.CurrentDomain.GetAssemblies();
            }
            catch (AppDomainUnloadedException)
            {
            }

            foreach (var current in from Assembly assembly in source orderby assembly.GetName().Name select assembly)
            {
                var text = string.Empty;
                try
                {
                    text = current.Location;
                }
                catch (NotSupportedException)
                {
                }

                _logger.Info(
                    "    {0} ({1}, {2})",
                    current.GetName().Name,
                    current.GetName().Version.ToString(),
                    text);
            }
        }

        /// <summary>
        /// Adds the environment variables.
        /// </summary>
        public void EnvironmentVariables()
        {
            Line();
            _logger.Info("Process environment:");
            Line();
            IDictionary dictionary = new Hashtable();
            try
            {
                dictionary = Environment.GetEnvironmentVariables();
            }
            catch (SecurityException)
            {
            }
            catch (OutOfMemoryException)
            {
            }

            foreach (var current in from string key in dictionary.Keys orderby key select key)
            {
                _logger.Info("    {0} = {1}", current, dictionary[current]);
            }
        }

        /// <summary>
        /// Adds info on the configuration file.
        /// </summary>
        /// <param name="configFile"></param>
        public void ConfigFile(string configFile)
        {
            Line();
            _logger.Info("Configuration File:");
            Line();
            try
            {
                using (var streamReader = new StreamReader(configFile))
                {
                    string text;
                    while ((text = streamReader.ReadLine()) != null)
                    {
                        _logger.Info("    {0}", text);
                    }
                }
            }
            catch (ArgumentNullException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (IOException)
            {
            }
            catch (OutOfMemoryException)
            {
            }
        }

        #region Private Members

        private Assembly ExecutionAssembly
        {
            get
            {
                return Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            }
        }

        private string GetFrameworkName()
        {
            return Type.GetType("Mono.Runtime") != null ? "Mono" : ".Net";
        }

        private string GetOSName()
        {
            var oSVersion = Environment.OSVersion;
            if (oSVersion.Platform != PlatformID.Win32NT)
            {
                return "Unknown OS";
            }

            var stringBuilder = new StringBuilder();
            var registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
            if (registryKey == null)
            {
                return "Unknown Windows";
            }

            stringBuilder.Append(registryKey.GetValue("ProductName"));
            if (stringBuilder.Length <= 0)
            {
                return "Unknown Windows";
            }

            if (Environment.OSVersion.ServicePack.Length > 0)
            {
                stringBuilder.Append(" ");
                stringBuilder.Append(Environment.OSVersion.ServicePack);
                stringBuilder.Replace("Service Pack ", "SP");
            }

            return stringBuilder.ToString();
        }

        #endregion
    }
}

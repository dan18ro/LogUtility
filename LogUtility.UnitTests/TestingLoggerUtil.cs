using System.IO;
using System.Reflection;

using LogUtility.Asynchronous;
using LogUtility.Configuration;

using NUnit.Framework;

namespace LogUtility.UnitTests
{
    [TestFixture]
    public class TestingLoggerUtil
    {
        private ILogUtility _synchronousLog;
        private ILogUtility _asynchronousLog;
 
        [SetUp]
        public void Init()
        {
            var config = new LogConfiguration(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "App.config"),
                "LogUtil1");
            _synchronousLog = new LogUtility(config, true);
            
            _asynchronousLog = new AsynchronousLogUtility(config, true);
        }

        [TearDown]
        public void Close()
        {
            _synchronousLog.StopLogging();
            _asynchronousLog.StopLogging();
        }

        [TestCase]
        public void TestSynchronous()
        {
            _synchronousLog.LogStartUpInformation();
            _synchronousLog.Info("Test message");
            _synchronousLog.Debug("Test message");
            _synchronousLog.Warning("Test message");
            _synchronousLog.Fatal("Test message");
            _synchronousLog.Error("Test message");
        }

        [TestCase]
        public void TestAsynchronous()
        {
            _asynchronousLog.LogStartUpInformation();
            _asynchronousLog.Info("Test message");
            _asynchronousLog.Debug("Test message");
            _asynchronousLog.Warning("Test message");
            _asynchronousLog.Fatal("Test message");
            _asynchronousLog.Error("Test message");
        }
    }
}

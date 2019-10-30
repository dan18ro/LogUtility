using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

using LoggerUtil;

using LogUtility.Configuration;

namespace LogUtility.Asynchronous
{
    /// <summary>
    /// Provides the implementation for one asynchronous logging component.
    /// </summary>
    public class AsynchronousLogUtility : LogUtil, ILogUtility
    {
        private readonly BlockingCollection<Message> _queue = new BlockingCollection<Message>(new ConcurrentQueue<Message>());
        private readonly Thread _writer;
        private volatile bool _shouldStop;

        /// <summary>
        /// Instantiates an asynchronous logging object.
        /// </summary>
        /// <param name="logConfiguration">Log configuration objec should be provided.</param>
        /// <param name="consoleAvailable">If console is available, and we want to log on console as well it should be set to true.</param>
        public AsynchronousLogUtility(ILogConfiguration logConfiguration, bool consoleAvailable = false)
            : base(logConfiguration, consoleAvailable)
        {
            BeginStartupInformationLog();
            _writer = new Thread(WriteMessageToFile);
            _writer.Start();

        }


        #region Public Methods
        public override void Error(string message, params object[] args)
        {
            var frame = new StackFrame(1);
            var method = frame.GetMethod();

            AddToQueue(message, args, MessageType.Error, method.ReflectedType.Name + ":" + method.Name);
        }


        public override void Info(string message, params object[] args)
        {
            var frame = new StackFrame(1);
            var method = frame.GetMethod();
            AddToQueue(message, args, MessageType.Info, method.ReflectedType.Name + ":" + method.Name);
        }

        public override void Debug(string message, params object[] args)
        {
            var frame = new StackFrame(1);
            var method = frame.GetMethod();
            AddToQueue(message, args, MessageType.Debug, method.ReflectedType.Name + ":" + method.Name);
        }

        public override void Fatal(string message, params object[] args)
        {
            var frame = new StackFrame(1);
            var method = frame.GetMethod();
            AddToQueue(message, args, MessageType.Fatal, method.ReflectedType.Name + ":" + method.Name);
        }

        public override void Warning(string message, params object[] args)
        {
            var frame = new StackFrame(1);
            var method = frame.GetMethod();
            AddToQueue(message, args, MessageType.Warning, method.ReflectedType.Name + ":" + method.Name);
        }


        public override void StopLogging()
        {
            _shouldStop = true;
            _queue.Add(new Message
                {
                    MessageText = "Shutting down logger component.{0}",
                    Time = DateTime.Now,
                    Type = MessageType.Info,
                    Args = new object[] { "!" }
                });
            EndStartupInformationLog();
        }

        #endregion

        #region Private Methods

        private Message GetMessage(string message, object[] args, MessageType messageType, string methodName)
        {
            return new Message
            {
                Args = args,
                MessageText = message,
                Type = messageType,
                MethodName = methodName,
                Time = DateTime.Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };
        }

        private void AddToQueue(string message, object[] args, MessageType messageType, string methodName)
        {
            _queue.Add(GetMessage(message, args, messageType, methodName));
        }

        private void WriteMessageToFile()
        {

            while (!_shouldStop)
            {
                try
                {
                    Log();
                }
                catch
                {

                }
            }
        }

        private void Log()
        {
            var message = _queue.Take();
            WriteMessage(message);
        }


        private string GetMessageText(Message message)
        {
            if (message == null)
                return null;
            return string.Join(" ",
                               new[]
                                   {
                                       "["+message.Time.Date.ToShortDateString(),
                                       message.Time.TimeOfDay.ToString()+"]", message.Type.ToString(),
                                       "[" + message.ThreadId + ":" + message.MethodName + "]",
                                       string.Format(message.MessageText, message.Args)
                                   });
        }

        private void WriteMessage(Message message)
        {
            var messageText = GetMessageText(message);
            switch (message.Type)
            {
                case MessageType.Debug:
                    Logger.Logger.Log(typeof(AsynchronousLogUtility), log4net.Core.Level.Debug, messageText, null);
                    ConsoleWriter.ConsoleWrite(messageText, "DEBUG");
                    break;
                case MessageType.Error:
                    Logger.Logger.Log(typeof(AsynchronousLogUtility), log4net.Core.Level.Error, messageText, null);
                    ConsoleWriter.ConsoleWrite(messageText, "ERROR");
                    break;
                case MessageType.Fatal:
                    Logger.Logger.Log(typeof(AsynchronousLogUtility), log4net.Core.Level.Fatal, messageText, null);
                    ConsoleWriter.ConsoleWrite(messageText, "FATAL");
                    break;
                case MessageType.Info:
                    Logger.Logger.Log(typeof(AsynchronousLogUtility), log4net.Core.Level.Info, messageText, null);
                    ConsoleWriter.ConsoleWrite(messageText, "INFO");
                    break;
                case MessageType.Warning:
                    Logger.Logger.Log(typeof(AsynchronousLogUtility), log4net.Core.Level.Warn, messageText, null);
                    ConsoleWriter.ConsoleWrite(messageText, "WARN");
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}


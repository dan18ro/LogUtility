using System;

namespace LogUtility.Asynchronous
{
    internal sealed class Message
    {
        public string MessageText { get; set; }
        public object[] Args { get; set; }
        public MessageType Type { get; set; }
        public string MethodName { get; set; }
        public DateTime Time { get; set; }
        public int ThreadId { get; set; }
    }
}

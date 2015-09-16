using System;

namespace Fievel.Wpf.Data
{
    public class LogEventArgs : EventArgs
    {
        public LogInfo Log { get; set; }

        public Guid GroupId { get; set; }

        public LogEventArgs(LogInfo log)
        {
            Log = log;
        }

        public LogEventArgs(LogInfo log, Guid groupId)
            : this(log)
        {
            GroupId = groupId;
        }
    }
}
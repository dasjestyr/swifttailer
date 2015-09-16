using System;

namespace Fievel.Wpf.Data
{
    public class LogGroupEventArgs : EventArgs
    {
        public LogGroup NewGroup { get; set; }

        public LogGroupEventArgs(LogGroup newGroup)
        {
            NewGroup = newGroup;
        }
    }
}
using System;

namespace Fievel.Wpf.Data
{
    public class LogGroupAddedEventArgs : EventArgs
    {
        public LogGroup NewGroup { get; set; }

        public LogGroupAddedEventArgs(LogGroup newGroup)
        {
            NewGroup = newGroup;
        }
    }
}
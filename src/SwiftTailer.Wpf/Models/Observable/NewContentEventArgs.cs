using System;
using System.Collections.Generic;
using System.Linq;

namespace SwiftTailer.Wpf.Models.Observable
{
    public class NewContentEventArgs : EventArgs
    {
        public TailFile Context { get; private set; }

        public IEnumerable<LogLine> NewLines { get; private set; }

        public int NewLineCount { get; private set; }

        public NewContentEventArgs(TailFile context, IEnumerable<LogLine> newLines)
        {
            var lines = newLines.ToList();

            Context = context;
            NewLines = lines;
            NewLineCount = lines.Count();
        }
    }
}
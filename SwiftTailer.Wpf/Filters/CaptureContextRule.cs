using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class CaptureContextRule : ILogLineFilter
    {
        private readonly IList<LogLine> _logLines;

        private int Range => HeadCount + TailCount + 1;

        public int HeadCount { get; set; }

        public int TailCount { get; set; }

        public SearchMode SearchMode { get; set; }

        public string Description => "Captures context of unfiltered results";

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureContextRule" /> class.
        /// </summary>
        /// <param name="headCount">The head count.</param>
        /// <param name="tailCount">The tail count.</param>
        /// <param name="searchMode">The current search mode. This is used to ensure the filter is not applied in the wrong mode.</param>
        /// <param name="logLines">The log lines.</param>
        public CaptureContextRule(int headCount, int tailCount, SearchMode searchMode, IList<LogLine> logLines)
        {
            HeadCount = headCount;
            TailCount = tailCount;
            SearchMode = searchMode;
            _logLines = logLines;
        }

        public bool ApplyFilter(LogLine logLine)
        {
            lock (this)
            {
                // this only applies to Filter mode
                if (SearchMode != SearchMode.Filter || Range == 0)
                    return false;

                Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                {
                    for (var i = 0; i < _logLines.Count; i++)
                    {

                        if (_logLines[i].Highlight.Category != HighlightCategory.None)
                            continue;

                        var contextSlice = ExtractContext(i);
                        _logLines[i].Context.Clear();
                        _logLines[i].Context.AddRange(contextSlice);
                    }
                }).Wait();

                return true;
            }
        }

        private IEnumerable<LogLine> ExtractContext(int currentIndex)
        {
            var startIndex = currentIndex - HeadCount;
            var sliceSize = Range;

            if (startIndex < 0)
            {
                sliceSize += startIndex;
                startIndex = 0;
            }

            if (startIndex + sliceSize > _logLines.Count)
                sliceSize = _logLines.Count - startIndex;
            
            var slice = _logLines
                .Skip(startIndex)
                .Take(sliceSize)
                .ToList();

            return slice;
        } 
    }
}

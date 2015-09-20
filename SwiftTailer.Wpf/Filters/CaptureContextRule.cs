using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class CaptureContextRule : ILogLineFilter
    {
        private readonly ISearchSource _source;
        private readonly IList<LogLine> _logLines;

        private int Range => HeadCount + TailCount + 1;

        public int HeadCount { get; set; }

        public int TailCount { get; set; }

        public string Description => "Captures context of unfiltered results";

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureContextRule" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="headCount">The head count.</param>
        /// <param name="tailCount">The tail count.</param>
        /// <param name="logLines">The log lines.</param>
        public CaptureContextRule(ISearchSource source, int headCount, int tailCount, IList<LogLine> logLines)
        {
            HeadCount = headCount;
            TailCount = tailCount;
            _source = source;
            _logLines = logLines;
        }
        
        public async Task<bool> ApplyFilter(LogLine logLine)
        {
            // this only applies to Filter mode
            if (_source.SearchMode != SearchMode.Filter || 
                string.IsNullOrEmpty(_source.SearchPhrase) || 
                Range < 2)
                    return false;
            
            var logArray = _logLines.ToArray();

            for (var i = 0; i < logArray.Length; i++)
            {
                if (_logLines[i].Highlight.Category != HighlightCategory.None)
                    continue;

                _logLines[i].DeFilter();
                _logLines[i].LineContext = ExtractContext(i, logArray);
            }

            return true;
        }

        private string ExtractContext(int currentIndex, LogLine[] logarray)
        {
            var startIndex = currentIndex - HeadCount;
            var sliceSize = Range;

            // make sure we don't go out of bounds
            if (startIndex < 0)
            {
                sliceSize += startIndex;
                startIndex = 0;
            }

            if (startIndex + sliceSize > _logLines.Count)
                sliceSize = _logLines.Count - startIndex;

            // get the slice
            var slice = new LogLine[sliceSize];
            Array.Copy(logarray, startIndex, slice, 0, sliceSize);

            return GetContextContent(slice);
        }

        private static string GetContextContent(IEnumerable<LogLine> lines)
        {
            var context = string.Join("\n", lines.Select(l => l.Content));
            return context;
        }
    }
}

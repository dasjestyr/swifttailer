using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class CaptureContextRule : ILogLineFilter
    {
        private readonly IList<LogLine> _logLines;
        private CancellationToken _cancellationToken;

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
        /// <param name="cancellationToken">The cancellation token.</param>
        public CaptureContextRule(int headCount, int tailCount, SearchMode searchMode, IList<LogLine> logLines, CancellationToken cancellationToken)
        {
            HeadCount = headCount;
            TailCount = tailCount;
            SearchMode = searchMode;
            _logLines = logLines;
            _cancellationToken = cancellationToken;
        }

        public async Task<bool> ApplyFilter(LogLine logLine)
        {
            // this only applies to Filter mode
            if (SearchMode != SearchMode.Filter || Range == 0)
                return false;

            var logArray = _logLines.ToArray();
            await Task.Run(() =>
            {
                Parallel.ForEach(logArray, async line =>
                {
                    var i = _logLines.IndexOf(line);
                    if (_logLines[i].Highlight.Category != HighlightCategory.None)
                        return;

                    //var context = ExtractContext(i, logArray);
                    _logLines[i].LineContext = await Task.Run(() => ExtractContext(i, logArray), _cancellationToken);
                });
            }, _cancellationToken);

            return true;
        }

        public void SetCancellationToken(CancellationToken token)
        {
            _cancellationToken = token;
        }

        private string ExtractContext(int currentIndex, LogLine[] logarray)
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
            
            Trace.WriteLine($"Extracting {sliceSize} lines...");

            var slice = new LogLine[sliceSize];
            Array.Copy(logarray, startIndex, slice, 0, sliceSize);

            var context = string.Join("\n", slice.Select(l => l.Content));

            return context;
        } 
    }
}

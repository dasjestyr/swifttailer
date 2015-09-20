using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class CaptureContextRule : ILogLineFilter
    {
        private readonly int _headCount;
        private readonly int _tailCount;
        private readonly SearchMode _searchMode;
        private readonly IList<LogLine> _logLines;
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
            _headCount = headCount;
            _tailCount = tailCount;
            _searchMode = searchMode;
            _logLines = logLines;
        }

        public bool ApplyFilter(LogLine logLine)
        {
            // this only applies to Filter mode
            if (_searchMode != SearchMode.Filter)
                return false;
            
            for (var i = 0; i < _logLines.Count; i++)
            {
                if (_logLines[i].Highlight.Category != HighlightCategory.None) continue;

                var contextSlice = ExtractContext(i);
                _logLines[i].Context = new ObservableCollection<LogLine>(contextSlice);
            }

            return true;
        }

        private List<LogLine> ExtractContext(int currentIndex)
        {
            var contextRange = _headCount + _tailCount + 1;
            var startIndex = currentIndex - _headCount;
            var sliceSize = contextRange;

            if (startIndex + sliceSize > _logLines.Count)
                sliceSize = _logLines.Count;

            if (startIndex < 0)
                startIndex = 0;

            var slice = _logLines
                .Skip(startIndex)
                .Take(sliceSize)
                .ToList();

            return slice;
        } 
    }
}

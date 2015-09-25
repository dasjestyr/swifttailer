//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using SwiftTailer.Wpf.Models.Observable;

//namespace SwiftTailer.Wpf.Filters
//{
//    // TODO: considering trying to figure out how to refactor this into the HideLineRule since
//    // this is dependent on that filter being run first
//    public class CaptureContextRule : ILogLineFilter
//    {
//        private readonly ISearchSource _source;
//        private readonly int _headCount;
//        private readonly int _tailCount;
//        private int Range => _headCount + _tailCount + 1;

//        public string Description => "Captures context of unfiltered results";

//        /// <summary>
//        /// Initializes a new instance of the <see cref="CaptureContextRule" /> class.
//        /// </summary>
//        /// <param name="source">The source.</param>
//        /// <param name="headCount">The head count.</param>
//        /// <param name="tailCount">The tail count.</param>
//        public CaptureContextRule(ISearchSource source, int headCount, int tailCount)
//        {
//            _headCount = headCount;
//            _tailCount = tailCount;
//            _source = source;
//        }

//        public async Task<bool> ApplyFilter(LogLine logLine)
//        {
//            // this only applies to Filter mode
//            if (_source.SearchMode != SearchMode.Filter || 
//                string.IsNullOrEmpty(_source.SearchPhrase) || 
//                Range < 2)
//                    return false;

//            var sw = new Stopwatch();
//            sw.Start();

//            var logArray = _source.LogLines.ToArray();
//            for (var i = 0; i < logArray.Length; i++)
//            {
//                if (logArray[i].Highlight.Category != HighlightCategory.None)
//                    continue;

//                logArray[i].DeFilter();
//                _source.LogLines[i].LineContext = ExtractContext(i, logArray);
//            }

//            sw.Stop();
//            Debug.WriteLine($"CaptureContext completed in {sw.ElapsedMilliseconds}ms");
//            return true;
//        }

//        private string ExtractContext(int currentIndex, LogLine[] logarray)
//        {
//            var startIndex = currentIndex - _headCount;
//            var sliceSize = Range;

//            // make sure we don't go out of bounds
//            if (startIndex < 0)
//            {
//                sliceSize += startIndex;
//                startIndex = 0;
//            }

//            if (startIndex + sliceSize > logarray.Length)
//                sliceSize = logarray.Length - startIndex;

//            // get the slice
//            var slice = new LogLine[sliceSize];
//            Array.Copy(logarray, startIndex, slice, 0, sliceSize);

//            return GetContextContent(slice);
//        }

//        private static string GetContextContent(IEnumerable<LogLine> lines)
//        {
//            var context = string.Join("\n", lines.Select(l => l.Content));
//            return context;
//        }
//    }
//}

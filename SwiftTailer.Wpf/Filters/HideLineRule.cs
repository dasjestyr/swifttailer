using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class HideLineRule : ILogLineFilter
    {
        private readonly ISearchSource _source;
        private readonly int _headCount;
        private readonly int _tailCount;

        private int Range => _headCount + _tailCount + 1;

        public string Description => "Applies filter to designate whether or not the line should be hidden. " +
                                     "Do not place this into the same filter chain with another search mode filter.";

        public HideLineRule(ISearchSource source)
            : this(source, 0, 0)
        {
            _source = source;
        }

        public HideLineRule(ISearchSource source, int headCount, int tailCount)
        {
            _source = source;
            _headCount = headCount;
            _tailCount = tailCount;
        }

        public async Task<bool> ApplyFilter(LogLine logLine)
        {
            if (string.IsNullOrEmpty(logLine.Content) || string.IsNullOrEmpty(_source.SearchPhrase))
                return false;

            var sw = new Stopwatch();
            sw.Start();

            bool wasApplied;
            switch (_source.PhraseType)
            {
                case PhraseType.Literal:
                    wasApplied = ApplyLiteral(logLine);
                    break;
                case PhraseType.Regex:
                    wasApplied = ApplyRegex(logLine);
                    break;
                default:
                    wasApplied = false;
                    break;
            }

            sw.Stop();
            Debug.WriteLineIf(
                wasApplied && sw.ElapsedMilliseconds > 0, 
                $"HideLine filter applied in {sw.ElapsedMilliseconds}ms.");

            return wasApplied;
        }

        private bool ApplyLiteral(LogLine logLine)
        {
            if (logLine.Content.IndexOf(_source.SearchPhrase, _source.ComparisonRule) != -1) 
                return false;

            ExtractContext(logLine);
            logLine.Highlight = LogHighlight.Hide;

            return true;
        }

        private bool ApplyRegex(LogLine logLine)
        {
            var regex = new Regex(_source.SearchPhrase);
            if (regex.IsMatch(logLine.Content))
                return false;

            ExtractContext(logLine);
            logLine.Highlight = LogHighlight.Hide;

            return true;
        }

        private void ExtractContext(LogLine logLine)
        {
            if (_source.SearchMode != SearchMode.Filter)
                return;

            var lineIndex = _source.LogLines.IndexOf(logLine);
            var lineContext = ExtractContext(lineIndex, _source.LogLines.ToArray());
            _source.LogLines[lineIndex].LineContext = string.IsNullOrEmpty(lineContext)
                ? logLine.Content
                : lineContext;
        }

        private string ExtractContext(int currentIndex, LogLine[] logarray)
        {
            var startIndex = currentIndex - _headCount;
            var sliceSize = Range;

            // make sure we don't go out of bounds
            if (startIndex < 0)
            {
                sliceSize += startIndex;
                startIndex = 0;
            }

            if (startIndex + sliceSize > logarray.Length)
                sliceSize = logarray.Length - startIndex;

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
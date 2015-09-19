using System;
using System.Diagnostics;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class SearchHighlightFilter : ILogLineFilter
    {
        private readonly ISearchSource _source;
        private readonly StringComparison _comparisonRule;

        public string Description => "Applies highlight to log lines that contain the specified search term";

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchHighlightFilter" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="comparisoneRule">The comparisone rule.</param>
        public SearchHighlightFilter(ISearchSource source, StringComparison comparisoneRule)
        {
            _source = source;
            _comparisonRule = comparisoneRule;
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns></returns>
        public bool ApplyFilter(LogLine logLine)
        {
            if (string.IsNullOrEmpty(_source.SearchPhrase) || // search for spaces is for the birds
                string.IsNullOrEmpty(logLine.Content) || // if a man can't see, he can't fight
                logLine.Content.IndexOf(_source.SearchPhrase, _comparisonRule) == -1) // not droids you're looking for
            {
                Debug.WriteLine($"Skipped {logLine.Content}");
                return false;
            }

            logLine.Highlight = LogHighlight.Find;
            Debug.WriteLine($"Highlighted {logLine.Content}");
            return true;
        }
    }
}

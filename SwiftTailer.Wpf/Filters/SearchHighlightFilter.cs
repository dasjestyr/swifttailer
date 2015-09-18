using System;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class SearchHighlightFilter : ILogLineFilter
    {
        private readonly string _searchPhrase;
        private readonly StringComparison _comparisonRule;

        public string Description => "Applies highlight to log lines that contain the specified search term";

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchHighlightFilter" /> class.
        /// </summary>
        /// <param name="searchPhrase">The search phrase.</param>
        public SearchHighlightFilter(string searchPhrase)
        {
            // TODO: get from settings
            _comparisonRule = StringComparison.OrdinalIgnoreCase;
            _searchPhrase = searchPhrase;
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns></returns>
        public bool ApplyFilter(LogLine logLine)
        {
            if (string.IsNullOrEmpty(logLine.Content) || logLine.Content.IndexOf(_searchPhrase, _comparisonRule) == -1)
                return false;

            logLine.Highlight = LogHighlight.Find;
            return true;
        }
    }
}

using System;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class HideLineFilter : ILogLineFilter
    {
        private readonly StringComparison _comparisonRule;
        private readonly string _phrase;

        public HideLineFilter(string phrase)
        {
            // TODO: get from settings
            _comparisonRule = StringComparison.OrdinalIgnoreCase;
            _phrase = phrase;
        }

        public bool ApplyFilter(LogLine logLine)
        {
            // REFACTOR: this logic may be unreliable since it's really just
            // the opposite of SearchHighlightFilter
            if (string.IsNullOrEmpty(logLine.Content) || logLine.Content.IndexOf(_phrase, _comparisonRule) != -1)
                return false;

            logLine.Highlight = LogHighlight.Hide;
            return true;
        }

        public string Description => "Applies filter to designate whether or not the line should be hidden.";
    }
}
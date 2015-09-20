using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class HideLineRule : ILogLineFilter
    {
        private readonly ISearchSource _source;
        private readonly PhraseType _phraseType;
        private readonly StringComparison _comparisonRule;

        public string Description => "Applies filter to designate whether or not the line should be hidden. " +
                                     "Do not place this into the same filter chain with another search mode filter.";

        public HideLineRule(ISearchSource source, PhraseType phraseType, StringComparison comparisonRule)
        {
            _source = source;
            _phraseType = phraseType;
            _comparisonRule = comparisonRule;
        }

        public async Task<bool> ApplyFilter(LogLine logLine)
        {
            if (string.IsNullOrEmpty(logLine.Content) || string.IsNullOrEmpty(_source.SearchPhrase))
                return false;

            bool wasApplied;
            switch (_phraseType)
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

            return wasApplied;
        }

        private bool ApplyLiteral(LogLine logLine)
        {
            if (logLine.Content.IndexOf(_source.SearchPhrase, _comparisonRule) != -1) // not droids you're looking for
                return false;
            

            logLine.Highlight = LogHighlight.Hide;
            return true;
        }

        private bool ApplyRegex(LogLine logLine)
        {
            var regex = new Regex(_source.SearchPhrase);
            if (regex.IsMatch(logLine.Content))
                return false;

            logLine.Highlight = LogHighlight.Hide;
            return true;
        }
    }
}
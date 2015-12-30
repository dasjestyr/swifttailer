using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SwiftTailer.Wpf.Models;
#pragma warning disable CS1998

namespace SwiftTailer.Wpf.Filters
{
    public class FindHighlightRule : ILogLineFilter
    {
        private readonly ISearchSource _source;
        private readonly PhraseType _phraseType;
        private readonly StringComparison _comparisonRule;

        /// <summary>
        /// Gets or sets the description of the filter.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description => "Applies highlight to log lines that contain the specified search term. " +
                                     "Do not place this in the same filter chain as another search mode filter.";

        /// <summary>
        /// Initializes a new instance of the <see cref="FindHighlightRule" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="phraseType"></param>
        /// <param name="comparisoneRule">The comparisone rule.</param>
        public FindHighlightRule(ISearchSource source, PhraseType phraseType, StringComparison comparisoneRule)
        {
            _source = source;
            _phraseType = phraseType;
            _comparisonRule = comparisoneRule;
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns></returns>
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
            //if (logLine.Content.IndexOf(_source.SearchPhrase, _comparisonRule) == -1) // not droids you're looking for
            //    return false;

            logLine.Highlight = LogHighlight.Find;
            return true;
        }

        private bool ApplyRegex(LogLine logLine)
        {
            try
            {
                // the user could still be typing... 
                
                var regex = new Regex(_source.SearchPhrase);
                if (!regex.IsMatch(logLine.Content))
                    return false;

                var matchString = (
                    from Match match in regex.Matches(logLine.Content)
                    select match.Value)
                    .ToList();

                logLine.SearchOptions.SearchPhrase = string.Join(",", matchString);
                logLine.Highlight = LogHighlight.Find;
                return true;
            }
            catch
            {
                logLine.SearchOptions.SearchPhrase = string.Empty;
                return false;
            }
        }
    }
}

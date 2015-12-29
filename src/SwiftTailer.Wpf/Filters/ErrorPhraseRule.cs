using System;
using System.Linq;
using System.Threading.Tasks;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class ErrorPhraseRule : ILogLineFilter
    {
        private readonly ISearchSource _source;
        public string Description => "Applies 'Error' highlighting for keywords";

        public ErrorPhraseRule(ISearchSource source)
        {
            _source = source;
        }

        public async Task<bool> ApplyFilter(LogLine logLine)
        {
            if (string.IsNullOrEmpty(logLine.Content) || !_source.ErrorPhraseCollection.Any())
                return false;

            if (_source.ErrorPhraseCollection.All(word => logLine.Content.IndexOf(word, StringComparison.OrdinalIgnoreCase) == -1))
                return false;

            logLine.Highlight = LogHighlight.Error;
            return true;
        }
    }

    public class GeneralPhraseRule : ILogLineFilter
    {
        private readonly ISearchSource _source;
        public string Description => "Applies 'General' highlight for keywords";

        public GeneralPhraseRule(ISearchSource source)
        {
            _source = source;
        }

        public async Task<bool> ApplyFilter(LogLine logLine)
        {
            if (string.IsNullOrEmpty(logLine.Content) || !_source.GeneralPhraseCollection.Any())
                return false;

            if (_source.GeneralPhraseCollection
                .All(word => logLine.Content.IndexOf(word, StringComparison.OrdinalIgnoreCase) == -1))
                return false;

            logLine.Highlight = LogHighlight.General;
            return true;
        }
    }
}

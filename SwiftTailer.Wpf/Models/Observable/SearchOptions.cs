using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SwiftTailer.Wpf.Filters;

namespace SwiftTailer.Wpf.Models.Observable
{
    public class SearchOptions : ModelBase, ISearchSource
    {
        private readonly TailFile _tail;
        private bool _caseSensitive;
        private string _searchPhrase;
        private SearchMode _searchMode = SearchMode.Find;
        private PhraseType _phraseType = PhraseType.Literal;
        private string _errorPhrases = string.Empty;
        private string _generalPhrases = string.Empty;

        private StringComparison CompareRule
            => CaseSensitive
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

        public IEnumerable<string> ErrorPhraseCollection
            => ErrorPhrases.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

        public IEnumerable<string> GeneralPhraseCollection
            => GeneralPhrases.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

        public string SearchPhrase
        {
            get { return _searchPhrase; }
            set
            {
                _searchPhrase = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public string ErrorPhrases
        {
            get { return _errorPhrases; }
            set
            {
                _errorPhrases = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public string GeneralPhrases
        {
            get { return _generalPhrases; }
            set
            {
                _generalPhrases = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set
            {
                _caseSensitive = value;
                SetApplicator();
                OnPropertyChanged();
            }
        }

        public SearchMode SearchMode
        {
            get { return _searchMode; }
            set
            {
                _searchMode = value;
                SetApplicator();
                OnPropertyChanged();
            }
        }

        public PhraseType PhraseType
        {
            get { return _phraseType; }
            set
            {
                _phraseType = value;
                SetApplicator();
                OnPropertyChanged();
            }
        }

        public SearchOptions(TailFile tail)
        {
            _tail = tail;
            HighlightApplicator.ClearGlobalFilters();
            SetApplicator();
        }

        private void SetApplicator()
        {
            HighlightApplicator.ClearGlobalFilters();
            
            if(SearchMode == SearchMode.Find)
                HighlightApplicator.AddFilter(new FindHighlightRule(this, PhraseType, CompareRule));

            if(SearchMode == SearchMode.Filter)
                HighlightApplicator.AddFilter(new HideLineRule(this, PhraseType, CompareRule));

            // auto-enabled
            HighlightApplicator.AddFilter(new GeneralPhraseRule(this));
            HighlightApplicator.AddFilter(new ErrorPhraseRule(this));
            
            Trace.WriteLine("Applicator was reconfigured!");
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            lock (this)
            {
                Task.Run(() => HighlightApplicator.Apply(_tail.LogLines));
            }
        }
    }

    public interface ISearchSource
    {
        string SearchPhrase { get; }

        IEnumerable<string> ErrorPhraseCollection { get; } 

        IEnumerable<string> GeneralPhraseCollection { get; } 
    }

    #region -- Enums --
    public enum SearchMode
    {
        Find,
        Filter
    }

    public enum PhraseType
    {
        Literal,
        Regex
    }
    #endregion
}
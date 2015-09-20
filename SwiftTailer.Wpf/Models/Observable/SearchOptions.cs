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
        private readonly CaptureContextRule _captureRule;
        private readonly TailFile _tail;
        private readonly HighlightApplicator _applicator = new HighlightApplicator();
        private bool _isInitialized;
        private bool _caseSensitive;
        private string _searchPhrase;
        private SearchMode _searchMode = SearchMode.Find;
        private PhraseType _phraseType = PhraseType.Literal;
        private string _errorPhrases = string.Empty;
        private string _generalPhrases = string.Empty;
        private int _contextHeadSize;
        private int _contextTailSize;

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
                InitializeApplicator();
                OnPropertyChanged();
            }
        }

        public SearchMode SearchMode
        {
            get { return _searchMode; }
            set
            {
                _searchMode = value;
                InitializeApplicator();
                OnPropertyChanged();
            }
        }

        public PhraseType PhraseType
        {
            get { return _phraseType; }
            set
            {
                _phraseType = value;
                InitializeApplicator();
                OnPropertyChanged();
            }
        }

        public int ContextHeadSize
        {
            get { return _contextHeadSize; }
            set
            {
                _contextHeadSize = value;
                _captureRule.HeadCount = value;
                ApplyFilters();
                OnPropertyChanged();
            }
        }

        public int ContextTailSize
        {
            get { return _contextTailSize; }
            set
            {
                _contextTailSize = value;
                _captureRule.TailCount = value;
                ApplyFilters();
                OnPropertyChanged();
            }
        }

        public SearchOptions(TailFile tail)
        {
            _tail = tail;

            // keeping a steady instance over this so that we can update the head and tail sizes without
            // re-initializing the applicator
            _captureRule = new CaptureContextRule(ContextHeadSize, ContextTailSize, SearchMode, _tail.LogLines);
            _tail.NewLinesAdded += NewContentAddedHandler;
        }

        private void InitializeApplicator()
        {
            lock (this)
            {
                _applicator.ClearGlobalFilters();
                if (SearchMode == SearchMode.Find)
                    _applicator.AddFilter(new FindHighlightRule(this, PhraseType, CompareRule));

                if (SearchMode == SearchMode.Filter)
                {
                    _captureRule.SearchMode = SearchMode;

                    // must be applied in this order
                    _applicator.AddFilter(new HideLineRule(this, PhraseType, CompareRule));
                    _applicator.AddFilter(_captureRule);
                }

                // auto-enabled
                _applicator.AddFilter(new GeneralPhraseRule(this));
                _applicator.AddFilter(new ErrorPhraseRule(this));

                _isInitialized = true;
                Trace.WriteLine("Applicator was (re)initialized!");

                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (!_isInitialized)
            {
                InitializeApplicator();
            }

            lock (this)
            {
                Task.Run(() => _applicator.Apply(_tail.LogLines));
            }
        }

        private void NewContentAddedHandler(object sender, NewContentEventArgs args)
        {
            _applicator.Apply(args.NewLines);
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
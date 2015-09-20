using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using SwiftTailer.Wpf.Filters;

namespace SwiftTailer.Wpf.Models.Observable
{
    public class SearchOptions : ModelBase, ISearchSource
    {
        private readonly TailFile _tail;
        private readonly HighlightApplicator _applicator = new HighlightApplicator();
        private CancellationTokenSource _cancellationTokenSource;
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
                ApplyFilters();
                OnPropertyChanged();
            }
        }

        public string ErrorPhrases
        {
            get { return _errorPhrases; }
            set
            {
                _errorPhrases = value;
                ApplyFilters();
                OnPropertyChanged();
            }
        }

        public string GeneralPhrases
        {
            get { return _generalPhrases; }
            set
            {
                _generalPhrases = value;
                ApplyFilters();
                OnPropertyChanged();
            }
        }

        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set
            {
                _caseSensitive = value;
                ApplyFilters();
                OnPropertyChanged();
            }
        }

        public SearchMode SearchMode
        {
            get { return _searchMode; }
            set
            {
                _searchMode = value;
                ApplyFilters();
                OnPropertyChanged();
            }
        }

        public PhraseType PhraseType
        {
            get { return _phraseType; }
            set
            {
                _phraseType = value;
                ApplyFilters();
                OnPropertyChanged();
            }
        }

        public int ContextHeadSize
        {
            get { return _contextHeadSize; }
            set
            {
                _contextHeadSize = value;
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
                ApplyFilters();
                OnPropertyChanged();
            }
        }

        public SearchOptions(TailFile tail)
        {
            _tail = tail;
            _tail.NewLinesAdded += NewContentAddedHandler;
        }

        private async void ApplyFilters()
        {
            // doing this every time seems to be the only way 
            // to keep this shit uniformly in sync
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            Trace.WriteLine("Interrupted previous filter configuration");

            _applicator.ClearGlobalFilters();

            switch (SearchMode)
            {
                case SearchMode.Find:
                    _applicator.AddFilter(new FindHighlightRule(this, PhraseType, CompareRule));
                    break;
                case SearchMode.Filter:
                    // must be applied in this order
                    _applicator.AddFilter(new HideLineRule(this, PhraseType, CompareRule));
                    _applicator.AddFilter(new CaptureContextRule(this, ContextHeadSize, ContextTailSize, _tail.LogLines, _cancellationTokenSource.Token));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // auto-enabled
            _applicator.AddFilter(new GeneralPhraseRule(this));
            _applicator.AddFilter(new ErrorPhraseRule(this));

            Trace.WriteLine("Applying filters...");

            // make sure to stop anything that's still in flight

            await Application.Current.Dispatcher.InvokeAsync(() => _applicator.Apply(_tail.LogLines, _cancellationTokenSource));
        }

        private void NewContentAddedHandler(object sender, NewContentEventArgs args)
        {
            ApplyFilters();
        }
    }

    public interface ISearchSource
    {
        SearchMode SearchMode { get; }

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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SwiftTailer.Wpf.Filters;

namespace SwiftTailer.Wpf.Models.Observable
{
    public class SearchOptions : ModelBase, ISearchSource
    {
        private readonly CaptureContextRule _captureRule;
        private readonly TailFile _tail;
        private readonly HighlightApplicator _applicator = new HighlightApplicator();
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isInitialized;
        private bool _caseSensitive;
        private string _searchPhrase;
        private SearchMode _searchMode = SearchMode.Find;
        private PhraseType _phraseType = PhraseType.Literal;
        private string _errorPhrases = string.Empty;
        private string _generalPhrases = string.Empty;
        private int _contextHeadSize = 4;
        private int _contextTailSize = 4;

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
            _cancellationTokenSource = new CancellationTokenSource();
            _captureRule = new CaptureContextRule(ContextHeadSize, ContextTailSize, SearchMode, _tail.LogLines, _cancellationTokenSource.Token);
            _tail.NewLinesAdded += NewContentAddedHandler;
        }

        private void InitializeApplicator()
        {
            lock (this)
            {
                _cancellationTokenSource = new CancellationTokenSource();
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

        private async void ApplyFilters()
        {
            if (!_isInitialized)
                InitializeApplicator();
            
            Trace.WriteLine("Applying filters...");

            // make sure to stop anything that's still in flight
            ResetCancellationToken();
            await Application.Current.Dispatcher.InvokeAsync(() => _applicator.Apply(_tail.LogLines, _cancellationTokenSource));
        }

        private void NewContentAddedHandler(object sender, NewContentEventArgs args)
        {
            // these feels disjointed in that it's not in sync with anything else
            //ResetCancellationToken();
            _applicator.Apply(args.NewLines, _cancellationTokenSource);
        }

        private void ResetCancellationToken()
        {
            Trace.WriteLine("Cancelling all currently running filters...");
            _cancellationTokenSource.Cancel();

            Trace.WriteLine("Creating a new token source...");
            _cancellationTokenSource = new CancellationTokenSource();
            _captureRule.SetCancellationToken(_cancellationTokenSource.Token);
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<LogLine> LogLines => _tail.LogLines;

        /// <summary>
        /// Gets or sets the search phrase.
        /// </summary>
        /// <value>
        /// The search phrase.
        /// </value>
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

        /// <summary>
        /// Gets or sets the error phrases.
        /// </summary>
        /// <value>
        /// The error phrases.
        /// </value>
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

        /// <summary>
        /// Gets or sets the general phrases.
        /// </summary>
        /// <value>
        /// The general phrases.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether [case sensitive].
        /// </summary>
        /// <value>
        /// {D255958A-8513-4226-94B9-080D98F904A1}  <c>true</c> if [case sensitive]; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets the search mode.
        /// </summary>
        /// <value>
        /// The search mode.
        /// </value>
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

        /// <summary>
        /// Gets or sets the type of the phrase.
        /// </summary>
        /// <value>
        /// The type of the phrase.
        /// </value>
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

        /// <summary>
        /// Gets or sets the size of the context head.
        /// </summary>
        /// <value>
        /// The size of the context head.
        /// </value>
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

        /// <summary>
        /// Gets or sets the size of the context tail.
        /// </summary>
        /// <value>
        /// The size of the context tail.
        /// </value>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchOptions" /> class.
        /// </summary>
        /// <param name="tail">The tail.</param>
        public SearchOptions(TailFile tail)
        {
            _tail = tail;
            _tail.NewLinesAdded += NewContentAddedHandler;
        }

        private void ApplyFilters()
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
                    _applicator.AddFilter(new CaptureContextRule(this, ContextHeadSize, ContextTailSize));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // auto-enabled
            _applicator.AddFilter(new GeneralPhraseRule(this));
            _applicator.AddFilter(new ErrorPhraseRule(this));

            Trace.WriteLine("Applying filters...");

            // make sure to stop anything that's still in flight

            Application.Current.Dispatcher.Invoke(() => _applicator.Apply(_tail.LogLines, _cancellationTokenSource));
        }

        private void NewContentAddedHandler(object sender, NewContentEventArgs args)
        {
            ApplyFilters();
        }
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
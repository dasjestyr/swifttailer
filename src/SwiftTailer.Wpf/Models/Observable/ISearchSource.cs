using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SwiftTailer.Wpf.Models.Observable
{
    public interface ISearchSource
    {
        /// <summary>
        /// Gets the search mode.
        /// </summary>
        /// <value>
        /// The search mode.
        /// </value>
        SearchMode SearchMode { get; }

        /// <summary>
        /// Gets the type of the phrase.
        /// </summary>
        /// <value>
        /// The type of the phrase.
        /// </value>
        PhraseType PhraseType { get; }

        /// <summary>
        /// Gets the comparison rule.
        /// </summary>
        /// <value>
        /// The comparison rule.
        /// </value>
        StringComparison ComparisonRule { get; }

        /// <summary>
        /// Gets the search phrase.
        /// </summary>
        /// <value>
        /// The search phrase.
        /// </value>
        string SearchPhrase { get; }

        /// <summary>
        /// Gets the error phrase collection.
        /// </summary>
        /// <value>
        /// The error phrase collection.
        /// </value>
        IEnumerable<string> ErrorPhraseCollection { get; }

        /// <summary>
        /// Gets the general phrase collection.
        /// </summary>
        /// <value>
        /// The general phrase collection.
        /// </value>
        IEnumerable<string> GeneralPhraseCollection { get; }

        /// <summary>
        /// Gets the log lines.
        /// </summary>
        /// <value>
        /// The log lines.
        /// </value>
        ObservableCollection<LogLine> LogLines { get; } 
    }
}
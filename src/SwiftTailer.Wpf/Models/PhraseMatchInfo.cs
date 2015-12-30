using SwiftTailer.Wpf.Filters;

namespace SwiftTailer.Wpf.Models
{
    /// <summary>
    /// Represents a string search match
    /// </summary>
    public class PhraseMatchInfo
    {
        // TODO: Consider having this class perform the match

        public string Source { get; }

        /// <summary>
        /// Gets the starting index of the match.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; }

        /// <summary>
        /// Gets the length of the match
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length { get; }

        /// <summary>
        /// Gets the highlight.
        /// </summary>
        /// <value>
        /// The highlight.
        /// </value>
        public LogHighlight Highlight { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhraseMatchInfo" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length.</param>
        /// <param name="category">The category.</param>
        public PhraseMatchInfo(string source, int index, int length, LogHighlight category)
        {
            Source = source;
            Index = index;
            Length = length;
            Highlight = category;
        }
    }
}
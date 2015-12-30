using System.Windows.Media;

namespace SwiftTailer.Wpf.Filters
{
    public class LogHighlight
    {
        // TODO: maybe leverage this rich enum to provide a user-defined color as well?

        /// <summary>
        /// No hilight
        /// </summary>
        public static LogHighlight None => new LogHighlight(HighlightCategory.None, Colors.Transparent);

        /// <summary>
        /// Highlight search hit
        /// </summary>
        /// <value>
        /// The find.
        /// </value>
        public static LogHighlight Find => new LogHighlight(HighlightCategory.Find, Colors.Goldenrod);

        /// <summary>
        /// Highlight an error hit
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public static LogHighlight Error => new LogHighlight(HighlightCategory.Error, Colors.Red);

        /// <summary>
        /// Indicates that the object should be hidden.
        /// </summary>
        /// <value>
        /// The hide.
        /// </value>
        public static LogHighlight Hide => new LogHighlight(HighlightCategory.Hide, Colors.Transparent);

        /// <summary>
        /// Gets or sets the general.
        /// </summary>
        /// <value>
        /// The general.
        /// </value>
        public static LogHighlight General => new LogHighlight(HighlightCategory.General, Colors.Green);


        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public HighlightCategory Category { get; set; }

        public Color HighlightColor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogHighlight" /> class.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="highlightColor">Color of the highlight.</param>
        public LogHighlight(HighlightCategory category, Color highlightColor)
        {
            Category = category;
            HighlightColor = highlightColor;
        }

        public override string ToString()
        {
            return Category.ToString();
        }
    }
}
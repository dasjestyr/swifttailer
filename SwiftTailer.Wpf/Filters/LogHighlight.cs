namespace SwiftTailer.Wpf.Filters
{
    public class LogHighlight
    {
        // TODO: maybe leverage this rich enum to provide a user-defined color as well?

        /// <summary>
        /// No hilight
        /// </summary>
        public static LogHighlight None => new LogHighlight(HighlightCategory.None);

        /// <summary>
        /// Highlight search hit
        /// </summary>
        /// <value>
        /// The find.
        /// </value>
        public static LogHighlight Find => new LogHighlight(HighlightCategory.Find);

        /// <summary>
        /// Highlight an error hit
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public static LogHighlight Error => new LogHighlight(HighlightCategory.Error);

        /// <summary>
        /// Indicates that the object should be hidden.
        /// </summary>
        /// <value>
        /// The hide.
        /// </value>
        public static LogHighlight Hide => new LogHighlight(HighlightCategory.Hide);



        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public HighlightCategory Category { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogHighlight" /> class.
        /// </summary>
        /// <param name="category">The category.</param>
        public LogHighlight(HighlightCategory category)
        {
            Category = category;
        }

        public override string ToString()
        {
            return Category.ToString();
        }
    }
}
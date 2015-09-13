namespace Fivel.Wpf.Models.Observable
{
    public class LogHighlight
    {
        public static LogHighlight None => new LogHighlight(HighlightCategory.None);

        public static LogHighlight Find => new LogHighlight(HighlightCategory.Find);

        public static LogHighlight Error => new LogHighlight(HighlightCategory.Error);

        public enum HighlightCategory
        {
            None,
            Find,
            Error
        }

        public HighlightCategory Category { get; set; }

        public LogHighlight(HighlightCategory category)
        {
            Category = category;
        }
    }
}
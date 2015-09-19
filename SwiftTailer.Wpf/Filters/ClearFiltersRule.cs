using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class ClearFiltersRule : ILogLineFilter
    {
        public bool ApplyFilter(LogLine logLine)
        {
            logLine.Highlight = LogHighlight.None;
            return true;
        }

        public string Description => "Sets all log line highlights to 'none'";
    }
}
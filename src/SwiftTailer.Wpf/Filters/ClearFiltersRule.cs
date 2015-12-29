using System.Threading.Tasks;
using SwiftTailer.Wpf.Models;

namespace SwiftTailer.Wpf.Filters
{
    public class ClearFiltersRule : ILogLineFilter
    {
        public async Task<bool> ApplyFilter(LogLine logLine)
        {
            logLine.DeFilter();
            return true;
        }

        public string Description => "Sets all log line highlights to 'none'";
    }
}
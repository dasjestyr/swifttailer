using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public static class HighlightApplicator
    {
        private static readonly List<ILogLineFilter> _filters = new List<ILogLineFilter>();
        
        // TODO: global filters are half-baked at the moment. How should they work? Maybe user settings determine whether they're
        // applied before or after explicit rules?

        /// <summary>
        /// Gets the loaded filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public static IReadOnlyCollection<ILogLineFilter> GlobalFilters => new ReadOnlyCollection<ILogLineFilter>(_filters);

        /// <summary>
        /// Applies the global filter chain to the each log line.
        /// </summary>
        /// <param name="logLines">The log lines.</param>
        public static void Apply(IEnumerable<LogLine> logLines)
        {
            // TODO: maybe there's a decent way to run this in parallel?
            var logLineList = logLines.ToList();
            foreach (var filter in _filters)
            {
                Apply(filter, logLineList);
            }
        }


        /// <summary>
        /// Applies the specified log lines.
        /// </summary>
        /// <param name="logLines">The log lines.</param>
        /// <param name="filters">The filters to be applied in order. The last rule that applies will be the rule applied. For example, if a line matches 'dog' and then 'cat' then the rule for 'cat' will be applied.</param>
        public static void Apply(IEnumerable<LogLine> logLines, params ILogLineFilter[] filters)
        {
            // TODO: maybe there's a decent way to run this in parallel?
            var logLineList = logLines.ToList();
            foreach (var filter in filters)
            {
                Apply(filter, logLineList);
            }
        }

        /// <summary>
        /// Helper method. Applies a single filter to a collection of log lines.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="logLines">The log lines.</param>
        public static void Apply(ILogLineFilter filter, IEnumerable<LogLine> logLines)
        {
            foreach (var line in logLines)
            {
                filter.ApplyFilter(line);
            }
        }

        /// <summary>
        /// Adds the filter to a global set.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public static void AddFilter(ILogLineFilter filter)
        {
            _filters.Add(filter);
        }

        /// <summary>
        /// Adds the filters to a global set.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void AddFilter(IEnumerable<ILogLineFilter> filters)
        {
            _filters.AddRange(filters);
        }
    }
}
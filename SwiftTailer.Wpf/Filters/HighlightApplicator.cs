using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public static class HighlightApplicator
    {
        private static readonly List<ILogLineFilter> _filters = new List<ILogLineFilter>();
        
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
        /// <param name="breakOnFirst">If true, the filter chain will stop processing when any rule gets applied.</param>
        public static void Apply(IEnumerable<LogLine> logLines, bool breakOnFirst)
        {
            // TODO: maybe there's a decent way to run this in parallel?
            var logLineList = logLines.ToList();
            foreach (var filter in _filters)
            {
                Apply(filter, logLineList, breakOnFirst);
            }
        }


        public static void Apply(IEnumerable<LogLine> logLines, bool breakOnFirst, params ILogLineFilter[] filters)
        {
            // TODO: maybe there's a decent way to run this in parallel?
            var logLineList = logLines.ToList();
            foreach (var filter in filters)
            {
                Apply(filter, logLineList, breakOnFirst);
            }
        }

        /// <summary>
        /// Helper method. Applies a single filter to a collection of log lines.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="logLines">The log lines.</param>
        /// <param name="breakOnFirst">If true, the filter chain will stop processing when any rule gets applied.</param>
        public static void Apply(ILogLineFilter filter, IEnumerable<LogLine> logLines, bool breakOnFirst)
        {
            foreach (var line in logLines)
            {
                if (filter.ApplyFilter(line) && breakOnFirst)
                    break;
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
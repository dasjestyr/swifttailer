using System.Collections.Generic;
using System.Collections.ObjectModel;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class LogLineFilterChain
    {
        private readonly List<ILogLineFilter> _filters = new List<ILogLineFilter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LogLineFilterChain" /> class.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public LogLineFilterChain(params ILogLineFilter[] filters)
        {
            AddFilter(filters);
        }

        /// <summary>
        /// Gets the loaded filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public IReadOnlyCollection<ILogLineFilter> Filters => new ReadOnlyCollection<ILogLineFilter>(_filters);

        /// <summary>
        /// Applies the filter chain to the each log line.
        /// </summary>
        /// <param name="logLines">The log lines.</param>
        /// <param name="breakOnFirst">If true, the filter chain will stop processing when any rule gets applied.</param>
        public void ApplyFilterChain(IEnumerable<LogLine> logLines, bool breakOnFirst)
        {
            // TODO: maybe there's a decent way to run this in parallel?
            foreach (var logLine in logLines)
            {
                // run line through all filters
                foreach (var filter in Filters)
                {
                    if (filter.ApplyFilter(logLine) && breakOnFirst)
                        break;
                }
            }
        }

        /// <summary>
        /// Adds the filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void AddFilter(ILogLineFilter filter)
        {
            _filters.Add(filter);
        }

        /// <summary>
        /// Adds the filter.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public void AddFilter(IEnumerable<ILogLineFilter> filters)
        {
            _filters.AddRange(filters);
        }
    }
}
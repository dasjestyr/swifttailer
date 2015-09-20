using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public class HighlightApplicator
    {
        private readonly List<ILogLineFilter> _globalFilters = new List<ILogLineFilter>();
        private readonly object _lockObject;

        /// <summary>
        /// Gets the loaded filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public IReadOnlyCollection<ILogLineFilter> GlobalFilters => new ReadOnlyCollection<ILogLineFilter>(_globalFilters);
        
        /// <summary>
        /// Clears the global filters. This will also insert the ClearFilter rule at the top of the collection
        /// </summary>
        public void ClearGlobalFilters()
        {
            lock (_lockObject)
            {
                _globalFilters.Clear();
                _globalFilters.Add(new ClearFiltersRule()); // always needs to be first
            }
        }

        public HighlightApplicator()
        {
            _lockObject = new object();
        }

        /// <summary>
        /// Applies the global filter chain to the each log line.
        /// </summary>
        /// <param name="logLines">The log lines.</param>
        /// <param name="cts">The CTS.</param>
        public void Apply(IEnumerable<LogLine> logLines, CancellationTokenSource cts)
        {
            var logLineList = logLines.ToList();

            // TODO: maybe there's a decent way to run this in parallel?
            // TODO: this tends to throw exceptions on startup stating the collection has been modified - the app will resume

            lock (_lockObject)
            {
                foreach (var filter in _globalFilters)
                {
                    Apply(filter, logLineList, cts);
                }
            }
        }

        /// <summary>
        /// Applies the specified log lines.
        /// </summary>
        /// <param name="logLines">The log lines.</param>
        /// <param name="cts">The CTS.</param>
        /// <param name="filters">The filters to be applied in order. The last rule that applies will be the rule applied. For example, if a line matches 'dog' and then 'cat' then the rule for 'cat' will be applied.</param>
        public async void Apply(IEnumerable<LogLine> logLines, CancellationTokenSource cts, params ILogLineFilter[] filters)
        {
            // TODO: maybe there's a decent way to run this in parallel?
            var logLineList = logLines.ToList();
            foreach (var filter in filters)
            {
                await Apply(filter, logLineList, cts);
            }
        }

        /// <summary>
        /// Helper method. Applies a single filter to a collection of log lines.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="logLines">The log lines.</param>
        /// <param name="cts">The CTS.</param>
        public async Task Apply(ILogLineFilter filter, IEnumerable<LogLine> logLines, CancellationTokenSource cts)
        {
            await Task.Run(async () =>
            {
                foreach (var line in logLines)
                {
                    await filter.ApplyFilter(line);
                }
            }, cts.Token);
        }

        /// <summary>
        /// Adds the filter to a global set.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void AddFilter(ILogLineFilter filter)
        {
            lock (_lockObject)
            {
                _globalFilters.Add(filter);
            }
        }

        /// <summary>
        /// Adds the filters to a global set.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public void AddFilter(IEnumerable<ILogLineFilter> filters)
        {
            lock (_lockObject)
            {
                _globalFilters.AddRange(filters);
            }
        }

        /// <summary>
        /// Removes the filter from the global set.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void RemoveFilter(ILogLineFilter filter)
        {
            _globalFilters.Remove(filter);
            Debug.WriteLine($"Removed {filter.Description} from global set");
        }
    }
}
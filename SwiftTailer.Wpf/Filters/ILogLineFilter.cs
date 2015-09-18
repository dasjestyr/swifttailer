using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.Filters
{
    public interface ILogLineFilter
    {
        /// <summary>
        /// When implemented, applies filters to log line and returns true or false, indicating whether or not the filter was a applied.
        /// </summary>
        /// <param name="logLine">The log line to be processed.</param>
        /// <returns></returns>
        bool ApplyFilter(LogLine logLine);

        /// <summary>
        /// Gets the description of the filter.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description { get; }
    }
}
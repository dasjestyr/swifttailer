using System.Collections.Generic;
using Newtonsoft.Json;

namespace SwiftTailer.Wpf.Data
{
    public class Logs
    {
        [JsonProperty("groups")]
        public List<LogGroup> Groups { get; set; }

        /// <summary>
        /// Adds the log group to the LogSource singleton.
        /// </summary>
        /// <param name="group">The group.</param>
        public void AddLogGroup(LogGroup group)
        {
            LogSource.Instance.AddGroup(group);
        }
    }
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fievel.Wpf.Data
{
    public class LogGroup
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("logs")]
        public List<LogInfo> Logs { get; set; } = new List<LogInfo>();

        public LogGroup()
        {
            Id = Guid.NewGuid();
        }

        public LogGroup(string name)
            : this()
        {
            Name = name;
        }

        /// <summary>
        /// Adds the log through the LogSource singleton.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AddLog(LogInfo log)
        {
            LogSource.Instance.AddLog(Id, log);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<LogInfo> Logs { get; set; }

        public LogGroup()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Adds the log. The order will be set automatically.
        /// </summary>
        /// <param name="log">The log.</param>
        public void AddLog(LogInfo log)
        {
            if (Logs.Any())
            {
                var highestOrder = Logs.Max(l => l.Order);
                log.Order = highestOrder + 1;
            }
            else
            {
                log.Order = 0;
            }
            
            Logs.Add(log);
        }
    }
}
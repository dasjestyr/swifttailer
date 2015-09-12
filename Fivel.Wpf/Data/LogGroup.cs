using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fivel.Wpf.Data
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
    }
}
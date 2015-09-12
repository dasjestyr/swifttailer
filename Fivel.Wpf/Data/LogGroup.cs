using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fivel.Wpf.Data
{
    public class LogGroup
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("logs")]
        public List<LogInfo> Logs { get; set; }
    }
}
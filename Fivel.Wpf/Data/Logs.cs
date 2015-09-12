using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fivel.Wpf.Data
{
    public class Logs
    {
        [JsonProperty("groups")]
        public List<LogGroup> Groups { get; set; }
    }
}
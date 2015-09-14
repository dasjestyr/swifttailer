using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fievel.Wpf.Data
{
    public class Logs
    {
        [JsonProperty("groups")]
        public List<LogGroup> Groups { get; set; }
    }
}
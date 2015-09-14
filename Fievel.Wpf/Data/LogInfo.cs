using System;
using Newtonsoft.Json;

namespace Fievel.Wpf.Data
{
    public class LogInfo
    {
        [JsonIgnore]
        public string Id { get; private set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        public LogInfo()
        {
            Id = Guid.NewGuid().ToString("N");
        }
    }
}
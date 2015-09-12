using Newtonsoft.Json;

namespace Fivel.Wpf.Data
{
    public class LogInfo
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }
    }
}
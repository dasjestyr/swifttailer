using Newtonsoft.Json;

namespace SwiftTailer.Wpf.Data
{
    public class SwifTailerSettings
    {
        [JsonProperty("MaxReadLength")]
        public int MaxReadLength { get; set; }

        [JsonProperty("MaxDisplayLines")]
        public int MaxDisplayLines { get; set; }

        [JsonProperty("PollingInterval")]
        public int PollingInterval { get; set; }

        [JsonProperty("UserEmail")]
        public string UserEmail { get; set; }

        [JsonProperty("AutoFollow")]
        public bool AutoFollowTail { get; set; }
    }
}
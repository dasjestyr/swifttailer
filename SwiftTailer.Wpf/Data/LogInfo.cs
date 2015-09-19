using System;
using System.IO;
using Newtonsoft.Json;

namespace SwiftTailer.Wpf.Data
{
    public class LogInfo
    {
        private string _alias;

        [JsonIgnore]
        public Guid Id { get; private set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("alias")]
        public string Alias
        {
            get
            {
                return string.IsNullOrEmpty(_alias) 
                    ? Path.GetFileName(Location) 
                    : _alias;
            }
            set { _alias = value; }
        }

        [JsonProperty("location")]
        public string Location { get; set; }

        public LogInfo()
            : this(string.Empty)
        {
            
        }

        public LogInfo(string location)
            : this(location, string.Empty)
        {
            
        }

        public LogInfo(string location, string alias)
        {
            Id = Guid.NewGuid();
            Location = location;
            Alias = alias;
        }
    }
}
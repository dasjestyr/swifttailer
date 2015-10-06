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
                    ? Path.GetFileName(Filename) 
                    : _alias;
            }
            set { _alias = value; }
        }

        [JsonProperty("location")]
        public string Filename { get; set; }

        [JsonIgnore]
        public string DirectoryPath => Path.GetDirectoryName(Filename);


        public LogInfo()
            : this(string.Empty)
        {
            
        }

        public LogInfo(string filename)
            : this(filename, string.Empty)
        {
            
        }

        public LogInfo(string filename, string alias)
        {
            Id = Guid.NewGuid();
            Filename = filename;
            Alias = alias;
        }
    }
}
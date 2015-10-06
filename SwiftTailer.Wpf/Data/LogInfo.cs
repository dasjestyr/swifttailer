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
                    ? Path.GetFileName(FullPath) 
                    : _alias;
            }
            set { _alias = value; }
        }

        [JsonProperty("location")]
        public string FullPath { get; set; }

        [JsonIgnore]
        public string DirectoryPath => Path.GetDirectoryName(FullPath);


        public LogInfo()
            : this(string.Empty)
        {
            
        }

        public LogInfo(string fullPath)
            : this(fullPath, string.Empty)
        {
            
        }

        public LogInfo(string fullPath, string alias)
        {
            Id = Guid.NewGuid();
            FullPath = fullPath;
            Alias = alias;
        }
    }
}
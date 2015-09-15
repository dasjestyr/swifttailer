using System;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Fievel.Wpf.Data
{
    public class LogSource : ILogSource
    {
        public event LogCollectionChangedHandler LogCollectionChanged;

        private readonly string _logFileLocation;
        private readonly object _objectLock;
        private Logs _logs;
        private static LogSource _instance;

        public static LogSource Instance => _instance ?? (_instance = new LogSource());

        public Logs Logs
        {
            get { return _logs; }
            private set
            {
                _logs = value;
                OnLogCollectionChanged();
            }
        }

        private LogSource()
        {
            _objectLock = new object();
            var tempFileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DesignTime", "DemoFiles.json");
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);            
            
            var userFilePath = Path.Combine(localAppDataPath, "Fievel");

            // if the app directory doesn't exist, create it
            if (!Directory.Exists(userFilePath))
            {
                Trace.WriteLine($"{userFilePath} did not exist. Creating...");
                Directory.CreateDirectory(userFilePath);
            }

            // if the file doesn't already exist, create it
            _logFileLocation = Path.Combine(userFilePath, "LogConfig.json");
            if (!File.Exists(_logFileLocation))
            {
                Trace.WriteLine($"{_logFileLocation} did not exist. Creating file with a default config...");
                File.Copy(tempFileLocation, _logFileLocation);
            }

            LoadLogs(_logFileLocation);
        }

        public void SaveState()
        {
            var logsJson = JsonConvert.SerializeObject(Logs, Formatting.Indented);
            lock (_objectLock)
            {
                WriteToFile(_logFileLocation, logsJson);
                OnLogCollectionChanged();
            }
        }

        private void LoadLogs(string path)
        {
            
            using (var fs = File.OpenText(path))
            {
                var json = fs.ReadToEnd();
                Logs = JsonConvert.DeserializeObject<Logs>(json);
                OnLogCollectionChanged();
            }
        }

        private void OnLogCollectionChanged()
        {
            LogCollectionChanged?.Invoke(this, new EventArgs());
        }

        private static void WriteToFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }
    }

    public delegate void LogCollectionChangedHandler(object sender, EventArgs args);
}

using System;
using System.IO;
using Newtonsoft.Json;

namespace Fivel.Wpf.Data
{
    public class DemoLogSource : ILogSource
    {
        public event LogCollectionChangedHandler LogCollectionChanged;

        private readonly string _logFileLocation;
        private readonly object _objectLock;
        private Logs _logs;
        private static DemoLogSource _instance;

        public static DemoLogSource Instance => _instance ?? (_instance = new DemoLogSource());

        public Logs Logs
        {
            get { return _logs; }
            private set
            {
                _logs = value;
                OnLogCollectionChanged();
            }
        }

        private DemoLogSource()
        {
            _objectLock = new object();
            _logFileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DesignTime", "DemoFiles.json");
            LoadLogs(_logFileLocation);
        }

        public void SaveState()
        {
            var logsJson = JsonConvert.SerializeObject(Logs);
            lock (_objectLock)
            {
                WriteToFile(_logFileLocation, logsJson);
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

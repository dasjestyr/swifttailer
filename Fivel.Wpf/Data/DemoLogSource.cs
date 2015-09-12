using System;
using System.IO;
using Newtonsoft.Json;

namespace Fivel.Wpf.Data
{
    public class DemoLogSource : ILogSource
    {
        public event LogCollectionChangedHandler LogCollectionChanged;

        private static DemoLogSource _instance;

        public static DemoLogSource Instance => _instance ?? (_instance = new DemoLogSource());

        public Logs Logs { get; private set; }

        private DemoLogSource()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DesignTime", "DemoFiles.json");
            LoadLogs(filePath);
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
    }

    public delegate void LogCollectionChangedHandler(object sender, EventArgs args);
}

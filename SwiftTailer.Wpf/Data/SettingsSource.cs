using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace SwiftTailer.Wpf.Data
{
    public class SettingsSource
    {
        private static SettingsSource _instance;
        private static readonly object Lock = new object();

        public string WorkingDirectory { get; }

        public string SettingsFileLocation { get; }

        public static SettingsSource Instance => _instance ?? (_instance = new SettingsSource());

        public SwifTailerSettings Settings { get; private set; }

        private SettingsSource()
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // if the app directory doesn't exist, create it
            WorkingDirectory = Path.Combine(localAppDataPath, "SwiftTailer");
            if (!Directory.Exists(WorkingDirectory))
            {
                Trace.WriteLine($"{WorkingDirectory} did not exist. Creating...");
                Directory.CreateDirectory(WorkingDirectory);
            }

            var templateLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DesignTime", "Settings.json");

            // if the file doesn't exist, create it
            SettingsFileLocation = Path.Combine(WorkingDirectory, "Settings.json");
            if (!File.Exists(SettingsFileLocation))
            {
                Trace.WriteLine($"{SettingsFileLocation} did not exist. Creating settings file with default template");
                File.Copy(templateLocation, SettingsFileLocation);
            }

            LoadSettingsFromFile();
        }

        public static void SaveState()
        {
            var settings = JsonConvert.SerializeObject(Instance.Settings);
            lock (Lock)
            {
                File.WriteAllText(Instance.SettingsFileLocation, settings);
                Trace.WriteLine("Settings saved!");
            }
        }

        private void LoadSettingsFromFile()
        {
            if (!File.Exists(SettingsFileLocation))
            {
                Trace.WriteLine($"Couldn't find settings file at {SettingsFileLocation}");
                return;
            }

            using (var fs = File.OpenText(SettingsFileLocation))
            {
                var json = fs.ReadToEnd();
                Settings = JsonConvert.DeserializeObject<SwifTailerSettings>(json);
            }
        }
    }

    public class SwifTailerSettings
    {
        [JsonProperty("MaxReadLength")]
        public int MaxReadLength { get; set; }

        [JsonProperty("MaxDisplayLines")]
        public int MaxDisplayLines { get; set; }

        [JsonProperty("PollingInterval")]
        public int PollingInterval { get; set; }
    }
}

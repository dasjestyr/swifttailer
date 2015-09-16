using System;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;

namespace Fievel.Wpf.Data
{
    public class LogSource : ILogSource
    {
        public event LogCollectionChangedHandler LogCollectionChanged;
        public event LogGroupCollectionChangedHandler LogGroupCollectionChanged;
        public event LogGroupAddedHandler LogGroupAdded;
        public event LogGroupDeletedHandler LogGroupDeleted;
        public event LogGroupEditedHandler LogGroupEdited;

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

        public void AddLog(Guid groupId, LogInfo log)
        {
            Logs.Groups.First(group => group.Id.Equals(groupId)).Logs.Add(log);
            SaveState();
            OnLogCollectionChanged();
        }

        public void RemoveLog(LogInfo log)
        {
            // brute force
            foreach (var group in Logs.Groups)
            {
                group.Logs.RemoveAll(l => l.Id.Equals(log.Id));
            }
            SaveState();
            OnLogCollectionChanged();
        }

        public void AddGroup(LogGroup group)
        {
            Logs.Groups.Add(group);
            SaveState();
            OnLogGroupCollectionChanged();
            OnLogGroupAdded(group);
        }

        public void UpdateGroup(LogGroup group)
        {
            var existingGroup = Logs.Groups.First(g => g.Id.Equals(group.Id));
            existingGroup.Name = group.Name;
            SaveState();
            OnLogGroupCollectionChanged();
            OnLogGroupEdited(group);
        }

        public void RemoveGroup(LogGroup group)
        {
            Logs.Groups.RemoveAll(g => g.Id.Equals(group.Id));
            SaveState();
            OnLogGroupCollectionChanged();
            OnLogGroupDeleted();
        }

        private void OnLogCollectionChanged()
        {
            Debug.WriteLine("OnLogCollectionChanged fired (LogSource)");
            LogCollectionChanged?.Invoke(this, new EventArgs());
        }

        private void OnLogGroupCollectionChanged()
        {
            Debug.WriteLine("OnLogGroupCollectionChanged fired (LogSource)");
            LogGroupCollectionChanged?.Invoke(this, new EventArgs());
        }

        private void OnLogGroupAdded(LogGroup newGroup)
        {
            Debug.WriteLine("OnLogAdded fired (LogSource)");
            LogGroupAdded?.Invoke(this, new LogGroupEventArgs(newGroup));
        }

        private void OnLogGroupEdited(LogGroup logGroup)
        {
            Debug.WriteLine("LogGroupEdited fired (LogSource)");
            LogGroupEdited?.Invoke(this, new LogGroupEventArgs(logGroup));
        }

        private void OnLogGroupDeleted()
        {
            Debug.WriteLine("OnLogGroupDeleted fired (LogSource)");
            LogGroupDeleted?.Invoke(this, new EventArgs());
        }

        private static void WriteToFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }
        
    }

    public delegate void LogCollectionChangedHandler(object sender, EventArgs args);

    public delegate void LogGroupCollectionChangedHandler(object sender, EventArgs args);

    public delegate void LogGroupAddedHandler(object sender, LogGroupEventArgs args);

    public delegate void LogGroupDeletedHandler(object sender, EventArgs args);

    public delegate void LogGroupEditedHandler(object sender, LogGroupEventArgs args);
}

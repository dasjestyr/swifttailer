using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace SwiftTailer.Wpf.Data
{
    public class LogSource : ILogSource
    {
        public event LogSourceBoundHandler LogSourceBound;
        public event LogGroupCollectionChangedHandler LogGroupCollectionChanged;
        public event LogGroupAddedHandler LogGroupAdded;
        public event LogGroupDeletedHandler LogGroupDeleted;
        public event LogGroupEditedHandler LogGroupEdited;
        public event LogAddedHandler LogAdded;
        public event LogRemovedHandler LogRemoved;

        private readonly object _objectLock;
        private static LogSource _instance;

        public static LogSource Instance => _instance ?? (_instance = new LogSource());

        public string LogFileLocation { get; }

        public Logs Logs { get; private set; }

        private LogSource()
        {
            _objectLock = new object();
            var tempFileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DesignTime", "DemoFiles.json");
            
            // if the file doesn't already exist, create it
            LogFileLocation = Path.Combine(Settings.WorkingDirectory, "LogConfig.json");
            if (!File.Exists(LogFileLocation))
            {
                Trace.WriteLine($"{LogFileLocation} did not exist. Creating file with a default config...");
                File.Copy(tempFileLocation, LogFileLocation);
            }

            LoadLogs(LogFileLocation);
        }

        public void ReplaceConfig(Logs logs)
        {
            Logs = logs;
            SaveState();
        }

        public void SaveState()
        {
            var logsJson = JsonConvert.SerializeObject(Logs, Formatting.Indented);
            lock (_objectLock)
            {
                WriteToFile(LogFileLocation, logsJson);
            }
            Trace.WriteLine("Saved config state!");
            OnLogSourceBound();
        }

        private void LoadLogs(string path)
        {
            
            using (var fs = File.OpenText(path))
            {
                var json = fs.ReadToEnd();
                Logs = JsonConvert.DeserializeObject<Logs>(json);
                OnLogSourceBound();
            }
        }

        public void AddLog(Guid groupId, LogInfo log)
        {
            var addTo = Logs.Groups
                .First(group => group.Id.Equals(groupId));

            if (addTo.Logs.Any(l => l.Alias.Equals(log.Alias, StringComparison.OrdinalIgnoreCase)))
            {
                Trace.WriteLine($"A log with the alias {log.Alias} already exists in the group {addTo.Name}! Aborting");
                return;
            }

            addTo.Logs.Add(log);

            SaveState();
            OnLogAdded(log, groupId);
        }

        public void AddLog(LogGroup group, IEnumerable<LogInfo> logs)
        {
            var saveGroup = Logs.Groups.FirstOrDefault(g => g.Name.Equals(@group.Name, StringComparison.InvariantCultureIgnoreCase)) ?? @group;

            foreach (var log in logs)
            {
                if(!saveGroup.Logs.Any(l => l.Alias.Equals(log.Alias, StringComparison.Ordinal)))
                    saveGroup.Logs.Add(log);

                // update now to prevent a possible race condition in the OnLogAdded
                OnLogAdded(log, saveGroup.Id); 
            }

            SaveState();
        }

        public void RemoveLog(LogInfo log)
        {
            // brute force
            foreach (var group in Logs.Groups)
            {
                group.Logs.RemoveAll(l => l.Id.Equals(log.Id));
            }

            SaveState();
            OnLogRemoved(log);
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

        private void OnLogSourceBound()
        {
            Debug.WriteLine("OnLogSourceBound fired (LogSource)");
            LogSourceBound?.Invoke(this, new EventArgs());
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

        private void OnLogAdded(LogInfo log, Guid logId)
        {
            Debug.WriteLine("OnLogAdded fired (LogSource)");
            LogAdded?.Invoke(this, new LogEventArgs(log, logId));
        }

        private void OnLogRemoved(LogInfo log)
        {
            Debug.WriteLine("OnLogRemoved fired (LogSource)");
            LogRemoved?.Invoke(this, new LogEventArgs(log));
        }

        private static void WriteToFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }
        
    }

    public delegate void LogCollectionChangedHandler(object sender, EventArgs args);

    public delegate void LogAddedHandler(object sender, LogEventArgs args);

    public delegate void LogRemovedHandler(object sender, LogEventArgs args);

    public delegate void LogGroupCollectionChangedHandler(object sender, EventArgs args);

    public delegate void LogGroupAddedHandler(object sender, LogGroupEventArgs args);

    public delegate void LogGroupDeletedHandler(object sender, EventArgs args);

    public delegate void LogGroupEditedHandler(object sender, LogGroupEventArgs args);

    public delegate void LogSourceBoundHandler(object sender, EventArgs args);

}

﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.ViewModels
{
    public class MainViewModel : ModelBase
    {
        private bool _isRunning;
        private string _status = "Idle";
        private LogGroup _selectedGroup;
        private ObservableCollection<LogGroup> _groups;
        private ObservableCollection<TailFile> _tails = new ObservableCollection<TailFile>();
        private ObservableCollection<LogLine> _selectedLines;
        private TailFile _selectedTail;

        #region -- Commands --
        public SelectGroupCommand SelectGroupCommand { get; private set; }

        public StartTailingCommand StartTailingCommand { get; private set; }

        public StopTailingCommand StopTailingCommand { get; private set; }

        public OpenLogPickerDialogCommand OpenLogPickerDialogCommand { get; set; }

        public AddGroupDialogCommand AddGroupDialogCommand { get; set; }
        
        #endregion

        #region -- Observable Properties --

        public ObservableCollection<LogLine> SelectedLines
        {
            get { return _selectedLines; }
            set
            {
                _selectedLines = value;
                OnPropertyChanged();
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; } 
            set
            {
                _isRunning = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TailFile> Tails
        {
            get { return _tails; }
            set
            {
                _tails = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LogGroup> Groups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                OnPropertyChanged();
            }
        }

        public LogGroup SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                _selectedGroup = value;
                OnPropertyChanged();
                LogSourceChanged(this, new EventArgs());
            }
        }

        public TailFile SelectedTail
        {
            get { return _selectedTail; }
            set
            {
                _selectedTail = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public MainViewModel()
        {
            Groups = new ObservableCollection<LogGroup>(LogSource.Instance.Logs.Groups);
            SelectedGroup = Groups[0];

            SelectedGroup = LogSource.Instance.Logs.Groups[0];
            OpenLogPickerDialogCommand = new OpenLogPickerDialogCommand();
            AddGroupDialogCommand = new AddGroupDialogCommand();
            StopTailingCommand = new StopTailingCommand(this);
            StartTailingCommand = new StartTailingCommand(this);            
            SelectGroupCommand = new SelectGroupCommand(this);
            
            LogSource.Instance.LogCollectionChanged += LogSourceChanged;
            LogSource.Instance.LogAdded += LogAdded;
            LogSource.Instance.LogRemoved += LogRemoved;
            LogSource.Instance.LogGroupCollectionChanged += LogGroupSourceChanged;
            LogSource.Instance.LogGroupAdded += LogGroupAdded;
            LogSource.Instance.LogGroupEdited += LogGroupEdited;
            LogSource.Instance.LogGroupDeleted += LogGroupDeleted;

            StaticCommands.OpenLogLineCommand = new OpenLogLineCommand();
        }

        public void StartTailing()
        {
            Trace.WriteLine("Starting tails...");
            Status = "Running";
            IsRunning = true;

            // iterate in order to fire off tailing tasks
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Tails.Select(t => t.StartTailing()).ToList();
            CommandManager.InvalidateRequerySuggested();
        }
        
        public void StopTailing()
        {
            Trace.WriteLine("Stopping tails...");
            foreach (var tail in Tails)
            {
                tail.StopTailing();
            }
            Status = "Idle";
            IsRunning = false;
            CommandManager.InvalidateRequerySuggested();
        }

        public void SaveOrder()
        {
            // this was used to work with tab re-ordering which is currently disabled

            //var logs = Tails
            //    .ToList()
            //    .Select(log => log.LogInfo)
            //    .ToList();

            //LogSource.Instance.Logs.Groups
            //    .Single(g => g.Id.Equals(SelectedGroup.Id)).Logs = logs;

            //LogSource.Instance.SaveState();
        }

        #region -- Event Handlers --
        
        private void LogSourceChanged(object sender, EventArgs args)
        {
            Debug.WriteLine("Rebinding log tails...");
            if (SelectedGroup == null) return;

            Tails.Clear();

            var tails = SelectedGroup.Logs
                .OrderBy(l => l.Order)
                .Select(t => new TailFile(t));

            foreach (var tail in tails)
            {
                Tails.Add(tail);
            }
        }

        private void LogGroupSourceChanged(object sender, EventArgs args)
        {
            Debug.WriteLine("Updating groups...");
            Groups.Clear();
            foreach (var group in LogSource.Instance.Logs.Groups)
            {
                Groups.Add(group);
            }
        }

        private void LogAdded(object sender, LogEventArgs args)
        {
            Debug.WriteLine("Setting selected tail...");
            SelectedTail = Tails.First(tail => tail.Id.Equals(args.Log.Id));
        }

        private void LogRemoved(object sender, LogEventArgs args)
        {
            Debug.WriteLine("Setting selected tail...");
            SelectedTail = Tails.FirstOrDefault();
        }

        private void LogGroupAdded(object sender, LogGroupEventArgs args)
        {
            Debug.WriteLine("Setting selected group...");
            SelectedGroup = args.NewGroup;
        }

        private void LogGroupEdited(object sender, LogGroupEventArgs args)
        {
            Debug.WriteLine("Setting selected group...");
            SelectedGroup = args.NewGroup;
        }

        private void LogGroupDeleted(object sender, EventArgs args)
        {
            Debug.WriteLine("Setting selected group...");
            SelectedGroup = Groups.Any() ? Groups[0] : null;
        }

        #endregion
    }
}
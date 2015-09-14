using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Fievel.Wpf.Commands;
using Fievel.Wpf.Data;
using Fievel.Wpf.Models.Observable;
using Provausio.Common.Portable;

namespace Fievel.Wpf.ViewModels
{
    public class MainViewModel : ModelBase
    {
        private string _status;
        private LogGroup _selectedGroup;
        private ObservableCollection<LogGroup> _groups;
        private ObservableCollection<TailFile> _tails = new ObservableCollection<TailFile>();
        private bool _isRunning;
        private string _searchPhrase;

        #region -- Commands --
        public SelectGroupCommand SelectGroupCommand { get; private set; }
        public ToggleTailingCommand ToggleTailingCommand { get; private set; }
        public OpenLogPickerDialogCommand OpenLogPickerDialogCommand { get; set; }
        #endregion

        #region -- Observable Properties --
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
                BindGroups();
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

        public string SearchPhrase
        {
            get { return _searchPhrase; }
            set
            {
                _searchPhrase = value;
                UpdateFilters();
                OnPropertyChanged();
            }
        }

        private void UpdateFilters()
        {
            
        }

        #endregion

        public MainViewModel()
        {
            Status = "Idle";

            Groups = new ObservableCollection<LogGroup>(LogSource.Instance.Logs.Groups);
            SelectedGroup = LogSource.Instance.Logs.Groups[0];

            ToggleTailingCommand = new ToggleTailingCommand(this);
            SelectGroupCommand = new SelectGroupCommand(this);
            OpenLogPickerDialogCommand = new OpenLogPickerDialogCommand();

            LogSource.Instance.LogCollectionChanged += LogsChangedHandler;

            BindGroups();
        }

        public void StartTailing()
        {
            Trace.WriteLine("Starting tails...");
            Status = "Running";
            IsRunning = true;
            var tailingTasks = Tails.Select(t => t.StartTailing()).ToList();
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
        }

        public void BindGroups()
        {
            Trace.WriteLine("Rebinding log tails...");
            Tails.Clear();
            var tails = SelectedGroup.Logs
                .OrderBy(l => l.Order)
                .Select(t => new TailFile(t));

            foreach (var tail in tails)
            {
                Tails.Add(tail);
            }
        }

        public void SaveOrder()
        {
            var logs = Tails
                .ToList()
                .Select(log => log.LogInfo)
                .ToList();

            LogSource.Instance.Logs.Groups
                .Single(g => g.Id.Equals(SelectedGroup.Id)).Logs = logs;

            LogSource.Instance.SaveState();
        }

        private void LogsChangedHandler(object sender, EventArgs args)
        {
            BindGroups();
        }
    }
}

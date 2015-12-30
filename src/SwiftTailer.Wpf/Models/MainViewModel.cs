using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Dragablz;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using SwiftTailer.Wpf.Behaviors;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Controls;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.Infrastructure;
using SwiftTailer.Wpf.Pages;

namespace SwiftTailer.Wpf.Models
{
    public class MainViewModel : ModelBase, IProgressProvider, ITailControl
    {
        private bool _isRunning;
        private string _status = "Idle";
        private LogGroup _selectedGroup;
        private ObservableCollection<LogGroup> _groups = new ObservableCollection<LogGroup>();
        private ObservableCollection<TailFile> _tails = new ObservableCollection<TailFile>();
        private ObservableCollection<LogLine> _selectedLines = new ObservableCollection<LogLine>();
        private TailFile _selectedTail;
        private int _progressBarValue;
        private string _progressText;
        private bool _menuIsOpen;

        #region -- Commands --
        public SelectGroupCommand SelectGroupCommand { get; private set; }

        public StartTailingCommand StartTailingCommand { get; private set; }

        public StopTailingCommand StopTailingCommand { get; private set; }

        public ToggleTailingCommand ToggleTailingCommand { get; private set; }

        public OpenLogPickerDialogCommand OpenLogPickerDialogCommand { get; set; }

        public AddGroupDialogCommand AddGroupDialogCommand { get; set; }

        public FollowTailToggleCommand FollowTailToggleCommand { get; set; }
        
        public ICommand ToggleSearchOptionsCommand
        {
            get
            {
                return new RelayCommand(
                obj =>
                {
                    SelectedTail.ShowSearchOptions = !SelectedTail.ShowSearchOptions;
                });
            }
        }
        
        public FileDropMonitor FileDrop { get; }
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
                SetTails();
                SelectedTail = Tails.FirstOrDefault();
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

        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                OnPropertyChanged();
            }
        }

        public string ProgressText
        {
            get { return _progressText; }
            set
            {
                _progressText = value;
                OnPropertyChanged();
            }
        }

        public bool MenuIsOpen
        {
            get { return _menuIsOpen; }
            set
            {
                _menuIsOpen = value;
                OnPropertyChanged();
            }
        }

        public IInterTabClient IntertabClient { get; }

        #endregion

        public MainViewModel()
        {
            IntertabClient = new IntertabClient(new TailerWindowFactory());
            LogSourceBound(this, new EventArgs());
            OpenLogPickerDialogCommand = new OpenLogPickerDialogCommand();
            AddGroupDialogCommand = new AddGroupDialogCommand();
            StopTailingCommand = new StopTailingCommand(this);
            StartTailingCommand = new StartTailingCommand(this);            
            ToggleTailingCommand = new ToggleTailingCommand(this);
            SelectGroupCommand = new SelectGroupCommand(this);
            FollowTailToggleCommand = new FollowTailToggleCommand(this);

            LogSource.Instance.LogSourceBound += LogSourceBound;
            LogSource.Instance.LogAdded += LogAdded;
            LogSource.Instance.LogRemoved += LogRemoved;
            LogSource.Instance.LogGroupCollectionChanged += LogGroupSourceChanged;
            LogSource.Instance.LogGroupAdded += SetSelectedGroup;
            LogSource.Instance.LogGroupEdited += SetSelectedGroup;
            LogSource.Instance.LogGroupDeleted += LogGroupDeleted;

            FileDrop = new FileDropMonitor();
            FileDrop.Dropped.Subscribe(OnFileDrop);
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

        private void SetTails()
        {
            // explicit set for safety
            Tails.Clear();
            if (SelectedGroup != null && SelectedGroup.Logs.Any())
            {
                foreach (var tail in SelectedGroup.Logs
                    .Select(log => new TailFile(log)))
                {
                    Tails.Add(tail);
                }
            }
        }

        #region -- Event Handlers --

        private async void OnFileDrop(FileInfoCollection files)
        {
            var view = new FileDropDialog();
            view.DataContext = new FileDropViewModel(this, files);

            await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            var parameter = eventArgs.Parameter as FileDropResultArgs;
            if (parameter == null) return;

            switch (parameter.Option)
            {
                case FileDropOption.None:
                    MessageBox.Show("You chose cancel");
                    break;
                case FileDropOption.New:
                    MessageBox.Show($"Create {parameter.GroupName}!");
                    break;
                case FileDropOption.Add:
                    MessageBox.Show($"Add to {parameter.GroupName}");
                    break;
            }
        }

        private void LogSourceBound(object sender, EventArgs args)
        {
            // update the groups
            Groups.Clear();
            foreach (var group in LogSource.Instance.Logs.Groups
                .OrderBy(g => g.Name))
            {
                Groups.Add(group);
            }

            // set the selected group
            if (Groups.Any())
                SelectedGroup = Groups[0];

            // update tails
            SetTails();
                
            // set the selected tail
            if (Tails.Any())
                SelectedTail = Tails[0];
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

        private void SetSelectedGroup(object sender, LogGroupEventArgs args)
        {
            Debug.WriteLine("Setting selected group...");
            SelectedGroup = args.NewGroup;
        }

        private void LogAdded(object sender, LogEventArgs args)
        {
            Debug.WriteLine("Setting selected tail...");
            SetTails();
            var tail = Tails.FirstOrDefault(t => t.Id.Equals(args.Log.Id));
            if (tail != null)
                SelectedTail = tail;
        }

        private void LogRemoved(object sender, LogEventArgs args)
        {
            Debug.WriteLine("Setting selected tail...");
            SetTails();
            SelectedTail = Tails.LastOrDefault();
        }

        private void LogGroupDeleted(object sender, EventArgs args)
        {
            Debug.WriteLine("Setting selected group...");
            SelectedGroup = Groups.FirstOrDefault();
        }
        #endregion
    }
}

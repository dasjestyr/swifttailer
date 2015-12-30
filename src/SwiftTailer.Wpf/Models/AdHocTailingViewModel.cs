using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using SwiftTailer.Wpf.Behaviors;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.Infrastructure.Mvvm;

namespace SwiftTailer.Wpf.Models
{
    public class AdHocTailingViewModel : ModelBase, ITailControl
    {
        private string _windowTitle;
        private TailFile _tailFile;
        private LogLine _selectedLine;
        private bool _isRunning;
        private string _status;
        private TailFile _selectedTail;
        private string _logFont = Settings.LogWindowFont;

        #region -- Commands --
        public StartTailingCommand StartTailingCommand { get; private set; }

        public StopTailingCommand StopTailingCommand { get; private set; }

        public ToggleTailingCommand ToggleTailingCommand { get; private set; }

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

        #endregion

        #region -- Observable Properties --

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                OnPropertyChanged();
            }
        }

        public TailFile TailFile
        {
            get { return _tailFile; }
            set
            {
                _tailFile = value;
                OnPropertyChanged();
            }
        }

        public LogLine SelectedLine
        {
            get { return _selectedLine; }
            set
            {
                _selectedLine = value;
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

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                OnPropertyChanged();
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

        public string LogFont
        {
            get { return _logFont; }
            set
            {
                _logFont = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public AdHocTailingViewModel()
        {
            StartTailingCommand = new StartTailingCommand(this);
            StopTailingCommand = new StopTailingCommand(this);
            ToggleTailingCommand = new ToggleTailingCommand(this);
            FollowTailToggleCommand = new FollowTailToggleCommand(this);
        }

        public void SetTail(string path)
        {
            var logInfo = new LogInfo(path);
            TailFile = new TailFile(logInfo);
            SelectedTail = TailFile;
        }

        public void StartTailing()
        {
            Trace.WriteLine("Starting tail...");
            Status = "Running";
            IsRunning = true;

            Task.Run(() => TailFile.StartTailing());
            CommandManager.InvalidateRequerySuggested();
        }

        public void StopTailing()
        {
            Trace.WriteLine("Stopping tail...");
            TailFile.StopTailing();

            Status = "Idle";
            IsRunning = false;
            CommandManager.InvalidateRequerySuggested();
        }

        #region -- Event Handlers --

        public void SettingsChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals("LogWindowFont", StringComparison.OrdinalIgnoreCase))
            {
                LogFont = Settings.LogWindowFont;
            }
        }

        #endregion
    }
}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.ViewModels
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

        public void SetTail(string path)
        {
            var logInfo = new LogInfo(path);
            TailFile = new TailFile(logInfo);
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

        public StartTailingCommand StartTailingCommand { get; private set; }

        public StopTailingCommand StopTailingCommand { get; private set; }

        public AdHocTailingViewModel()
        {
            StartTailingCommand = new StartTailingCommand(this);
            StopTailingCommand = new StopTailingCommand(this);
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

        public void SettingsChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals("LogWindowFont", StringComparison.OrdinalIgnoreCase))
            {
                LogFont = Settings.LogWindowFont;
            }
        }
    }
}

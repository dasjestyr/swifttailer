using System.Collections.ObjectModel;
using Fievel.Wpf.Commands;
using Fievel.Wpf.Data;
using Fievel.Wpf.Models;

namespace Fievel.Wpf.ViewModels
{
    public class LogPickerDialogViewModel : ModelBase
    {
        private string _fileLocation;
        private LogGroup _selectedGroup;
        private ObservableCollection<LogGroup> _availableGroups;
        private string _alias;

        public ObservableCollection<LogGroup> AvailableGroups
        {
            get { return _availableGroups; }
            set
            {
                _availableGroups = value;
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
            }
        }

        public string FileLocation
        {
            get { return _fileLocation; }
            set
            {
                _fileLocation = value;
                OnPropertyChanged();
            }
        }

        public string Alias
        {
            get { return _alias; }
            set
            {
                _alias = value;
                OnPropertyChanged();
            }
        }

        public PickLogCommand PickLogCommand { get; set; }

        public SaveLogToGroupCommand SaveLogToGroupCommand { get; set; }

        public CloseWindowCommand CloseWindowCommand { get; set; }

        public LogPickerDialogViewModel()
        {
            CloseWindowCommand = new CloseWindowCommand();
            PickLogCommand = new PickLogCommand(this);
            AvailableGroups = new ObservableCollection<LogGroup>(LogSource.Instance.Logs.Groups);
            SaveLogToGroupCommand = new SaveLogToGroupCommand(this);
        }
    }
}

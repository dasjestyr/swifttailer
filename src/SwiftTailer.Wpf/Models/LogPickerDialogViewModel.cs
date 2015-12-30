using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.Infrastructure.Mvvm;

namespace SwiftTailer.Wpf.Models
{
    public class LogPickerDialogViewModel : ModelBase, ILogTail
    {
        private string _fileLocation;
        private LogGroup _selectedGroup;
        private string _alias;
        

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
            SaveLogToGroupCommand = new SaveLogToGroupCommand(this);
        }
    }
}

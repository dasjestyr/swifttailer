using Fievel.Wpf.Commands;
using Fievel.Wpf.Data;
using Fievel.Wpf.Models;
using Fievel.Wpf.Models.Observable;

namespace Fievel.Wpf.ViewModels
{
    public class LogPickerDialogViewModel : ModelBase
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

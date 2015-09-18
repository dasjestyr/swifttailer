using System.Collections.ObjectModel;
using SwiftTailer.Wpf.Data;

namespace SwiftTailer.Wpf.ViewModels
{
    public class LogConfigEditorViewModel : Models.Observable.ModelBase
    {
        private ObservableCollection<LogGroup> _logGroups;

        public ObservableCollection<LogGroup> LogGroups
        {
            get { return _logGroups; }
            set
            {
                _logGroups = value;
                OnPropertyChanged();
            }
        }

        public LogConfigEditorViewModel()
        {
            LogGroups = new ObservableCollection<LogGroup>(LogSource.Instance.Logs.Groups);
        }
    }
}

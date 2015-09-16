using System.Collections.ObjectModel;
using Fievel.Wpf.Data;
using Fievel.Wpf.Pages;
using ModelBase = Fievel.Wpf.Models.Observable.ModelBase;

namespace Fievel.Wpf.ViewModels
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

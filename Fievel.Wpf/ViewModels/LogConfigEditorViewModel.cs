using System.Collections.ObjectModel;
using Fievel.Wpf.Data;
using Fievel.Wpf.Pages;
using Provausio.Common.Portable;

namespace Fievel.Wpf.ViewModels
{
    public class LogConfigEditorViewModel : ModelBase
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

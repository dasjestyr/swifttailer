using System.Collections.ObjectModel;
using Fivel.Wpf.Data;
using Fivel.Wpf.Pages;
using Provausio.Common.Portable;

namespace Fivel.Wpf.ViewModels
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
            LogGroups = new ObservableCollection<LogGroup>(DemoLogSource.Instance.Logs.Groups);
        }
    }
}

using System.Collections.ObjectModel;
using System.Linq;
using Fivel.Wpf.Commands;
using Fivel.Wpf.Data;
using Fivel.Wpf.Models.Observable;
using Provausio.Common.Portable;

namespace Fivel.Wpf.ViewModels
{
    public class MainViewModel : ModelBase
    {
        private string _status;
        private LogGroup _selectedGroup;
        private ObservableCollection<LogGroup> _groups;
        private ObservableCollection<TailFile> _tails = new ObservableCollection<TailFile>();

        public SelectGroupCommand SelectGroupCommand { get; private set; }

        public ToggleTailingCommand ToggleTailingCommand { get; private set; }

        public bool IsRunning { get; set; }

        #region -- Observable Properties --

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
                BindGroups();
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

        #endregion

        public MainViewModel()
        {
            Status = "Idle";

            Groups = new ObservableCollection<LogGroup>(DemoLogSource.Instance.Logs.Groups);
            SelectedGroup = DemoLogSource.Instance.Logs.Groups[0];
            ToggleTailingCommand = new ToggleTailingCommand(this);
            SelectGroupCommand = new SelectGroupCommand(this);

            BindGroups();
        }
        
        public void StartTailing()
        {
            Status = "Running";
            IsRunning = true;
            var tasks = Tails.Select(t => t.StartTailing()).ToList();
        }

        public void StopTailing()
        {
            foreach (var tail in Tails)
            {
                tail.StopTailing();
            }
            Status = "Idle";
            IsRunning = false;
        }

        private void BindGroups()
        {
            Tails.Clear();
            var tails = SelectedGroup.Logs.Select(t => new TailFile(t));
            foreach (var tail in tails)
            {
                Tails.Add(tail);
            }
        }
    }
}

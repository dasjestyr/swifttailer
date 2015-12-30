using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.Infrastructure.Mvvm;

namespace SwiftTailer.Wpf.Models
{
    public class EditLogViewModel : ModelBase, ILogTail
    {
        private readonly TailFile _tail;

        public PickLogCommand PickLogCommand { get; set; }

        public string FileLocation
        {
            get { return _tail?.FilePath; }
            set
            {
                _tail.FilePath = value;
                ChangeLogLocation();
                OnPropertyChanged();
            }
        }

        public string Alias
        {
            get { return _tail?.Name; }
            set
            {
                _tail.Name = value;
                LogSource.Instance.SaveState();
                OnPropertyChanged();
            }
        }

        public EditLogViewModel()
        {
            
        }

        public EditLogViewModel(TailFile tail)
        {
            PickLogCommand = new PickLogCommand(this);
            _tail = tail;
        }

        private void ChangeLogLocation()
        {
            var wasRunning = _tail.IsRunning;
            if(wasRunning)
                _tail.StopTailing();

            LogSource.Instance.SaveState();
            _tail.LogLines.Clear();

            if (wasRunning)
#pragma warning disable 4014
                _tail.StartTailing();
#pragma warning restore 4014
        }
    }
}

using System;
using System.Windows.Input;
using SwiftTailer.Wpf.Models;

namespace SwiftTailer.Wpf.Commands
{
    public class ToggleTailingCommand : ICommand
    {
        private readonly ITailControl _vm;

        public ToggleTailingCommand(ITailControl vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return _vm != null;
        }

        public void Execute(object parameter)
        {
            if (!_vm.IsRunning)
                _vm.StartTailing();
            else
                _vm.StopTailing();
        }

        public event EventHandler CanExecuteChanged;
    }

    public class StartTailingCommand : ModelBase, ICommand
    {
        private readonly ITailControl _vm;
        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public StartTailingCommand(ITailControl vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return !_vm.IsRunning;
        }

        public void Execute(object parameter)
        {
            _vm.StartTailing();                    
        }

        public void HandleStopButtonClicked(object sender, EventArgs e)
        {
            IsEnabled = CanExecute(null);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class StopTailingCommand : ModelBase, ICommand
    {
        private readonly ITailControl _vm;
        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public StopTailingCommand(ITailControl vm)
        {
            _vm = vm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _vm.IsRunning;
        }

        public void Execute(object parameter)
        {
            _vm.StopTailing();           
        }

        public void HandleStartButtonClicked(object sender, EventArgs e)
        {
            IsEnabled = CanExecute(null);            
        }
    }

    public interface ITailControl
    {
        bool IsRunning { get; }

        TailFile SelectedTail { get; set; }

        void StartTailing();

        void StopTailing();
    }
}
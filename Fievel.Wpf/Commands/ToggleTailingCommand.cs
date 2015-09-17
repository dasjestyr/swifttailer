using System;
using System.Windows.Input;
using Fievel.Wpf.ViewModels;
using Fievel.Wpf.Models.Observable;

namespace Fievel.Wpf.Commands
{
    public class StartTailingCommand : ModelBase, ICommand
    {
        private readonly MainViewModel _vm;
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

        public StartTailingCommand(MainViewModel vm)
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
        private readonly MainViewModel _vm;
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

        public StopTailingCommand(MainViewModel vm)
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
}
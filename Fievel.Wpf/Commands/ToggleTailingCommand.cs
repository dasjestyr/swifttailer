using System;
using System.Windows.Controls;
using System.Windows.Input;
using Fievel.Wpf.ViewModels;

namespace Fievel.Wpf.Commands
{
    public class ToggleTailingCommand : ICommand
    {
        private readonly MainViewModel _vm;

        public ToggleTailingCommand(MainViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_vm.IsRunning)
            {
                _vm.StopTailing();
            }
            else
            {
                _vm.StartTailing();
            }

            var button = parameter as Button;
            if (button == null) return;
            button.IsEnabled = false;

            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;
    }
}
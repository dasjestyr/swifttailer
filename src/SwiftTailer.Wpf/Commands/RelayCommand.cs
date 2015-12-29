using System;
using System.Windows.Input;

namespace SwiftTailer.Wpf.Commands
{
    class RelayCommand : ICommand
    {
        private readonly Action<object> _relay;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> command)
        {
            _relay = command;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _relay(parameter);
        }
    }
}

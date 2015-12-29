using System;
using System.Windows.Controls;
using System.Windows.Input;
using SwiftTailer.Wpf.Models;

namespace SwiftTailer.Wpf.Commands
{
    public class PinLogLineCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var menuItem = parameter as MenuItem;
            var line = menuItem?.DataContext as LogLine;

            if (line != null)
                line.Pinned = true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class UnpinLogLineCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var menuItem = parameter as MenuItem;
            var line = menuItem?.DataContext as LogLine;

            if (line != null)
                line.Pinned = false;
        }

        public event EventHandler CanExecuteChanged;
    }
}

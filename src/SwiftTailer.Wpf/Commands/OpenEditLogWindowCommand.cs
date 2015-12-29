using System;
using System.Windows.Controls;
using System.Windows.Input;
using SwiftTailer.Wpf.Models;
using SwiftTailer.Wpf.Pages;

namespace SwiftTailer.Wpf.Commands
{
    public class OpenEditLogWindowCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var menuItem = parameter as MenuItem;

            var tail = menuItem?.DataContext as TailFile;
            if (tail == null) return;

            var editWindow = new EditLogInfo(tail);
            editWindow.ShowDialog();
        }

        public event EventHandler CanExecuteChanged;
    }
}

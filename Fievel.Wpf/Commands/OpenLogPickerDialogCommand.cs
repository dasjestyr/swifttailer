using System;
using System.Windows.Input;
using Fievel.Wpf.Pages;

namespace Fievel.Wpf.Commands
{
    public class OpenLogPickerDialogCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var dialog = new LogPicker();
            dialog.ShowDialog();
        }

        public event EventHandler CanExecuteChanged;
    }
}

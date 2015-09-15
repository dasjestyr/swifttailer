using System;
using System.Windows.Input;
using Fievel.Wpf.Data;
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
            var selectedGroup = parameter as LogGroup;
            if (selectedGroup == null) return;

            var dialog = new LogPicker(selectedGroup);
            dialog.ShowDialog();
        }

        public event EventHandler CanExecuteChanged;
    }
}

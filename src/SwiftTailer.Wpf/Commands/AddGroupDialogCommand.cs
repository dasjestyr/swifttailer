using System;
using System.Windows.Input;
using SwiftTailer.Wpf.Pages;

namespace SwiftTailer.Wpf.Commands
{
    public class AddGroupDialogCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var window = new AddGroupWindow();
            window.ShowDialog();
        }

        public event EventHandler CanExecuteChanged;
    }
}

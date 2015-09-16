using System;
using System.Windows.Input;
using Fievel.Wpf.Pages;

namespace Fievel.Wpf.Commands
{
    public class AddGroupDialogCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var window = new EditGroupWindow();
            window.ShowDialog();
        }

        public event EventHandler CanExecuteChanged;
    }
}

using System;
using System.Windows.Input;
using SwiftTailer.Wpf.Pages;

namespace SwiftTailer.Wpf.Commands
{
    public class OpenSettingsCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var w = new SettingsWindow();
            w.ShowDialog();
        }

        public event EventHandler CanExecuteChanged;
    }
}
